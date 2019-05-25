using InQuant.Framework.Data.Core;
using InQuant.Framework.Data.Core.Attributes;
using System;

namespace InQuant.Localizations.DbStringLocalizer
{
    [TableName("t_localization")]
    public class Localization : IEntity
    {
        public long Id { get; set; }

        public string Key { get; set; }

        public string Text { get; set; }

        public string Culture { get; set; }

        public string ResourceKey { get; set; }

        public DateTime LastModifiedTime { get; set; }

        public bool HasTrans { get; set; }
    }
}
