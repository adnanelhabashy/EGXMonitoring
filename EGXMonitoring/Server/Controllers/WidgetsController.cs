using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EGXMonitoring.Server.Services.WidgetService;
using EGXMonitoring.Shared.DTOS;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using EGXMonitoring.Server.HostedService;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using System.Threading.Tasks;


namespace EGXMonitoring.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WidgetsController : ControllerBase
    {
        private readonly IWidgetService _widgetService;
        private readonly HostedBackGroundService _backgroundService;

        public WidgetsController(IWidgetService widgetService,HostedBackGroundService backgroundService)
        {
            _widgetService = widgetService;
            _backgroundService = backgroundService;

        }
        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<ClientWidget>>>> GetWidgetInfo()
        {
            var info = await _widgetService.GetWidgetsInfo();
            return Ok(info);
        }


        [HttpPost("WidgetData")]
        public ActionResult<ServiceResponse<List<Dictionary<string, object>>>> GetWidgetData(ClientWidget widget)
        {
            ServiceResponse<List<Dictionary<string, object>>> data = new ServiceResponse<List<Dictionary<string, object>>>();
            if (widget.WidgetInfo.WIDGETTYPE == 1)
            {
                data = _widgetService.GetWidgetData(widget);
            }
            else if (widget.WidgetInfo.WIDGETTYPE == 2)
            {
                data = _widgetService.GetStatusWidgetData(widget);
            }

            return Ok(data);
        }

        [HttpPost("addwidget")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ServiceResponse<User>>> AddWidget(ClientWidget widget)
        {
            var response = await _widgetService.AddWidget(widget);
            return Ok(response);
        }

        [HttpPost("updatewidget")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ServiceResponse<User>>> UpdateWidget(ClientWidget widget)
        {
            var response = await _widgetService.UpdateWidget(widget);
            return Ok(response);
        }

        [HttpPost("deletewidget")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ServiceResponse<User>>> DeleteWidget(ClientWidget widget)
        {
            var response = await _widgetService.RemoveWidget(widget);
            return Ok(response);
        }

        [HttpGet("GetLayouts")]
        public async Task<ActionResult<ServiceResponse<List<TabLayouts>>>> GetLayouts()
        {
            var result = await _widgetService.GetTabLayouts();
            return Ok(result);
        }
        [HttpPost("SetLayouts")]
        public async Task<ActionResult<ServiceResponse<List<TabLayouts>>>> SetLayouts(List<TabLayouts> Layouts)
        {
            var response = await _widgetService.SetTabLayouts(Layouts);
            return Ok(response);
        }

        [HttpPost("DeleteLayouts")]
        public async Task<ActionResult<ServiceResponse<List<TabLayouts>>>> DeleteLayouts(List<TabLayouts> Layouts)
        {
            var response = await _widgetService.DeleteTabLayouts(Layouts);
            return Ok(response);
        }


        [HttpGet("restartBackGroundService"), AllowAnonymous]
        public async Task<IActionResult> RestartHostedService()
        {
            try
            {
                await _backgroundService.StopAsync(CancellationToken.None);
                await _backgroundService.StartAsync(CancellationToken.None);


                return Ok(new ServiceResponse<string>()
                {
                    Data = "Service Restarted Successfully.",
                    Success = true,
                    Message = "Service Restarted Successfully."
                });
            }
            catch(Exception ex)
            {
                return Ok(new ServiceResponse<string>()
                {
                    Data = "Erro Happened while restarting",
                    Success = true,
                    Message = ex.Message
                });
            }
 
        }

    }
}
