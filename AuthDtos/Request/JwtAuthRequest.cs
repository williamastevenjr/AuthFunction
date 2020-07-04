using System.ComponentModel.DataAnnotations;

namespace AuthDtos.Request
{
    public class JwtAuthRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

    }
}
