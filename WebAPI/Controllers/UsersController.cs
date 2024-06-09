using Business.Abstracts;
using Business.Dtos.Requests.User;
using Core.DataAccess.Paging;
using log4net;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILog _logger;

        public UsersController(IUserService userService, ILog logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetList([FromQuery] PageRequest pageRequest)
        {
            _logger.Info("Tüm kullanıcılar listelendi.");
            var result = await _userService.GetListAsync(pageRequest);
            return Ok(result);
        }

        [HttpGet("getbyid")]
        public async Task<IActionResult> Get([FromQuery] Guid id)
        {
            _logger.Info($"Kullanıcı ID'ye göre getirildi: {id}");
            var result = await _userService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpGet("getbymail")]
        public async Task<IActionResult> GetByMail([FromQuery] string mail)
        {
            _logger.Info($"Mail adresine göre kullanıcı getirildi: {mail}");
            var result = await _userService.GetByMailUserAsync(mail);
            return Ok(result);
        }

        [HttpGet("activate")]
        public async Task<IActionResult> Activate([FromQuery] string email)
        {
            _logger.Info($"Kullanıcı aktive edildi: {email}");
            var result = await _userService.ActivateUserAsync(email);
            return Ok(result);
        }

        [HttpDelete("deletebyid")]
        public async Task<IActionResult> DeleteById([FromBody] Guid id)
        {
            _logger.Info($"Kullanıcı silindi (ID): {id}");
            var result = await _userService.DeleteByIdAsync(id);
            return Ok(result);
        }

        [HttpDelete("deletebymail")]
        public async Task<IActionResult> DeleteByMail([FromBody] string email)
        {
            _logger.Info($"Kullanıcı silindi (Mail): {email}");
            var result = await _userService.DeleteByMailAsync(email);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateUserRequest updateUserRequest)
        {
            _logger.Info($"Kullanıcı güncellendi: {updateUserRequest.Id}");
            var result = await _userService.UpdateAsync(updateUserRequest);
            return Ok(result);
        }
    }
}
