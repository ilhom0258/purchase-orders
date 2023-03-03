using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OberMind.PurchaseOrders.Application.DTOs;
using OberMind.PurchaseOrders.Domain.Entities;
using OberMind.PurchaseOrders.Infrastructure.DbContexts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OberMind.PurchaseOrders.Application.Services
{
    public interface IUserService
    {
        string Login(LoginDTO user);
        Task<UserDTO> Register(RegisterUserDTO user);
    }

    public class UserService : IUserService
    {
        private readonly IConfiguration _config;
        private readonly DataContext _context;

        public UserService(IConfiguration config, DataContext context)
        {
            _config = config;
            _context = context;
        }

        public string Login(LoginDTO loginUser)
        {
            var user = _context.Users.FirstOrDefault(u => u.Login == loginUser.Login);

            if (user != null && user.Password == loginUser.Password)
            {
                var authClaims = new[]
                {
                    new Claim(ClaimTypes.Name, user.Login),
                    new Claim("UserId", user.Id.ToString())
                };

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Secret"]));

                var token = new JwtSecurityToken(
                    issuer: _config["JWT:ValidIssuer"],
                    audience: _config["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }

            return null;
        }

        public async Task<UserDTO> Register(RegisterUserDTO registerUserDto) 
        {
            if (await _context.Users.AnyAsync(u => u.Login == registerUserDto.Login))
            {
                throw new Exception("User already exists");
            }

            var newUser = new User
            {
                Id = Guid.NewGuid(),
                Login = registerUserDto.Login,
                Name = registerUserDto.Name,
                Password = registerUserDto.Password
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return UserDTO.ToUserDTO(newUser);
        }
    }
}
