using System;
using System.Collections.Generic;
using System.Text;

namespace InQuant.LocalizationAdmin.Models
{
    public class LocalizationModel
    {
        public string ResourceKey { get; set; }

        public string Key { get; set; }

        public List<TextModel> Text { get; set; }
    }

    public class TextModel
    {
        public string Culture { get; set; }

        public string Text { get; set; }

        public bool HasTrans { get; set; }
    }
}
