﻿using EGXMonitoring.Shared.DTOS;

namespace EGXMonitoring.Client.Services.WidgetService
{
    public interface IWidgetService
    {
        Task<ServiceResponse<List<ClientWidget>>> GetWidgets();

    }
}
