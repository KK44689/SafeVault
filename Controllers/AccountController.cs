using Microsoft.AspNetCore.Mvc;
using SecureAuthApp.Services;

namespace SecureAuthApp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        private IJwtService _jwtService;

        public AccountController(IJwtService jwtService)
        {
            _jwtService = jwtService;
        }

        [HttpPost("Login")]
        public ActionResult Login(UserModel model)
        {
            if (!ValidationHelpers.IsInputValid(model.Username) || !ValidationHelpers.IsInputValid(model.Password))
            {
                return Unauthorized("Invalid input.");
            }

            Console.WriteLine("hashed password : " + BCrypt.Net.BCrypt.HashPassword("securepassword123"));

            string sanitizedUsername = ValidationHelpers.SanitizeForXss(model.Username);
            string sanitizedPassword = ValidationHelpers.SanitizeForXss(model.Password);

            var user = UserStore.Users.Find(user => user.Username == sanitizedUsername);

            if (user is not null && BCrypt.Net.BCrypt.Verify(sanitizedPassword, user.Password))
            {
                string jwtToken = _jwtService.GenerateToken(sanitizedUsername, model.Role);
                return Ok($"Login Success:Token: {jwtToken}");
            }

            return Unauthorized("Invalid login attempt.");
        }

        [HttpPost("Register")]
        public ActionResult Register(UserModel model)
        {
            if (!ModelState.IsValid || !ValidationHelpers.IsInputValid(model.Username) || !ValidationHelpers.IsInputValid(model.Password))
            {
                return Unauthorized("Invalid or unsafe input.");
            }

            string sanitizedUsername = ValidationHelpers.SanitizeForXss(model.Username);
            string sanitizedPassword = ValidationHelpers.SanitizeForXss(model.Password);

            if (UserStore.Users.Any(user => user.Username == sanitizedUsername))
            {
                return Unauthorized("Username already exists.");
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(sanitizedPassword);
            var newUser = new UserModel { Username = sanitizedUsername, Password = sanitizedPassword, Role = model.Role };
            UserStore.Users.Add(newUser);

            // 🔐 Issue JWT upon successful registration
            string jwtToken = _jwtService.GenerateToken(sanitizedUsername, model.Role);
            return Ok($"Register Success:Token: {jwtToken}");
        }
    }
}