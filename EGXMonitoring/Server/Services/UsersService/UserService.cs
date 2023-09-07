using EGXMonitoring.Shared.DTOS;
using Microsoft.EntityFrameworkCore;

namespace EGXMonitoring.Server.Services.UsersService
{
    public class UserService : IUserService
    {
        private readonly DataContext _context;

        public UserService(DataContext context)
        {
            _context = context;
        }
        public async Task<ServiceResponse<List<User>>> GetUsers()
        {
            try
            {
                var users = await _context.Users.ToListAsync();
                var result = new ServiceResponse<List<User>>();
                result.Data = users;
                return result;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<User>>() { Data = null, Success = false, Message = ex.Message };

            }
        }

        public async Task<ServiceResponse<User>> AddUser(User user)
        {
            try
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                return new ServiceResponse<User>()
                {
                    Data = user,
                    Success = true,
                    Message = "User added"
                };
            }
            catch (Exception ex) 
            {
                return new ServiceResponse<User>() { Data = null, Success = false, Message = ex.Message };
            }
        }

        public async Task<ServiceResponse<User>> DeleteUser(User user)
        {
            try
            {
                user.ISACTIVE = 0;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return new ServiceResponse<User>()
                {
                    Data = user,
                    Success = true,
                    Message = "User added"
                };
            }
            catch (Exception ex) {
                return new ServiceResponse<User>() { Data = null, Success = false, Message = ex.Message };
            }
        }

        public async Task<ServiceResponse<User>> UpdateUser(User user)
        {
            try
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return new ServiceResponse<User>()
                {
                    Data = user,
                    Success = true,
                    Message = "User added"
                };
            }
            catch (Exception ex) {
                return new ServiceResponse<User>() { Data = null, Success = false, Message = ex.Message };
            }
        }
    }
}
