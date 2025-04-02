using System.ComponentModel.DataAnnotations;

namespace PainterPalApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string Role { get; set; }
        public ICollection<JobEmployee> JobAssignments { get; set; } = new List<JobEmployee>();
    
    }
}
