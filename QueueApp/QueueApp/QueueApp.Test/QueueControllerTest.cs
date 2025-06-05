using Xunit;
using Microsoft.EntityFrameworkCore;
using QueueApp.Controllers;
using QueueApp.Data;
using QueueApp.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace QueueApp.Tests.Controllers
{
    public class QueueControllerTests
    {
        private readonly AppDbContext _dbContext;
        private readonly QueueController _controller;

        public QueueControllerTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options;

            _dbContext = new AppDbContext(options);

            _dbContext.QueueLinks.Add(new QueueLink
            {
                Title = "Test Queue",
                UniqueCode = Guid.NewGuid().ToString("N").Substring(0, 8),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });
            _dbContext.SaveChanges();

            _controller = new QueueController(_dbContext);
        }

        [Fact]
        public async Task GetAllQueues_ReturnsActiveQueues()
        {
            var result = await _controller.GetAllQueues();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var queues = Assert.IsType<List<QueueLink>>(okResult.Value);
            Assert.Single(queues);
        }

        [Fact]
        public async Task GetQueueByCode_ReturnsCorrectQueue()
        {
            var queue = _dbContext.QueueLinks.First();
            var result = await _controller.GetQueueByCode(queue.Id.ToString());
            var okResult = Assert.IsType<OkObjectResult>(result);
            var foundQueue = Assert.IsType<QueueLink>(okResult.Value);
            Assert.Equal(queue.Id, foundQueue.Id);
        }

        [Fact]
        public async Task GetQueueItems_ReturnsQueueItems()
        {
            var queue = _dbContext.QueueLinks.First();
            var user = new User { Name = "Test User", PhoneNumber = "1234567890" };
            _dbContext.Users.Add(user);
            _dbContext.QueueItems.Add(new QueueItem
            {
                QueueLinkId = queue.Id,
                User = user,
                EnqueuedAt = DateTime.UtcNow,
                Status = Models.Enums_queue.Models.QueueStatus.Waiting
            });
            _dbContext.SaveChanges();

            var result = await _controller.GetQueueItems(queue.Id.ToString());
            var okResult = Assert.IsType<OkObjectResult>(result);
            var items = Assert.IsType<List<QueueItem>>(okResult.Value);
            Assert.Single(items);
            Assert.Equal(user.Name, items[0].User.Name);
        }
    }
}