using System.Collections.Generic;

namespace InQuant.LocalizationAdmin.Models
{
    public class LocalizationUpdateModel
    {
        public string ResourceKey { get; set; }

        public string Key { get; set; }

        public List<TextModel> Text { get; set; }
    }
}
