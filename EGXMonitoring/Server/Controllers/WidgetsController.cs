using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EGXMonitoring.Server.Services.WidgetService;
using EGXMonitoring.Shared.DTOS;

namespace EGXMonitoring.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WidgetsController : ControllerBase
    {
        private readonly IWidgetService _widgetService;

        public WidgetsController(IWidgetService widgetService)
        {
            _widgetService = widgetService;
        }
        [HttpGet]
        public async Task<ActionResult<List<ServiceResponse<Widget>>>> GetWidgetsData()
        {
            var info = await _widgetService.GetWidgetsInfo();
            return Ok(info);
        }
    }
}
