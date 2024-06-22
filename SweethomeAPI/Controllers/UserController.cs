using Microsoft.AspNetCore.Authorization;
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
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IBaseRepository<User> _userRepository;
    private readonly AppDbContext  _appDbContext;

    public UserController(IBaseRepository<User> userRepository, SignInManager<User> signInManager, UserManager<User> userManager, AppDbContext appDbContext, RoleManager<IdentityRole> roleManager)
    {
        _userRepository = userRepository;
        _signInManager = signInManager;
        _userManager = userManager;
        _appDbContext = appDbContext;
        _roleManager = roleManager;
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
        var currentUserId = await _appDbContext.Users
            .Where(x => x.UserName == $"{loginModel.UserName}")
            .Select(x => x.Id)
            .FirstOrDefaultAsync();
        var currentUser = await _userManager.FindByNameAsync(loginModel.UserName);

        if (currentUser == null)
        {
            return Unauthorized();
        }

        bool administrator = await _userManager.IsInRoleAsync(currentUser, "admin");

        var loginResult = new
        {
            userId = currentUserId,
            isAdministrator = administrator
        };

        if (result.Succeeded)
        {
            return Ok(new { loginResult.userId, loginResult.isAdministrator });
        }


        return Unauthorized();
    }


    public record RegistrationModel(string UserName, string Password, string LastName, string SurName, string Email, string phoneNumber, string Address, string Role);
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
        if (await _roleManager.RoleExistsAsync(registerModel.Role))
        {
            var result = await _userManager.
            CreateAsync(user, registerModel.Password);
            await _signInManager.SignInAsync(user, true);
            var currentUserId = await _appDbContext.Users
            .Where(x => x.UserName == registerModel.UserName)
            .Select(x => x.Id)
            .FirstOrDefaultAsync();

            if (!result.Succeeded)
            {
                return BadRequest("Ошибка создания пользователя");
            }
            await _userManager.AddToRoleAsync(user, registerModel.Role);
             
            return Ok(new { userId = currentUserId});

        }
        else
        {
            return BadRequest("Роли не существует");
        }


       
    }

   
}