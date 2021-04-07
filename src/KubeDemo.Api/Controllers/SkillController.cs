using Microsoft.AspNetCore.Mvc;

namespace KubeDemo.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SkillController : ControllerBase
    {
        [HttpGet, Route("~/")] public string Index() => nameof(SkillController);
    }
}