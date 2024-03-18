using Google.Authenticator;
using GoogleAuthenticatorAPIService.Models;
using Microsoft.AspNetCore.Mvc;


namespace GoogleAuthenticatorAPIService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GoogleAuthenticatorApiController : ControllerBase
    {
        private readonly IConfiguration _configuration;
		private readonly ILogger<GoogleAuthenticatorApiController> _logger;

        public GoogleAuthenticatorApiController(ILogger<GoogleAuthenticatorApiController> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

		[HttpPost]
		[Route("Login")]
		public async Task<IActionResult> Login([FromBody] Login login)
		{
			if (login.UserName == "Admin" && login.Password == "12345")
			{
				return Ok(new { Message = "Login successful" });
			}
			else
			{
				return Unauthorized(new { Message = "Invalid credentials" });
			}
		}

		[HttpPost]
		[Route("TwoFactorAuthenticate")]
		public async Task<IActionResult> TwoFactorAuthenticate([FromBody] TwoFactorAuthenticationModel model)
		{
			string googleAuthKey = _configuration["GoogleAuthKey"];

			TwoFactorAuthenticator twoFactorAuthenticator = new TwoFactorAuthenticator();
			bool isValid = twoFactorAuthenticator.ValidateTwoFactorPIN(googleAuthKey, model.CodeDigit);

			if (isValid)
			{
				return Ok(new { Message = "Two-factor authentication successful" });
			}
			else
			{
				return Unauthorized(new { Message = "Invalid two-factor authentication code" });
			}
		}

		[HttpPost]
		[Route("Logoff")]
		public async Task<IActionResult> Logoff()
		{
			try
			{
				HttpContext.Session.Clear();
				return Ok(new { Message = "Logged off successfully" });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { Message = "An error occurred while logging off", Error = ex.Message });
			}
		}

	}
}
