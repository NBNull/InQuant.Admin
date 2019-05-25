using InQuant.Authorization.Permissions;
using InQuant.Authorization.Token;
using InQuant.Framework.Data.Core.Repositories;
using InQuant.Framework.Data.Extensions;
using InQuant.Framework.Exceptions;
using InQuant.Framework.Mvc.Models;
using InQuant.Framework.Utils;
using InQuant.Security.Models.AdminUsers;
using InQuant.Security.Models.Entities;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace InQuant.Security.Services.Impl
{
    public class AdminUserService : IAdminUserService
    {
        private readonly IStringLocalizer<AdminUserService> _localizer;
        private readonly ILogger<AdminUser> _logger;
        private readonly ITokenService _tokenService;
        private readonly IRepository<AdminUser> _adminUserRepository;
        private readonly IRoleManager _roleManager;

        public AdminUserService(ILogger<AdminUser> logger,
            IStringLocalizer<AdminUserService> localizer,
            IRoleManager roleManager,
            ITokenService tokenService,
            IRepository<AdminUser> adminUserRepository)
        {
            _logger = logger;
            _localizer = localizer;
            _roleManager = roleManager;
            _tokenService = tokenService;
            _adminUserRepository = adminUserRepository;
        }

        public async Task<List<AdminUser>> GetUsers(bool onlyValid = true, params int[] userIds)
        {
            if (userIds.Length == 0) return new List<AdminUser>();

            Expression<Func<AdminUser, bool>> predicate = x => userIds.Contains(x.Id);
            if (onlyValid)
            {
                predicate = predicate.And(x => x.IsValid);
            }
            predicate = predicate.And(x => x.IsDeleted == false);

            var data = await _adminUserRepository.Query(predicate).ToListAsync();

            return data.ToList();
        }

        public async Task<(LoginRpsModel rpsModel, ClaimsPrincipal principal)> Login(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName)) throw new HopexException(_localizer["用户名不能为空"]);
            if (string.IsNullOrWhiteSpace(password)) throw new HopexException(_localizer["密码不能为空"]);

            var user = await _adminUserRepository.Query(x => x.UserName == userName && x.IsValid && x.IsDeleted == false).SingleOrDefaultAsync();

            if (user == null) throw new HopexException(_localizer["用户不存在"]);

            if (!HashUtil.PasswordHashCheck(user.Salt, user.PasswordHash, password))
                throw new HopexException(_localizer["密码不正确"]);

            var (token, principal) = await _tokenService.BuildToken(new TokenInfo()
            {
                UserName = user.UserName,
                UserId = user.Id,
                IsAdmin = user.IsAdmin
            });

            return (new LoginRpsModel()
            {
                UserId = user.Id,
                Token = token
            }, principal);
        }

        public async Task Logout(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return;

            await _tokenService.DelToken(token);
        }

        public async Task<Pagination<AdminUser>> Search(AdminUserSearch search, Pager pager)
        {
            Expression<Func<AdminUser, bool>> predicate = x => x.IsValid && x.IsDeleted == false;
            if (!string.IsNullOrWhiteSpace(search.UserName))
            {
                predicate = predicate.And(x => x.UserName.Contains(search.UserName.Trim()));
            }

            int total = await _adminUserRepository.CountAsync(predicate);
            var data = await _adminUserRepository.Query(predicate)
                .OrderByDescending(x => x.Id)
                .Offset(pager.Offset)
                .Take(pager.Limit)
                .ToListAsync();

            var ret = new Pagination<AdminUser>(total, data, pager);
            return ret;
        }

        public async Task<int> Create(AdminUserCreateModel m, int creator)
        {
            if (m == null) throw new ArgumentNullException(nameof(m));
            if (string.IsNullOrWhiteSpace(m.UserName))
                throw new HopexException(_localizer["用户名不能为空"]);
            if (string.IsNullOrWhiteSpace(m.Password))
                throw new HopexException(_localizer["密码不能为空"]);
            if (m.Password.Contains(' '))
                throw new HopexException(_localizer["密码不能包含空格"]);
            if (!PasswordStrongCheck(m.Password))
                throw new HopexException(_localizer["密码强度不够（至少6位数）"]);

            if ((await _adminUserRepository.CountAsync(x => x.UserName == m.UserName && x.IsValid && x.IsDeleted == false)) != 0)
                throw new HopexException(_localizer["用户名已存在"]);

            var (salt, passwordHash) = HashUtil.PasswordHash(m.Password);

            var user = new AdminUser()
            {
                LastModifiedTime = DateTime.Now,
                LastModifier = creator,
                IsValid = true,
                UserName = m.UserName.Trim(),
                Salt = salt,
                PasswordHash = passwordHash,
                IsAdmin = m.IsAdmin
            };

            int id = await _adminUserRepository.InsertAsync(user);

            return id;
        }

        public async Task Update(AdminUserUpdateModel m, int modifier)
        {
            if (m == null) throw new ArgumentNullException(nameof(m));
            if (string.IsNullOrWhiteSpace(m.UserName)) throw new HopexException(_localizer["用户名不能为空"]);

            var user = _adminUserRepository.Get(m.Id);
            if (user == null) throw new HopexException(_localizer["用户不存在"]);

            if ((await _adminUserRepository.CountAsync(x => x.UserName == m.UserName && x.IsValid && x.IsDeleted == false && x.Id != m.Id)) != 0)
                throw new HopexException(_localizer["用户名已存在"]);

            user.UserName = m.UserName;
            user.IsAdmin = m.IsAdmin;
            user.LastModifiedTime = DateTime.Now;
            user.LastModifier = modifier;

            await _adminUserRepository.UpdateAsync(user);
        }

        public async Task Delete(int userId, int @operator)
        {
            var r = await _adminUserRepository.GetAsync(userId);
            if (r != null)
            {
                r.IsDeleted = true;
                r.LastModifiedTime = DateTime.Now;
                r.LastModifier = @operator;
                await _adminUserRepository.DeleteAsync(r);
            }
        }

        /// <summary>
        /// 校验密码强度
        /// 最小6位数 
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        private bool PasswordStrongCheck(string password)
        {
            if (password.Length < 6)
                return false;

            return true;
        }

        public async Task ChangePassword(int userId, string newPassword, int @operator)
        {
            if (newPassword.Contains(' '))
                throw new HopexException(_localizer["密码不能包含空格"]);
            if (!PasswordStrongCheck(newPassword))
                throw new HopexException(_localizer["密码强度不够（至少6位数）"]);

            var user = await _adminUserRepository.GetAsync(userId);
            if (user == null)
                throw new HopexException(_localizer["用户不存在"]);

            if (HashUtil.PasswordHashCheck(user.Salt, user.PasswordHash, newPassword))
                throw new HopexException(_localizer["新密码不能等于旧密码"]);

            var (salt, passwordHash) = HashUtil.PasswordHash(newPassword);

            user.PasswordHash = passwordHash;
            user.Salt = salt;
            user.LastModifiedTime = DateTime.Now;
            user.LastModifier = @operator;

            await _adminUserRepository.UpdateAsync(user);

            await _tokenService.DelToken(userId);
        }



        public async Task<List<AdminUser>> GetsByIds(params int[] agentIds)
        {
            if (agentIds.Length == 0) return new List<AdminUser>();

            var data = await _adminUserRepository.Query(x => agentIds.Contains(x.Id)).ToListAsync();            

            return data.ToList();
        }

    }
}
