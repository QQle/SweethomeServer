using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sweethome.Domain;
using SweetHome.DAL.Interfaces;

namespace SweethomeAPI.Controllers
{
    [Route("Problem")]
    public class ProblemController : ControllerBase
    {


        private readonly IBaseRepository<Problem> _baseRepository;
        private readonly ILogger<ProblemController> _logger;

        public ProblemController(IBaseRepository<Problem> baseRepository, ILogger<ProblemController> logger)
        {
            _baseRepository = baseRepository;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }



        public record CreateProblem(string Problem, string Description, DateTime DateOfSolution, string UserId);
        [HttpPost("createproblem")]
        public async Task<IActionResult> CreateProblemByUserId([FromBody] CreateProblem createProblem)
        {
            if (string.IsNullOrEmpty(createProblem.UserId))
            {
                _logger.LogWarning("UserId is null or empty.");
                return BadRequest("UserId is null or empty.");
            }

            if (createProblem == null)
            {
                _logger.LogWarning("CreateProblem request body is null.");
                return BadRequest("Request body is null.");
            }

           
            var problem = new Problem()
            {
                Problems = createProblem.Problem,
                Description = createProblem.Description,
                DateOfsolution = createProblem.DateOfSolution,
                UserId = createProblem.UserId,
            };


            try
            {
                var result = await _baseRepository.CreateAsync(problem);

                if (result != null)
                {
                    return Ok(new { userProblem = result.ToString() });
                }

                return Unauthorized();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a problem.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
