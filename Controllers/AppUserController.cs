using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebApiKnowledge.Models;

namespace WebApiKnowledge.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppUserController : ControllerBase
    {
        private UserManager<AppUser> userManager;
        private readonly AppSettings appSettings;
        private IMapper mapper;

        public object Configuration { get; private set; }

        public AppUserController(UserManager<AppUser> userManager, IOptions<AppSettings> appSettings, IMapper mapper)
        {
            this.userManager = userManager;
            this.appSettings = appSettings.Value;
            this.mapper = mapper;
        }

        [HttpPost]
        [Route("Register")]
        //POST: /api/AppUser/Register
        public async Task<Object> Register(AppUserModel model)
        {
            var appUser = mapper.Map<AppUser>(model);

            try
            {
                var result = await userManager.CreateAsync(appUser, model.Password);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        [Route("Login")]
        //POST: /api/AppUser/Login
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            var user = await userManager.FindByNameAsync(loginModel.UserName);
            if (user != null && await userManager.CheckPasswordAsync(user, loginModel.Password))
            {

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim("UserID", user.Id)
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(20),
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.JWT_Secret)), SecurityAlgorithms.HmacSha256Signature)
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                var token = tokenHandler.WriteToken(securityToken);
                return Ok(new { token });
            } 
            else
            {
                return BadRequest("Username or password is invalid");
            }

        }

        [HttpGet]
        [Route("GetUser")]
        [Authorize]
        //GET : /api/AppUser/GetUser
        public async Task<Object> GetUserProfile()
        {
            string userId = User.Claims.First(u => u.Type == "UserID").Value;
            var user = await userManager.FindByIdAsync(userId);
            return new
            {
                user.FullName,
                user.Email,
                user.UserName
            };
        }
    }
}   