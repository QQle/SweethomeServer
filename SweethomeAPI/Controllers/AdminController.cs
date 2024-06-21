using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sweethome.Domain;
using SweetHome.DAL;
using SweetHome.DAL.Interfaces;

namespace SweethomeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly UserManager<User> _userManager;
        public const string ADMINISTRATOR = "Administrator";
        public AdminController( AppDbContext appDbContext , UserManager<User> userManager)
        {
            _appDbContext = appDbContext;
            _userManager = userManager;
        }
        private async Task<bool> IsCurrentUserInRole(string role)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return false;
            }

            return await _userManager.IsInRoleAsync(user, role);
        }
        public record ChangeStatusRequest(string UserId, string ProblemId, string NewStatus);

        [HttpPatch("ChangeStatus")]
        public async Task<IActionResult> ChangeStatus([FromBody] ChangeStatusRequest request)
        {
            if (!await IsCurrentUserInRole(ADMINISTRATOR))
            {
                return  BadRequest("У вас нет доступа для этой операции.");
            }

            var problem = await _appDbContext.Problems
                .FirstOrDefaultAsync(p => p.Id == request.ProblemId && p.UserId == request.UserId);

            if (problem == null)
            {
                return NotFound("Заявка не найдена.");
            }

            problem.Status = request.NewStatus;
            await _appDbContext.SaveChangesAsync();

            return Ok("Статус заявки успешно обновлен.");
        }
        [HttpGet("GetAllProblems")]
        public async Task<IActionResult> GetAllProblems()
        {
            if (!await IsCurrentUserInRole(ADMINISTRATOR))
            {
                return BadRequest("У вас нет доступа для этой операции.");
            }

            var result = await _appDbContext.Problems.ToListAsync();
                

            return Ok(result);
        }
    }
}
