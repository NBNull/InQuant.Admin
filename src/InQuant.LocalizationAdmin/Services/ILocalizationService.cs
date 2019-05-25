using InQuant.Framework.Mvc.Models;
using InQuant.LocalizationAdmin.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InQuant.LocalizationAdmin.Services
{
    public interface ILocalizationService
    {
        Task<List<ResourceModel>> GetResources(string keyword);

        Task<List<LocalizationModel>> GetLocalizations(LocalizationsSearch search);

        Task UpdateLocalization(LocalizationUpdateModel m, int @operator);

        Task DelLocalization(string resourceKey, string key);

        Task DelResource(string resourceKey);
    }
}
