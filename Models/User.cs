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

        public ICollection<UserMaterialPreference> MaterialPreferences { get; set; } = new List<UserMaterialPreference>();
        public ICollection<UserPreferredColour> ColourPreferences { get; set; } = new List<UserPreferredColour>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow ;
   }
}
