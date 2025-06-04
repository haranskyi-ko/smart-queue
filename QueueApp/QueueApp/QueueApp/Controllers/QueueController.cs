namespace QueueApp.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using QueueApp.Data;
    using QueueApp.Shared.Models;
    using System.Threading.Tasks;
    using System.Linq;

    [ApiController]
    [Route("api/queue")]
    public class QueueController : ControllerBase
    {
        private readonly AppDbContext _db;

        public QueueController(AppDbContext db)
        {
            _db = db;
        }

        // GET: api/queue
        [HttpGet]
        public async Task<IActionResult> GetAllQueues()
        {
            var queues = await _db.QueueLinks
                .Where(q => q.IsActive)
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();
            return Ok(queues);
        }

        // GET: api/queue/{code}
        [HttpGet("{code}")]
        public async Task<IActionResult> GetQueueByCode(string code)
        {
            var queue = await _db.QueueLinks.FirstOrDefaultAsync(q => q.Id.ToString().Equals(code) && q.IsActive);
            if (queue == null)
                return NotFound("Черга не знайдена або неактивна.");
            return Ok(queue);
        }

        // GET: api/queue/{code}/items
        [HttpGet("{code}/items")]
        public async Task<IActionResult> GetQueueItems(string code)
        {
            var queue = await _db.QueueLinks.FirstOrDefaultAsync(q => q.Id.ToString().Equals(code) && q.IsActive);
            if (queue == null)
                return NotFound("Черга не знайдена або неактивна.");

            var items = await _db.QueueItems
                .Where(q => q.QueueLinkId == queue.Id)
                .Include(q => q.User)
                .OrderBy(q => q.EnqueuedAt)
                .ToListAsync();

            return Ok(items);
        }
    }
}