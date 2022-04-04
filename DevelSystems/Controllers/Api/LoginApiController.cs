using DevelSystem.Context;
using DevelSystem.Data;
using DevelSystem.Interfaces;
using DevelSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DevelSystem.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class LoginApiController : Controller
    {

        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUnitOfWorkFactory uowFactory;
        private readonly IConfiguration _configuration;
        //private readonly IRepository<Clientes> clientesRepository;


        public LoginApiController(UserManager<IdentityUser> userManager, IConfiguration configuration, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;

            ApplicationDbContext context = new ApplicationDbContext();
            this.uowFactory = new EntityFrameworkUnitOfWorkFactory(context);

        }





        [HttpPost]
        [Route("Login")]
        public async Task<object> Login([FromBody] AuthRequest model)
        {
            if (model == null)
                return BadRequest();

            var usuario = model.Usuario;
            var password = model.Password;
            var result = await _signInManager.PasswordSignInAsync(usuario, password, false, false);
            if (result.Succeeded)
            {
                var appUser = _userManager.Users.SingleOrDefault(r => r.UserName == usuario);
                string Token = await GenerateJwtToken(usuario, appUser);

                return Ok(Token);
            }
            else
                return BadRequest();
        }

        private async Task<string> GenerateJwtToken(string email, IdentityUser user)
        {
            await Task.Delay(1);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("JwtKey")));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            int dias = Convert.ToInt32(_configuration.GetValue<string>("JwtExpireDays"));
            var expires = DateTime.Now.AddDays(Convert.ToDouble(dias));

            var token = new JwtSecurityToken(
                _configuration.GetValue<string>("JwtIssuer"),
                _configuration.GetValue<string>("JwtAudience"),
                claims,
                expires: expires,
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}