using InQuant.Framework.Data.Core.Repositories;
using InQuant.Framework.Data.Extensions;
using InQuant.Framework.Exceptions;
using InQuant.LocalizationAdmin.Models;
using InQuant.Localizations.DbStringLocalizer;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace InQuant.LocalizationAdmin.Services.Impl
{
    public class LocalizationService : ILocalizationService
    {
        private readonly IStringLocalizer<LocalizationService> _localizer;
        private readonly IRepository<Localization> _localizationRepository;
        private readonly SqlLocalizationOptions _sqlLocalizationOptions;
        private readonly string _redis_publish_channel = "localization.resource.changed";

        public LocalizationService(
            IStringLocalizer<LocalizationService> localizer,
            IOptions<SqlLocalizationOptions> sqlLocalizationOptions,
            IRepository<Localization> localizationRepository)
        {
            _localizationRepository = localizationRepository;
            _localizer = localizer;
            _sqlLocalizationOptions = sqlLocalizationOptions.Value;
        }

        public async Task DelLocalization(string resourceKey, string key)
        {
            if (string.IsNullOrWhiteSpace(resourceKey) || string.IsNullOrWhiteSpace(key))
                return;

            _localizationRepository.Delete(x => x.ResourceKey == resourceKey && x.Key == key);

            await NotifyResourceChanged(resourceKey);
        }

        public Task<List<LocalizationModel>> GetLocalizations(LocalizationsSearch search)
        {
            if (search == null)
                throw new ArgumentNullException(nameof(search));

            Expression<Func<Localization, bool>> predicate = x => x.ResourceKey == search.ResourceKey;
            if (!string.IsNullOrWhiteSpace(search.Key))
            {
                predicate = predicate.And(x => x.Key.Contains(search.Key.Trim()));
            }

            var data = _localizationRepository.Query(predicate).ToList();

            var ms = data
                .GroupBy(x => new
                {
                    ResourceKey = x.ResourceKey,
                    Key = x.Key
                })
                .Select(x => new LocalizationModel()
                {
                    ResourceKey = x.Key.ResourceKey,
                    Key = x.Key.Key,
                    Text = x.Select(y => new TextModel()
                    {
                        Text = y.Text,
                        Culture = y.Culture,
                        HasTrans = y.HasTrans
                    }).ToList()
                }).ToList();

            foreach (var m in ms)
            {
                foreach (var c in _sqlLocalizationOptions.SupportedCultures)
                {
                    if (!m.Text.Any(x => x.Culture == c))
                    {
                        m.Text.Add(new TextModel()
                        {
                            Culture = c.ToString(),
                            HasTrans = false
                        });
                    }
                }
            }

            foreach (var m in ms)
            {
                m.Text = m.Text.Where(x => x.Culture != _sqlLocalizationOptions.DefaultCulture).ToList();
            }

            return Task.FromResult(ms);
        }

        public Task<List<ResourceModel>> GetResources(string keyword)
        {
            const string sql = @"select resourceKey,count(1) as cnt from(
                        select resourceKey,`key`
                        from t_localization
                        {0}
                        group by resourceKey,`key`) as tmp group by resourceKey";

            Dictionary<string, object> paras = new Dictionary<string, object>();
            string where = string.Empty;

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                where = "where resourceKey like @keyword";
                paras.Add("keyword", $"%{keyword.Trim()}%");
            }

            var data = _localizationRepository.QueryDynamic(string.Format(sql, where), paras);

            return Task.FromResult(data.Select(x => new ResourceModel()
            {
                ResourceKey = x.resourceKey,
                Cnt = (int)x.cnt
            }).ToList());
        }

        public async Task UpdateLocalization(LocalizationUpdateModel m, int @operator)
        {
            if (string.IsNullOrWhiteSpace(m.ResourceKey))
                throw new HopexException(_localizer["分类不能为空"]);
            if (string.IsNullOrWhiteSpace(m.Key))
                throw new HopexException(_localizer["词条不能为空"]);

            const string sql = @"replace into t_localization(culture,resourceKey,`key`,text,lastModifiedTime,hasTrans) 
                                 values(@culture,@resourceKey,@key,@text,now(),@hasTrans);";

            var localizations = m.Text.Select(x => new
            {
                culture = x.Culture,
                resourceKey = m.ResourceKey,
                key = m.Key,
                text = x.Text,
                hasTrans = !string.IsNullOrWhiteSpace(x.Text)
            }).ToList();

            await _localizationRepository.Execute(sql, localizations);

            await NotifyResourceChanged(m.ResourceKey);
        }

        private async Task NotifyResourceChanged(string resourceKey)
        {
            await RedisHelper.PublishAsync(_redis_publish_channel, resourceKey);
        }

        public async Task DelResource(string resourceKey)
        {
            _localizationRepository.Delete(x => x.ResourceKey == resourceKey);
            await NotifyResourceChanged(resourceKey);
        }
    }
}
