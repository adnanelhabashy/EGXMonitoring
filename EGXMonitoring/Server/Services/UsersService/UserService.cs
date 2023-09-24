using EGXMonitoring.Shared.DTOS;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

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
                CreatepasswordHash("123456", out byte[] passwordHash, out byte[] passwordSalt);
                user.PASSWORDSALT = passwordSalt;
                user.PASSWORDHASH = passwordHash;
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

        private void CreatepasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<ServiceResponse<User>> DeleteUser(User user)
        {
            try
            {
                user.ISACTIVE = 0;
                _context.Users.Remove(user);
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
