namespace AdminCompanyEmpManagementSystem
{
    public class AppSettingJwt
    {
        public string SecretKey { get; set; }
        public int TokenValidityInMinutes { get; set; }
        public int RefreshTokenExpireDays { get; set; }
    }
}
