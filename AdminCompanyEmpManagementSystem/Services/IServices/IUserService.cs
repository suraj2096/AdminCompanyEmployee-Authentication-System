using AdminCompanyEmpManagementSystem.Identity;

namespace AdminCompanyEmpManagementSystem.Services.IServices
{
    public interface IUserService
    {
        Task<bool> IsUnique(string userName);
        Task<ApplicationUser?> AuthenticateUser(string userName, string userPassword);
        Task<bool> RegisterUser(ApplicationUser userCredentials);
        Task<ApplicationUser?> AddOrUpdateUserRefreshToken(ApplicationUser user);
        Task<ApplicationUser?> CheckUserInDb(string userName);
    }
}
