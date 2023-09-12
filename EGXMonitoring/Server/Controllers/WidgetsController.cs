using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EGXMonitoring.Server.Services.WidgetService;
using EGXMonitoring.Shared.DTOS;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace EGXMonitoring.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WidgetsController : ControllerBase
    {
        private readonly IWidgetService _widgetService;

        public WidgetsController(IWidgetService widgetService)
        {
            _widgetService = widgetService;
        }
        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<ClientWidget>>>> GetWidgetInfo()
        {
            var info = await _widgetService.GetWidgetsInfo();
            return Ok(info);
        }


        [HttpPost("WidgetData")]
        public  ActionResult<ServiceResponse<List<Dictionary<string, object>>>> GetWidgetData(ClientWidget widget)
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
        public async Task<ActionResult<ServiceResponse<User>>> AddWidget(ClientWidget widget)
        {
            var response = await _widgetService.AddWidget(widget);
            return Ok(response);
        }

        [HttpPost("updatewidget")]
        public async Task<ActionResult<ServiceResponse<User>>> UpdateWidget(ClientWidget widget)
        {
            var response = await _widgetService.UpdateWidget(widget);
            return Ok(response);
        }

        [HttpPost("deletewidget")]
        public async Task<ActionResult<ServiceResponse<User>>> DeleteWidget(ClientWidget widget)
        {
            var response = await _widgetService.RemoveWidget(widget);
            return Ok(response);
        }


    }
}
