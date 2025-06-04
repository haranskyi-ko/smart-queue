namespace QueueApp.Controllers
{
    using global::Models.Enums_queue.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using QueueApp.Data;
    using QueueApp.Shared.Models;

    [ApiController]
    [Route("api/queue")]
    public class QueueApiController : ControllerBase
    {
        private readonly AppDbContext _db;

        public QueueApiController(AppDbContext db)
        {
            _db = db;
        }

        public class RegisterRequest
        {
            public string Name { get; set; } = string.Empty;
            public string PhoneNumber { get; set; } = string.Empty;
            public string Code { get; set; } = string.Empty;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name) ||
                string.IsNullOrWhiteSpace(request.PhoneNumber) ||
                string.IsNullOrWhiteSpace(request.Code))
            {
                return BadRequest("Всі поля є обов’язковими.");
            }

            var queue = await _db.QueueLinks
                .FirstOrDefaultAsync(q => q.Id.ToString().Equals(request.Code) && q.IsActive);

            if (queue == null)
                return NotFound("Черга не знайдена або неактивна.");

            var user = await _db.Users.FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber);
            if (user == null)
            {
                user = new User
                {
                    Name = request.Name,
                    PhoneNumber = request.PhoneNumber,
                    RegistrationTime = DateTime.UtcNow
                };
                _db.Users.Add(user);
                await _db.SaveChangesAsync();
                // Reload the user to ensure Id is set
                user = await _db.Users.FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber);
            }

            if (user == null || queue == null)
            {
                return BadRequest("User or queue not found.");
            }

            var alreadyInQueue = await _db.QueueItems.AnyAsync(q =>
                q.UserId == user.Id &&
                q.QueueLinkId == queue.Id &&
                q.Status != QueueStatus.Completed &&
                q.Status != QueueStatus.Cancelled);

            if (alreadyInQueue && !queue.AllowMultipleEntries)
            {
                return Conflict("Ви вже зареєстровані в цій черзі.");
            }

            var queueItem = new QueueItem
            {
                UserId = user.Id,
                QueueLinkId = queue.Id,
                EnqueuedAt = DateTime.UtcNow,
                Status = QueueStatus.Waiting
            };
            _db.QueueItems.Add(queueItem);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "Успішно зареєстровано",
                queueItemId = queueItem.Id,
                userId = user.Id
            });
        }
    }
}
