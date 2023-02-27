using AdminCompanyEmpManagementSystem.Identity;
using AdminCompanyEmpManagementSystem.Models.DTOs;
using AdminCompanyEmpManagementSystem.Models;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AdminCompanyEmpManagementSystem.Services.IServices;

namespace AdminCompanyEmpManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtManager _jwtManager;

        private readonly IMapper _mapper;


        public AuthenticationController(IUserService userService, IMapper mapper, IJwtManager jwtManager)
        {
            _userService = userService;
            _mapper = mapper;
            _jwtManager = jwtManager;

        }
        [Route("Login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserLoginDetail user)
        {
            // here we check user is not register directly come to login
            if (await _userService.IsUnique(user.UserName)) return Ok(new {Status= 0, Message = "Please Register first then login!!!" });

            // here we will authenticatae the user.
            var userAuthorize = await _userService.AuthenticateUser(user.UserName, user.Password);
            if (userAuthorize == null) return Ok(new { Status= -1,Message="Invalid Login Ceredentials!!!!"});

            return Ok(new { Status= 1, Token = userAuthorize.Token, RefreshToken = userAuthorize.RefreshToken,Role = userAuthorize.Role });
        }
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDTO userRegisterDetail)
        {
            // here we will check the model is valid or not 
            if (userRegisterDetail == null || !ModelState.IsValid) return BadRequest();

            // here we will map the UserRegisterDTO data to ApplicationUser.
            var ApplicationUserDetail = _mapper.Map<ApplicationUser>(userRegisterDetail);
            ApplicationUserDetail.PasswordHash = userRegisterDetail.Password;

           // here we will check the user is already register or not.
            if (!await _userService.IsUnique(userRegisterDetail.UserName)) return Ok(new {Status=0, Message = "You are already register go to login" });
            
            // here we will register the user.
            var registerUser = await _userService.RegisterUser(ApplicationUserDetail);

            //  if user is register successfully or not then take decision accordingly.
            if (!registerUser) return StatusCode(StatusCodes.Status500InternalServerError);
            return Ok(new { Status=1, Message = "Register successfully!!!" });

        }
        [Route("RefreshToken")]
        [HttpPost]
        public async Task<IActionResult> RefreshToken(UserToken userToken)
        {
            // here check usertoken is null or in proper format.
            if (userToken == null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            // here we get claims from expired token.
            var claimUserDataFromToken = _jwtManager.GetClaimsFromExpiredToken(userToken.Token);
            if (claimUserDataFromToken == null)
            {
                return BadRequest(new { Status = 1, Message = "Token not expire" });
            }
            var claimUserIdentity = claimUserDataFromToken.Identity as ClaimsIdentity;
            var claimUser = claimUserIdentity?.FindFirst(ClaimTypes.Name)??null;
            if (claimUser == null)
            {
                return Unauthorized();
            }

            // here we will check the user name in token claims is present in owr database
            var checkUserInDb = await _userService.CheckUserInDb(claimUser.Value);
            if(checkUserInDb == null) { return BadRequest(); }

            // here check user refresh token is same as refresh token stored in database.
            if (checkUserInDb.RefreshToken != userToken.RefreshToken) return Unauthorized(new { Message = "Go to login!!!!!!" });
            
            // here check refresh token is expire or not if expire then send him to login page.
            if (checkUserInDb.RefreshTokenValidDate < DateTime.Now) return BadRequest(new { Message = "Go to login page to generate new refresh token " });
            
            // here we will generate the access token but not regenerate the refresh token.
            var generateNewToken = _jwtManager.GenerateToken(checkUserInDb, false);

            // here we will send the usertoken to the user.
            UserToken usertoken = new UserToken()
            {
                Token = generateNewToken?.Token??"null",
                RefreshToken = generateNewToken?.RefreshToken ?? "null",
            };
            return Ok(usertoken);

        }
    }
}
