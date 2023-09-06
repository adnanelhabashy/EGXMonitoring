﻿using Microsoft.AspNetCore.Http;
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
        [Authorize]
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
                 data = _widgetService.GetWidgetData(widget.WidgetInfo);
            }
            else if (widget.WidgetInfo.WIDGETTYPE == 2)
            {
                 data = _widgetService.GetStatusWidgetData(widget.WidgetInfo);
            }
               
            return Ok(data);
        }


    }
}
