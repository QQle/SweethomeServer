using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sweethome.Domain;
using SweetHome.DAL;
using SweetHome.DAL.Interfaces;
using SweetHome.DAL.Repos;

namespace SweethomeAPI.Controllers
{
    [Route("Problem")]
    public class ProblemController : ControllerBase
    {


        private readonly IBaseRepository<Problem> _baseRepository;

        public ProblemController(IBaseRepository<Problem> baseRepository)
        {
            _baseRepository = baseRepository;
        }



        public record CreateProblem(string Problem, string Description, DateTime DateOfSolution, string UserId);
        [HttpPost("createproblem")]
        public async Task<IActionResult> CreateProblemByUserId([FromBody] CreateProblem createProblem)
        {
            var problem = new Problem()
            {
                Problems = createProblem.Problem,
                Description = createProblem.Description,
                DateOfsolution = createProblem.DateOfSolution,
                UserId = createProblem.UserId



            };

            var result = await _baseRepository
                .CreateAsync(problem);

            if (result != null)
            {

                return Ok(new { userProblem = result.ToString() });
            }

            return Unauthorized();
        }
    }
}
