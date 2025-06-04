using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Models.Enums_queue.Models;
using QueueApp.Data;
using QueueApp.Server.Hubs;
using QueueApp.Shared.Models;       // QueueLink, QueueItem, QueueStatus

namespace QueueApp.Controllers;

[ApiController]
[Route("api/admin")]
public class AdminApiController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IHubContext<QueueHub> _hub;

    public AdminApiController(AppDbContext db,
                              IHubContext<QueueHub> hub)   // ⬅ інʼєкція хаба
    {
        _db = db;
        _hub = hub;
    }

    /* ---------- GET /api/admin/queues ---------- */
    [HttpGet("queues")]
    public async Task<IActionResult> GetAllQueues() =>
        Ok(await _db.QueueLinks
                    .OrderByDescending(q => q.CreatedAt)
                    .ToListAsync());

    /* ---------- POST /api/admin/create ---------- */
    [HttpPost("create")]
    public async Task<IActionResult> CreateQueue([FromBody] QueueCreateModel request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            return BadRequest("Назва обов’язкова.");

        var queue = new QueueLink
        {
            Title = request.Title,
            UniqueCode = Guid.NewGuid().ToString("N")[..8],
            AllowMultipleEntries = request.AllowMultipleEntries,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _db.QueueLinks.Add(queue);
        await _db.SaveChangesAsync();
        return Ok(queue);
    }

    /* ---------- GET /api/admin/queue/{id} ---------- */
    [HttpGet("queue/{id:int}")]
    public async Task<IActionResult> GetQueue(int id)
    {
        var queue = await _db.QueueLinks.FindAsync(id);
        return queue is null ? NotFound() : Ok(queue);
    }

    /* ---------- GET /api/admin/queue/{id}/items ---------- */
    [HttpGet("queue/{id:int}/items")]
    public async Task<IActionResult> GetQueueItems(int id)
    {
        var items = await _db.QueueItems
                             .Where(i => i.QueueLinkId == id)
                             .Include(i => i.User)
                             .OrderBy(i => i.EnqueuedAt)
                             .Select(i => new                      // DTO без циклів
                             {
                                 i.Id,
                                 i.Status,
                                 i.EnqueuedAt,
                                 User = new { i.UserId, i.User.Name, i.User.PhoneNumber }
                             })
                             .ToListAsync();
        return Ok(items);
    }

    /* ---------- PUT /api/admin/queue/item/{id}/status ---------- */
    [HttpPut("queue/item/{id:int}/status")]
    public async Task<IActionResult> UpdateQueueItemStatus(
        int id, [FromBody] StatusUpdateRequest request)
    {
        var item = await _db.QueueItems
                            .Include(i => i.QueueLink)
                            .FirstOrDefaultAsync(i => i.Id == id);
        if (item is null) return NotFound("Запис не знайдено.");

        item.Status = request.Status;
        await _db.SaveChangesAsync();

        // сповіщаємо всіх клієнтів конкретної черги
        await _hub.Clients
                  .Group(item.QueueLink.UniqueCode)  // ⬅ такий самий код підписки
                  .SendAsync("QueueUpdated");

        return Ok(new { item.Id, item.Status });
    }

    public class StatusUpdateRequest
    {
        public QueueStatus Status { get; set; }
    }
}
