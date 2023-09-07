using EGXMonitoring.Shared;
using static System.Net.WebRequestMethods;
using System.Net.Http.Json;
namespace EGXMonitoring.Client.Services.UsersService
{
    public class UserService : IUserService
    {
        private readonly HttpClient _http;
        public UserService(HttpClient http)
        {
            _http = http;
        }

        public async Task<ServiceResponse<User>> AddUser(User user)
        {
            var result = await _http.PostAsJsonAsync("api/Users/adduser", user);
            var data = await result.Content.ReadFromJsonAsync<ServiceResponse<User>>();
            return data;
        }

        public async Task<ServiceResponse<User>> DeleteUser(User user)
        {
            var result = await _http.PostAsJsonAsync("api/Users/deleteuser", user);
            var data = await result.Content.ReadFromJsonAsync<ServiceResponse<User>>();
            return data;
        }

        public async Task<ServiceResponse<List<User>>> GetUsers()
        {
            var users = await _http.GetFromJsonAsync<ServiceResponse<List<User>>>("api/Users");
            return users;
        }

        public async Task<ServiceResponse<User>> UpdateUser(User user)
        {
            var result = await _http.PostAsJsonAsync("api/Users/updateuser", user);
            var data = await result.Content.ReadFromJsonAsync<ServiceResponse<User>>();
            return data;
        }
    }
}
