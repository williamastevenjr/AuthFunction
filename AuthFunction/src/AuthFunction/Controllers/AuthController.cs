using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthDtos;
using AuthDtos.Request;
using AuthService.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AuthFunction.Controllers
{
    /// <summary>
    /// ASP.NET Core Auth controller.
    /// </summary>
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        public async Task<ActionResult> JwtAuth([FromBody] JwtAuthRequest request)
        {
            var result = await _authService.Auth(request);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(new BaseResponse(result));
        }

        [HttpPost("refresh_token")]
        public async Task<ActionResult> JwtRefresh([FromBody] AuthRefreshTokenRequest request)
        {
            var result = await _authService.RefreshTokenAuth(request);
            if (result == null)
            {
                return BadRequest();
            }
            return Ok(new BaseResponse(result));
        }

        [HttpPost("Account")]
        public async Task<ActionResult> CreateAccount([FromBody] CreateAccountRequest request)
        {
            var result = await _authService.CreateAuth(request.Username, request.Password);
            if (result == null)
            {
                return BadRequest();
            }
            return Ok(new BaseResponse(result));
        }
    }
}
