using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController:BaseApiController
    {
        private DataContext _context;
        public AccountController(DataContext dataContext)
        {
            _context = dataContext;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if(await IfExists(registerDto.Name) == true)
            return BadRequest("User Name Exists");

            using var hmac = new HMACSHA512();
            var user = new AppUser()
            {
                UserName = registerDto.Name.ToLower(),
                PasswordHash  = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(user);
        }

        private async Task<bool> IfExists(string name)
        {
            return await _context.Users.AnyAsync(u => u.UserName == name.ToLower());
        }
    }
}