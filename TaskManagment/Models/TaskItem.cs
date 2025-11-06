using System.ComponentModel.DataAnnotations;
namespace TaskManagment.Models
{
	public class TaskItem
	{
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        public bool Completed { get; set; } = false;

        public DateTime CreatedAt { get; set; } 
        public DateTime UpdatedAt { get; set; } 
    }
}

