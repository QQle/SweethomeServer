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
        public const string ADMINISTRATOR = "Admin";
        public AdminController( AppDbContext appDbContext , UserManager<User> userManager)
        {
            _appDbContext = appDbContext;
            _userManager = userManager;
        }
      
        public record ChangeStatusRequest(string UserId, string ProblemId, string NewStatus);

        [HttpPatch("ChangeStatus")]
        public async Task<IActionResult> ChangeStatus([FromBody] ChangeStatusRequest request)
        {
       

            var problem = await _appDbContext.Problem
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
         

            var result = await _appDbContext.Problem.ToListAsync();
                

            return Ok(result);
        }
    }
}
