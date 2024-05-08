using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StorageHub.Api.Dto;
using StorageHub.ServiceLibrary.Entities;
using StorageHub.ServiceLibrary.Repositories;
using StorageHub.ServiceLibrary.Service;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace StorageHub.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<UserEntity> _signInManager;

        public UserController(UserManager<UserEntity> userManager, ITokenService tokenService, SignInManager<UserEntity> signInManager)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userManager.GetUsersInRoleAsync("User");

            return Ok(users);
        }

        [HttpGet("{userId}")]
        [Authorize]
        public async Task<IActionResult> GetUser([FromRoute] string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound("User was not found.");
            return Ok(user);
        }

        [HttpPost("register")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto userCreateDto)
        {
            try
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == userCreateDto.Email.ToLower());

                if (user != null)
                {
                    return BadRequest("This email is already taken");
                }

                var userName = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == userCreateDto.UserName.ToLower());

                if (userName != null)
                {
                    return BadRequest("This username is already taken");
                }

                if (!IsStrongPassword(userCreateDto.Password))
                {
                    return BadRequest("Password needs to be at least 12 characters long, and Include 1 upper-, 1 lowercase letter and a number");
                }

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var appUser = new UserEntity
                {
                    UserName = userCreateDto.UserName,
                    Email = userCreateDto.Email
                };

                var createdUser = await _userManager.CreateAsync(appUser, userCreateDto.Password);

                if (createdUser.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(appUser, "User");
                    if(roleResult.Succeeded)
                    {
                        return Ok(
                            new NewUserDto
                            {
                                Id = appUser.Id,
                                UserName = appUser.UserName,
                                Email = appUser.Email,
                                Token = _tokenService.CreateToken(appUser)
                            }
                          );
                    }
                    else
                    {
                        return StatusCode(500, roleResult.Errors);
                    }
                }
                else
                {
                    return StatusCode(500, createdUser.Errors);
                }
            }
            catch (Exception e)
            {

                return StatusCode(500, e);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.UserName.ToLower());

            if (user == null)
                return Unauthorized("Invalid username.");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
                return Unauthorized("Username not found and/or password incorrect.");

            return Ok(
                new NewUserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Token = _tokenService.CreateToken(user)
                }
            );

        }

        [HttpPost("register-admin")]
        public async Task<IActionResult> CreateAdmin([FromBody] CreateUserDto userCreateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var appUser = new UserEntity
                {
                    UserName = userCreateDto.UserName,
                    Email = userCreateDto.Email
                };

                var createdUser = await _userManager.CreateAsync(appUser, userCreateDto.Password);

                if (createdUser.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(appUser, "Admin");
                    if (roleResult.Succeeded)
                    {
                        return Ok(
                            new NewUserDto
                            {
                                Id = appUser.Id,
                                UserName = appUser.UserName,
                                Email = appUser.Email,
                                Token = _tokenService.CreateToken(appUser)
                            }
                          );
                    }
                    else
                    {
                        return StatusCode(500, roleResult.Errors);
                    }
                }
                else
                {
                    return StatusCode(500, createdUser.Errors);
                }
            }
            catch (Exception e)
            {

                return StatusCode(500, e);
            }
        }

        [HttpPost("login-admin")]
        public async Task<IActionResult> LoginAdmin(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.UserName.ToLower());

            if (user == null)
                return Unauthorized("Invalid username.");

            if (!await _userManager.IsInRoleAsync(user, "Admin"))
                return Unauthorized("User is not an admin");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
                return Unauthorized("Username not found and/or password incorrect.");

            return Ok(
                new NewUserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Token = _tokenService.CreateToken(user)
                }
            );

        }

        [HttpPut("update-user")]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto updateUser)
        {
            var user = await _userManager.FindByEmailAsync(updateUser.Email);

            if(user == null)
            {
                return NotFound("User not found");
            }

            if (user.UserName != updateUser.UserName)
            {
                var setUsernameResult = await _userManager.SetUserNameAsync(user, updateUser.UserName);
                

                if (!setUsernameResult.Succeeded)
                {
                    return BadRequest("Something went wrong while updating username.");
                }
            }

            return Ok("User updated successfully");
        }

        [HttpPut("update-password")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordDto updatePassword)
        {
            var user = await _userManager.FindByEmailAsync(updatePassword.Email);

            if(user == null)
            {
                return NotFound("User not found");
            }

            var passwordCheck = await _signInManager.CheckPasswordSignInAsync(user, updatePassword.Password, false);

            if (!passwordCheck.Succeeded)
            {
                return BadRequest("Entered wrong password.");
            }

            string token = await _userManager.GeneratePasswordResetTokenAsync(user);

            await _userManager.ResetPasswordAsync(user, token, updatePassword.NewPassword);


            return Ok("Password reset");
        }

        [HttpDelete("{userId}/delete")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            var deleteResult = await _userManager.DeleteAsync(user);

            if (!deleteResult.Succeeded)
            {
                return BadRequest(deleteResult.Errors);
            }

            return Ok("User deleted successfully");
        }

        static bool IsStrongPassword(string password)
        {
            string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{12,}$";

            return Regex.IsMatch(password, pattern);
        }

        [HttpPost("search")]
        [Authorize]
        public async Task<IActionResult> SearchStorages([FromBody] SearchStoragesDto search)
        {
            if (search.Query == null)
            {
                return BadRequest("Search is empty");
            }

            var query = search.Query.ToLower();

            var result = await _userManager.Users
                .Where(u => u.UserName.Contains(query) || u.Email.Contains(query))
                .ToListAsync();

            return Ok(result);

        }

    }
}
