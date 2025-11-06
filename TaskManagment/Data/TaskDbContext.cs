using Microsoft.EntityFrameworkCore;
using TaskManagment.Models;

namespace TaskManagment.Data
{
    public class TasksDbContext : DbContext
    {
        public TasksDbContext(DbContextOptions<TasksDbContext> options) : base(options) { }

        public DbSet<TaskItem> Tasks => Set<TaskItem>();
    }
}

