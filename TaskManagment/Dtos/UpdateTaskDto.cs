using System;
using System.ComponentModel.DataAnnotations;

namespace TaskManagment.Dtos
{
	public class UpdateTaskDto
	{
        [Required, StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        public bool Completed { get; set; }
    }
}

