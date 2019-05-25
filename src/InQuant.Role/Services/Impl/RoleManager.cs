using InQuant.Authorization;
using InQuant.Authorization.Permissions;
using InQuant.Cache;
using InQuant.Framework.Data.Core.Repositories;
using InQuant.Framework.Exceptions;
using InQuant.Security.Models;
using InQuant.Security.Models.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InQuant.Security.Services.Impl
{
    public class RoleManager : IRoleManager
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IRepository<AdminRole> _roleRepository;
        private readonly IRepository<RolePermission> _rolePermissionRepository;
        private readonly IRepository<UserRole> _userRoleRepository;
        private readonly IMemoryCache _memoryCache;
        private readonly IEnumerable<IPermissionProvider> _permissionProviders;

        public RoleManager(IDistributedCache distributedCache,
            IRepository<AdminRole> roleRepository,
            IMemoryCache memoryCache,
            IEnumerable<IPermissionProvider> permissionProviders,
            IRepository<UserRole> userRoleRepository,
            IRepository<RolePermission> rolePermissionRepository)
        {
            _distributedCache = distributedCache;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _memoryCache = memoryCache;
            _permissionProviders = permissionProviders;
            _rolePermissionRepository = rolePermissionRepository;
        }

        public async Task<Role> GetRoleById(int id)
        {
            var r = await _roleRepository.GetAsync(id);
            if (r == null)
                return default(Role);

            return r;
        }

        public async Task<IList<Permission>> GetRolePermissions(int roleId)
        {
            if (roleId <= 0) return null;

            string key = string.Format(CacheKeyConsts._cache_role_permission, roleId);

            return await _distributedCache.GetAsync(key, async () =>
             {
                 List<Permission> items = new List<Permission>();
                 var rs = await _rolePermissionRepository.Query(x => x.RoleId == roleId).ToListAsync();
                 if (rs.Count > 0)
                 {
                     var permissions = (await GetAllPermissions()).ToDictionary(x => x.Name, x => x);

                     foreach (var r in rs)
                     {
                         if (permissions.ContainsKey(r.PermissionName))
                             items.Add(permissions[r.PermissionName]);
                     }
                 }

                 return items;
             });
        }

        public Task<IList<Permission>> GetAllPermissions()
        {
            var val = _memoryCache.Get<List<Permission>>(CacheKeyConsts._cache_all_permission);
            if (val == null)
            {
                val = _permissionProviders.SelectMany(x => x.GetPermissions()).ToList();

                var repeats = val.GroupBy(x => x.Name).Where(k => k.Count() > 1).ToList();

                if (repeats.Count > 0)
                {
                    throw new HopexException($"权限项名称{string.Join(",", repeats.Select(g => g.Key))}重复");
                }

                _memoryCache.Set(CacheKeyConsts._cache_all_permission, val);
            }

            return Task.FromResult<IList<Permission>>(val);
        }

        /// <summary>
        /// 获取用户的权限（缓存1小时）
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IList<Permission>> GetUserPermissions(int userId)
        {
            if (userId <= 0) return null;

            var rs = await GetUserRoles(userId) ?? new List<Role>();

            HashSet<Permission> permissions = new HashSet<Permission>();
            foreach (var r in rs)
            {
                var ps = await GetRolePermissions(r.Id);
                if (ps == null || ps.Count == 0)
                    continue;

                permissions.UnionWith(ps);
            }

            return permissions.ToList();
        }

        public async Task<IList<Role>> GetUserRoles(int userId)
        {
            if (userId <= 0)
                return new List<Role>();

            string key = string.Format(CacheKeyConsts._cache_user_roles, userId);
            return await _distributedCache.GetAsync(key, async () =>
             {
                 var rs = await _userRoleRepository.Query(x => x.UserId == userId).ToListAsync();
                 var roleIds = rs.Select(x => x.RoleId).ToList();

                 if (roleIds.Count == 0)
                     return default(IList<Role>);

                 var data = (await _roleRepository.Query(x => roleIds.Contains(x.Id)).ToListAsync())
                     .Select(x => (Role)x)
                     .ToList();

                 return data;
             });
        }
    }
}
