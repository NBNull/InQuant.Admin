using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;

namespace InQuant.Localizations.DbStringLocalizer
{
    public class LocalizationModelContext
    {
        private readonly ILogger<LocalizationModelContext> _logger;
        private readonly IOptions<SqlLocalizationOptions> _options;
        private readonly SqlLocalizationOptions _sqlLocalizationOptions;
        private Dictionary<string, List<Localization>> _ldic;
        private readonly static object _lock = new object();

        public LocalizationModelContext(
            ILogger<LocalizationModelContext> logger,
           IOptions<SqlLocalizationOptions> localizationOptions)
        {
            _logger = logger;
            _options = localizationOptions;
            _sqlLocalizationOptions = localizationOptions.Value;
        }

        public void EnsureLoadAllLocations()
        {
            if (_ldic == null)
            {
                lock (_lock)
                {
                    if (_ldic == null)
                    {
                        _logger.LogInformation("初始化多语言词典");
                        using (var conn = new MySqlConnection(_sqlLocalizationOptions.DbConnectionString))
                        {
                            conn.Open();
                            var ls = conn.Query<Localization>("select * from t_localization", commandTimeout: 3);
                            _ldic = ls.GroupBy(x => x.ResourceKey)
                                .ToDictionary(x => x.Key, x => x.ToList());

                            conn.Close();
                        }
                        _logger.LogInformation("化多语言词典初始化完毕");
                    }
                }
            }
        }

        public IList<Localization> GetLocalizations(string resourceKey)
        {
            EnsureLoadAllLocations();
            if (_ldic.TryGetValue(resourceKey, out List<Localization> ls))
            {
                return ls;
            }
            return new List<Localization>();
        }
    }
}