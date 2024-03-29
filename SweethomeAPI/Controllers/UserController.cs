﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SweetHome.DAL.Interfaces;
using Sweethome.Domain;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using SweetHome.DAL;

namespace SweethomeAPI.Controllers;

[Route("user")]
public class UserController: ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IBaseRepository<User> _userRepository;
    private readonly AppDbContext  _appDbContext;

    public UserController(IBaseRepository<User> userRepository, SignInManager<User> signInManager, UserManager<User> userManager, AppDbContext appDbContext)
    {
        _userRepository = userRepository;
        _signInManager = signInManager;
        _userManager = userManager;
        _appDbContext = appDbContext;
    }

    //[Authorize]
    [HttpGet("getAllProblemsByUserId")]
    public async Task<IActionResult> GetProblemsByUserId(string userId)
    {

        var users = await _appDbContext.Users
            .Where(x => x.Id == $"{userId}")
            .Select(x => x.Problem)
            .FirstOrDefaultAsync();

        return Ok(users);
    }

    public record LoginModel(string UserName, string Password);

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
    {
 
        var result = await _signInManager.PasswordSignInAsync(loginModel.UserName, loginModel.Password, true, false);


        if (result.Succeeded)
        {

            var currentUser = await _appDbContext.Users
                .FirstOrDefaultAsync(x => x.UserName == loginModel.UserName);


            if (currentUser != null)
            {
                return Ok(new { userid = currentUser.Id });
            }
        }

        return Unauthorized();
    }



    public record RegistrationModel(string UserName, string Password, string LastName, string SurName, string Email, string phoneNumber, string Address);
    [HttpPost("signup")]
    public async Task<IActionResult> Registration([FromBody] RegistrationModel registerModel)
    {
        var user = new User()
        {
            UserName = registerModel.UserName,
            LastName = registerModel.LastName,
            Surname = registerModel.SurName,
            Email = registerModel.Email,
            PhoneNumber = registerModel.phoneNumber,
            Address = registerModel.Address,
        };
        var result = await _userManager.
            CreateAsync(user, $"{registerModel.Password}");

        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(user, true);
            var currentUserId = await _appDbContext.Users
            .Where(x => x.UserName == $"{registerModel.UserName}")
            .Select(x => x.Id)
            .ToListAsync();
            return Ok(new { userId = currentUserId });
        }
        
        return Unauthorized(result.Errors);
    }
}