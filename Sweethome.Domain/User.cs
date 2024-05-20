using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Sweethome.Domain
{
    public class User: IdentityUser
    {
        public string? Surname { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public bool? Subscribe { get; set; }
        public List<Problem>? Problem { get; set; }
    }
}
