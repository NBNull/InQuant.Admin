using InQuant.Authorization.Token;
using InQuant.Framework.Data.Core.Repositories;
using InQuant.Framework.Data.Extensions;
using InQuant.Framework.Exceptions;
using InQuant.Framework.Mvc.Models;
using InQuant.Security.Models;
using InQuant.Security.Models.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace InQuant.Security.Services.Impl
{
    public class RoleService : IRoleService
    {
        private readonly ILogger<RoleService> _logger;
        private readonly IStringLocalizer<RoleService> _localizer;
        private readonly IRepository<AdminRole> _roleRepository;
        private readonly IRepository<UserRole> _userRoleRepository;
        private readonly IRepository<RolePermission> _rolePermissionRepository;
        private readonly IDistributedCache _distributedCache;
        private readonly ITokenService _tokenService;
        private readonly IAdminUserService _adminUserService;

        public RoleService(ILogger<RoleService> logger,
            IStringLocalizer<RoleService> localizer,
            ITokenService tokenService,
            IRepository<UserRole> userRoleRepository,
            IDistributedCache distributedCache,
            IAdminUserService adminUserService,
            IRepository<AdminRole> roleRepository,
            IRepository<RolePermission> rolePermissionRepository)
        {
            _localizer = localizer;
            _logger = logger;
            _adminUserService = adminUserService;
            _userRoleRepository = userRoleRepository;
            _distributedCache = distributedCache;
            _roleRepository = roleRepository;
            _rolePermissionRepository = rolePermissionRepository;
            _tokenService = tokenService;
        }

        public async Task<int> Create(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
                throw new HopexException(_localizer["角色名称不能为空"]);

            if ((await _roleRepository.CountAsync(x => x.Name == roleName)) > 0)
                throw new HopexException(_localizer["名称已存在"]);

            var role = new AdminRole()
            {
                Name = roleName
            };

            int id = await _roleRepository.InsertAsync(role);

            return id;
        }

        public async Task Delete(int roleId)
        {
            _logger.LogInformation($"删除角色：{roleId}");

            await _roleRepository.DeleteAsync(x => x.Id == roleId);
        }

        public async Task<Pagination<AdminRole>> Search(RoleSearch search, Pager page)
        {
            Expression<Func<AdminRole, bool>> predicate = x => true;
            if (!string.IsNullOrWhiteSpace(search.RoleName))
            {
                predicate = predicate.And(x => x.Name.Contains(search.RoleName.Trim()));
            }

            var total = await _roleRepository.CountAsync(predicate);
            var data = await _roleRepository.Query(predicate)
                .OrderByDescending(x => x.Id)
                .Offset(page.Offset)
                .Take(page.Limit)
                .ToListAsync();

            var ret = new Pagination<AdminRole>(total, data, page);

            return ret;
        }

        public async Task Update(int roleId, string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                throw new HopexException(_localizer["角色名称不能为空"]);

            roleName = roleName.Trim();

            var r = await _roleRepository.GetAsync(roleId);
            if (r == null)
                throw new HopexException(_localizer["角色不存在"]);

            if ((await _roleRepository.CountAsync(x => x.Id != roleId && x.Name == roleName)) > 0)
                throw new HopexException(_localizer["名称已存在"]);

            r.Name = roleName;
            await _roleRepository.UpdateAsync(r);
        }

        public async Task UpdateRolePermission(int roleId, IList<string> permissionNames)
        {
            var r = await _roleRepository.GetAsync(roleId);

            if (r == null)
                throw new HopexException(_localizer["角色不存在"]);

            await _rolePermissionRepository.DeleteAsync(x => x.RoleId == roleId);

            await _rolePermissionRepository.InsertAsync(permissionNames.Select(x => new RolePermission()
            {
                RoleId = roleId,
                PermissionName = x
            }));

            await _distributedCache.RemoveAsync(string.Format(CacheKeyConsts._cache_role_permission, roleId));
        }

        public async Task UpdateUserRole(int userId, IEnumerable<int> roleIds)
        {
            if (userId <= 0) throw new ArgumentException(_localizer["用户ID错误"]);
            if (roleIds == null) throw new ArgumentNullException(nameof(roleIds));

            var user = (await _adminUserService.GetUsers(true, userId)).FirstOrDefault();
            if (user == null)
                throw new HopexException(_localizer["用户不存在"]);

            await _userRoleRepository.DeleteAsync(x => x.UserId == userId);

            if (roleIds.Count() > 0)
            {
                var roles = await _roleRepository.Query(x => roleIds.Contains(x.Id)).ToListAsync();
                if (roles.Count == 0)
                    throw new HopexException(_localizer["角色不存在"]);

                List<UserRole> urs = new List<UserRole>();
                foreach (var role in roles)
                {
                    urs.Add(new UserRole()
                    {
                        RoleId = role.Id,
                        UserId = user.Id
                    });
                }
                await _userRoleRepository.InsertAsync(urs);
            }

            await _distributedCache.RemoveAsync(string.Format(CacheKeyConsts._cache_user_roles, userId));
        }

        public async Task DeleteUserRole(int userId)
        {
            await _userRoleRepository.DeleteAsync(x => x.UserId == userId);

            await _distributedCache.RemoveAsync(string.Format(CacheKeyConsts._cache_user_roles, userId));
        }

        public async Task<List<UserRoleModel>> GetUserRoles(params int[] userIds)
        {
            const string sql = @"SELECT {0}
                FROM t_user_role ur
                inner join t_role r on ur.roleId=r.id 
                {1} ";

            var paras = new Dictionary<string, object>();

            List<string> wheres = new List<string>();

            if (userIds.Length > 0)
            {
                wheres.Add("FIND_IN_SET(ur.userId, @userId)");
                paras.Add("userId", string.Join(",", userIds));
            }

            string where = wheres.Count == 0 ? string.Empty : $" where {string.Join(" and ", wheres)}";

            var data = await _userRoleRepository.QueryDynamicAsync(string.Format(sql, "ur.UserId,ur.RoleId,r.name as RoleName", where), paras);

            var gs = data.GroupBy(x => x.UserId)
                .Select(x => new UserRoleModel()
                {
                    UserId = x.Key,
                    RoleIds = x.Select(y => (int)y.RoleId).ToList(),
                    Roles = string.Join(",", x.Select(y => y.RoleName))
                }).ToList();

            return gs;
        }
    }
}
