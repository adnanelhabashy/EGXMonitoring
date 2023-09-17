using EGXMonitoring.Shared.DTOS;

namespace EGXMonitoring.Server.Services.AuthService
{
    public interface IAuthService
    {
        Task<ServiceResponse<int>> Resiger(User user, string password);
        Task<bool> UserExists(string Username);
        Task<ServiceResponse<string>> Login(string Username, string password);
        Task<ServiceResponse<bool>> ChangePassword(string Username, string newPassword);
    }
}
