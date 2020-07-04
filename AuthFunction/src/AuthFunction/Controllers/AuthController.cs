using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public IEnumerable<string> Get([FromBody] JwtAuthRequest request)
        {
            return new List<string>{"a"};
        }
    }
}
