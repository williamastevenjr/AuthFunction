using System.Threading.Tasks;
using AuthDtos;
using AuthDtos.Request;
using AuthService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ws.JwtAuth.Extensions;

namespace AuthFunction.Controllers
{
    /// <summary>
    /// ASP.NET Core Auth controller.
    /// </summary>
    [ApiController]
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
                return Unauthorized("Invalid account or password.");
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
                return Unauthorized("Invalid refresh request.");
            }
            return Ok(new BaseResponse(result));
        }

        [HttpPost("Account")]
        [AllowAnonymous]
        public async Task<ActionResult> CreateAccount([FromBody] CreateAccountRequest request)
        {
            if (!request.Password.Equals(request.ConfirmPassword))
            {
                return BadRequest("Password doesn't match confirm password");
            }
            var result = await _authService.CreateAuth(request.AccountName, request.Password);
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
            var user = JwtInspector.GetJwtUser(HttpContext);
            await _authService.RemoveRefreshTokens(user.UserId);
            return Ok();
        }
    }
}
