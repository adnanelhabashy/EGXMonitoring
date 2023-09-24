using EGXMonitoring.Server.Data;
using EGXMonitoring.Shared;
using EGXMonitoring.Shared.DTOS;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace EGXMonitoring.Server.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        public AuthService(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<ServiceResponse<string>> Login(string Username, string password)
        {
            var response = new ServiceResponse<string>();
            var user = await _context.Users.FirstOrDefaultAsync(x => x.USERNAME.ToLower() == Username.ToLower());
            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found";
            }
            else if (!VerifyPasswordHash(password, user.PASSWORDHASH, user.PASSWORDSALT))
            {
                response.Success = false;
                response.Message = "wrong password";
            }
            else if(user.ISACTIVE != 1)
            {
                response.Success = false;
                response.Message = "User Not Active please refer to system admin to activate.";
            }
            else
            {
                response.Success = true;
                response.Data = CreateToken(user);
            }
            return response;
        }

        public async Task<ServiceResponse<int>> Resiger(User user, string password)
        {
            if (await UserExists(user.USERNAME))
            {
                return new ServiceResponse<int>
                {
                    Success = false,
                    Message = "User Already exists"
                };
            }
            else
            {
                CreatepasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
                user.PASSWORDSALT = passwordSalt;
                user.PASSWORDHASH = passwordHash;
                user.CREATEDON = DateTime.UtcNow;
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return new ServiceResponse<int>
                {
                    Success = true,
                    Data = user.ID,
                    Message = "Registration Successful"
                };
            }
        }

        public async Task<bool> UserExists(string email)
        {
            if (await _context.Users.AnyAsync(x => x.USERNAME.ToLower() == email.ToLower()))
            {
                return true;
            }
            else { return false; }
        }

        private void CreatepasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier,user.ID.ToString()),
                new Claim(ClaimTypes.Name,user.USERNAME),
                new Claim(ClaimTypes.Expiration, DateTime.Now.AddDays(1).ToString())
            };

            if (user.ISADMIN == 1)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            }

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSetting:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }

        public async Task<ServiceResponse<bool>> ChangePassword(string  Uername, string newPassword)
        {
            try
            {
                var user = await _context.Users.Where(U => U.USERNAME == Uername).FirstOrDefaultAsync();
                if (user == null)
                {
                    return new ServiceResponse<bool>()
                    {
                        Data = false,
                        Success = true,
                        Message = "User not found"
                    };
                }
                if(user.RESETPASSWORD != 1)
                {
                    return new ServiceResponse<bool>()
                    {
                        Data = false,
                        Success = true,
                        Message = "No Authorized to reset password please refer to Eng. Khairy, :)"
                    };
                }
                CreatepasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);
                user.PASSWORDSALT = passwordSalt;
                user.PASSWORDHASH = passwordHash;
                user.RESETPASSWORD = 0;
                _context.Update(user);
                await _context.SaveChangesAsync();
                return new ServiceResponse<bool>()
                {
                    Data = true,
                    Success = true,
                    Message = "Password has been changed"
                };
            }
            catch(Exception ex)
            {
                return new ServiceResponse<bool>()
                {
                    Success = false,
                    Data = false,
                    Message = ex.Message
                };
            }
            
        }
    }
}
