using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace InQuant.Localizations.DbStringLocalizer
{
    public class SqlStringLocalizer : IStringLocalizer
    {
        private readonly Dictionary<string, Dictionary<string, string>> _localizations;
        private readonly LocalizationModelContext _modelContext;
        private readonly string _resourceKey;
        private readonly SqlLocalizationOptions _options;

        public SqlStringLocalizer(Dictionary<string, Dictionary<string, string>> localizations,
            LocalizationModelContext modelContext,
            string resourceKey,
            SqlLocalizationOptions options)
        {
            _modelContext = modelContext;
            _localizations = localizations;
            _resourceKey = resourceKey;
            _options = options;
        }
        public LocalizedString this[string name]
        {
            get
            {
                if (name == null)
                {
                    throw new ArgumentNullException(nameof(name));
                }

                var text = GetText(name);

                return new LocalizedString(name, text.text, text.notSucceed);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                if (name == null)
                {
                    throw new ArgumentNullException(nameof(name));
                }

                var text = GetText(name);
                var str = string.Format(text.text ?? name, arguments);

                return new LocalizedString(name, str, text.notSucceed);
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            string culture = CultureInfo.CurrentCulture.Name;

            return _localizations.Keys.Select(x =>
            {
                if (_localizations.TryGetValue(x, out Dictionary<string, string> dic))
                {
                    if (dic.TryGetValue(culture, out string value))
                    {
                        return new LocalizedString(x, value, false);
                    }
                }
                return new LocalizedString(x, x, true);
            });
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return new SqlStringLocalizer(_localizations,
                _modelContext,
                _resourceKey,
                _options);
        }

        private (string text, bool notSucceed) GetText(string key)
        {
            var culture = CultureInfo.CurrentCulture.Name;

            //如果当前是默认语言，直接返回key
            if (culture == _options.DefaultCulture)
            {
                return (key, false);
            }

            bool notSucceed = true;

            if (_localizations.TryGetValue(key, out Dictionary<string, string> dic))
            {
                if (dic.TryGetValue(culture, out string result) && !string.IsNullOrWhiteSpace(result))
                {
                    notSucceed = false;
                    return (result, notSucceed);
                }
            }

            return (key, notSucceed);
        }
    }
}
