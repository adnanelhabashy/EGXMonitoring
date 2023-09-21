using EGXMonitoring.Server.Services.MailService;
using EGXMonitoring.Shared.DTOS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EGXMonitoring.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class MailController : ControllerBase
    {
        private readonly IMailService _mailService;
        public MailController(IMailService mailService)
        {
            _mailService = mailService;
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<Mail>>> GetWidgetInfo()
        {
            var response = await _mailService.GetMail();
            return Ok(response);
        }

        [HttpPost("addupdate")]
        public async Task<ActionResult<ServiceResponse<User>>> AddOrEditMail(Mail mail)
        {
            var response = await _mailService.AddOrEdit(mail);
            return Ok(response);
        }
    }
}
