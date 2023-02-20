using AdminCompanyEmpManagementSystem.Identity;
using AdminCompanyEmpManagementSystem.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Text;

namespace AdminCompanyEmpManagementSystem.Services
{
    public class UserService : IUserService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManagaer;
        private readonly IJwtManager _jwtManager;
        private readonly AppSettingJwt _appSettingJwt;

        public UserService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager,IJwtManager jwtManager,IOptions<AppSettingJwt> appSettingJwt)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtManager = jwtManager;
            _appSettingJwt = appSettingJwt.Value;
            _roleManagaer = roleManager;
        }


        #region Here we add or update Refresh Token in Database
        public async Task<ApplicationUser?> AddOrUpdateUserRefreshToken(ApplicationUser user)
        {
            user.RefreshTokenValidDate = DateTime.Now.AddDays(_appSettingJwt.RefreshTokenExpireDays);
            var userDetail = await _userManager.UpdateAsync(user);
            return userDetail.Succeeded?user:null;
        }
        #endregion
        #region Authenticate or check the user whether he exist or not in database
        public async Task<ApplicationUser?> AuthenticateUser(string userName, string userPassword)
        {
            // here we first check user exist in the database or not if not then not go user further.
            var userExist = await _userManager.FindByNameAsync(userName);

            // here we verify that user is genuine or not 
            var userVerification = await _signInManager.CheckPasswordSignInAsync(userExist, userPassword, false);
            if (!userVerification.Succeeded) return null;

            // here get the role of the login user.
            var roleUser = await _userManager.GetRolesAsync(userExist);
            userExist.Role = roleUser.FirstOrDefault();

            // here we will check that user refresh token expire or not if expire than take decision accordingly.
            if (userExist.RefreshTokenValidDate < DateTime.Now)
            {
                var userTokenGenerated = _jwtManager.GenerateToken(userExist, true);
                // check the user refresh token or  add the user refresh token or update the token.
                return await AddOrUpdateUserRefreshToken(userTokenGenerated);
            }
            return _jwtManager.GenerateToken(userExist, false);
        }
        #endregion
        #region simple method to check unique user in database
        public async Task<bool> IsUnique(string userName)
        {
            var userExist = await _userManager.FindByNameAsync(userName);
            if (userExist == null) return true;
            return false;
        }
        #endregion
        
        #region  Register the user here and also check here admin is already register or not means how much admin we create here we define.
        public async Task<bool> RegisterUser(ApplicationUser userCredentials)
        {
            // create the user here 
            // first check the role he gave is exist in the database or not
            if (await _roleManagaer.FindByNameAsync(userCredentials.Role) == null) return false;

           /* if (userCredentials.Role == SD.Role_Admin)
            {
                var CheckAdmin = await _userManager.GetUsersInRoleAsync(SD.Role_Admin);
                if (CheckAdmin.Count == 1) return false;
            }*/

            var user = await _userManager.CreateAsync(userCredentials, userCredentials.PasswordHash);
            if (!user.Succeeded) return false;
            // here assign the role to the user
            await _userManager.AddToRoleAsync(userCredentials, userCredentials.Role);
            return true;
        }
        #endregion

        #region we find the user in database and their role
        public async Task<ApplicationUser?> CheckUserInDb(string userName)
        {
            // here we find the user in database.
            var checkUserInDb = await _userManager.FindByIdAsync(userName);
            if (checkUserInDb == null) return null;

            // here we find the role of the user in application through database
            var userGetRole = await _userManager.GetRolesAsync(checkUserInDb);
            checkUserInDb.Role = userGetRole?.FirstOrDefault();
            return checkUserInDb;
        }
        #endregion
        #region this method generate password of length 10 
        public string? GeneratePassword()
        {
            int length = 10; 
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789@"; // Add any additional characters that you want to include in the random text
            StringBuilder Password = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                Password.Append(chars[random.Next(chars.Length)]);
            }
            return Password.ToString();
        }
        #endregion
    }
}
