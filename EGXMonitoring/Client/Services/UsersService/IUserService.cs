namespace EGXMonitoring.Client.Services.UsersService
{
    public interface IUserService
    {
        Task<ServiceResponse<List<User>>> GetUsers();

        Task<ServiceResponse<User>> AddUser(User user);
        Task<ServiceResponse<User>> UpdateUser(User user);
        Task<ServiceResponse<User>> DeleteUser(User user);

    }
}
