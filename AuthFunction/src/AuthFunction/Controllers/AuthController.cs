using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthDtos;
using AuthDtos.Request;
using AuthService.Interfaces;
using JwtAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthFunction.Controllers
{
    /// <summary>
    /// ASP.NET Core Auth controller.
    /// </summary>
    [Route("api/[controller]")]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [AllowAnonymous]
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
        [AllowAnonymous]
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
        [AllowAnonymous]
        public async Task<ActionResult> CreateAccount([FromBody] CreateAccountRequest request)
        {
            var result = await _authService.CreateAuth(request.Username, request.Password);
            if (result == null)
            {
                return BadRequest();
            }
            return Ok(new BaseResponse(result));
        }

        [HttpPost("Logout")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Logout()
        {
            var user = JwtUser.GetJwtUser(HttpContext);
            await _authService.RemoveRefreshTokens(user.UserGuid);
            return Ok();
        }
    }
}
