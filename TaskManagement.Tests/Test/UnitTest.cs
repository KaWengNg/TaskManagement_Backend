using Microsoft.EntityFrameworkCore;
using AutoMapper;
using TaskManagment.Data;
using TaskManagment.Mapping;
using TaskManagment.Services;
using TaskManagment.Dtos;

namespace TaskManagement.Tests.Test
{
    public class UnitTest
    {
        private readonly IMapper _mapper;
        private readonly DbContextOptions<TasksDbContext> _dbOptions;

        public UnitTest()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<TaskMapping>());
            _mapper = config.CreateMapper();

            // changed to testing DB instead
            _dbOptions = new DbContextOptionsBuilder<TasksDbContext>()
                .UseSqlite("Data Source=tasks_test.db")
                .Options;
        }

        private TaskService CreateService(TasksDbContext context)
        {
            return new TaskService(context, _mapper);
        }

        [Fact]
        public async Task CreateAsync_Should_Create_Task_Successfully()
        {
            using var context = new TasksDbContext(_dbOptions);

            // Recreate the testing db
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            var service = CreateService(context);

            var dto = new CreateTaskDto { Title = "", Description = "" };
            var result = await service.CreateAsync(dto);

            Assert.NotNull(result.Data);

            //Result: Successful created when title is empty in service layer. Remark: Need to enhance
        }
    }
}
