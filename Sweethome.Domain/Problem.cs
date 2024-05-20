using System.ComponentModel.DataAnnotations;

namespace Sweethome.Domain
{
    public class Problem
    {
        [Key]
        public string Id { get; set; }
        public string Problems { get; set; }
        public string Description { get; set; }
        public string Status { get; set; } = "Новая";
        public DateTime DateOfsolution { get; set; }
        public User User { get; set; }
        public string UserId { get; set; }
    }
}
