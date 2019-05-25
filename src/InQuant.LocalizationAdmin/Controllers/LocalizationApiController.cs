using InQuant.Authorization.Token;
using InQuant.LocalizationAdmin.Models;
using InQuant.LocalizationAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace InQuant.LocalizationAdmin.Controllers
{
    [Route("/api/localization/[action]")]
    [Authorize]
    [ApiController]
    public class LocalizationApiController : ControllerBase
    {
        private readonly ILocalizationService _localizationService;

        public LocalizationApiController(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
        }

        [HttpGet]
        public async Task<ActionResult> GetResources(string keyword)
        {
            var data = await _localizationService.GetResources(keyword);

            return Ok(data);
        }

        [HttpGet]
        public async Task<ActionResult> GetLocalizations([FromQuery]LocalizationsSearch search)
        {
            var data = await _localizationService.GetLocalizations(search);

            return Ok(data);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateLocalization(LocalizationUpdateModel m)
        {
            await _localizationService.UpdateLocalization(m, User.GetId());

            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult> DelLocalization(string resourceKey, string key)
        {
            await _localizationService.DelLocalization(resourceKey, key);

            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult> DelResource(string resourceKey)
        {
            await _localizationService.DelResource(resourceKey);

            return Ok();
        }
    }
}
