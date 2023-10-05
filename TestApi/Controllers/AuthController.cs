using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestApi.ApplicationDbContext;
using TestApi.Models;
using TestApi.Services;

namespace TestApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _dbcontext;
        private readonly IAuthService _iauthservice;
        public AuthController(AppDbContext dbcontext, IAuthService iauthservice)
        {
            _dbcontext = dbcontext; 
            _iauthservice = iauthservice;   
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("Register")]
        public async Task<AuthModel> Register(RegisterModel model)
        {
            return !ModelState.IsValid ? new AuthModel { Message = "Error" } : await this._iauthservice.RegisterAsync(model);
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public async Task<AuthModel> Login(SignInModel model)
        {
            return !ModelState.IsValid ? new AuthModel { Message = "Error" } : await this._iauthservice.Login(model);
        }
    }
}
