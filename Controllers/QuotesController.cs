using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PainterPalApi.Data;
using PainterPalApi.Models;
namespace PainterPalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Basic authorization for all endpoints
    public class QuotesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public QuotesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Quotes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Quote>>> GetQuotes([FromQuery] QuoteStatus? status = null)
        {
            var query = _context.Quotes
                .Include(q => q.Customer)
                .AsQueryable();

            // Filter by status if provided
            if (status.HasValue)
            {
                query = query.Where(q => q.QuoteStatus == status.Value);
            }

            return await query.ToListAsync();
        }

        // GET: api/Quotes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Quote>> GetQuote(int id)
        {
            var quote = await _context.Quotes
                .Include(q => q.Customer)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (quote == null)
            {
                return NotFound();
            }

            return quote;
        }

        // GET: api/Quotes/customer/5
        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<IEnumerable<Quote>>> GetCustomerQuotes(int customerId)
        {
            var customerQuotes = await _context.Quotes
                .Where(q => q.CustomerId == customerId)
                .ToListAsync();

            return customerQuotes;
        }

        // POST: api/Quotes
        [HttpPost]
        [Authorize(Policy = "BusinessOwnerPolicy")] // Only business owners can create quotes
        public async Task<ActionResult<Quote>> CreateQuote(Quote quote)
        {
            // Validate customer exists
            if (!await _context.Customers.AnyAsync(c => c.Id == quote.CustomerId))
            {
                return BadRequest("Customer does not exist");
            }

            // Set default status to Pending if not specified
            if (quote.QuoteStatus == 0) // Default enum value
            {
                quote.QuoteStatus = QuoteStatus.Pending;
            }

            _context.Quotes.Add(quote);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetQuote), new { id = quote.Id }, quote);
        }

        // PUT: api/Quotes/5
        [HttpPut("{id}")]
        [Authorize(Policy = "BusinessOwnerPolicy")] // Only business owners can update quotes
        public async Task<IActionResult> UpdateQuote(int id, Quote quote)
        {
            if (id != quote.Id)
            {
                return BadRequest();
            }

            _context.Entry(quote).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuoteExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // PUT: api/Quotes/5/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateQuoteStatus(int id, [FromBody] QuoteStatusUpdateModel model)
        {
            var quote = await _context.Quotes.FindAsync(id);
            if (quote == null)
            {
                return NotFound();
            }

            // Validate the status is a valid enum value
            if (!Enum.IsDefined(typeof(QuoteStatus), model.Status))
            {
                return BadRequest("Invalid status value");
            }

            quote.QuoteStatus = model.Status;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Quotes/5/convert
        [HttpPost("{id}/convert")]
        [Authorize(Policy = "BusinessOwnerPolicy")] // Only business owners can convert quotes to jobs
        public async Task<IActionResult> ConvertQuoteToJob(int id)
        {
            var quote = await _context.Quotes.FindAsync(id);
            if (quote == null)
            {
                return NotFound();
            }

            var job = new Job
            {
                CustomerId = quote.CustomerId,
                JobNotes = quote.QuoteNotes,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(7), // Example duration
                JobStatus = JobStatus.Pending
            };

            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(JobsController.GetJob), "Jobs", new { id = job.Id }, job);
        }

        private bool QuoteExists(int id)
        {
            return _context.Quotes.Any(e => e.Id == id);
        }
    }

    public class QuoteStatusUpdateModel
    {
        public QuoteStatus Status { get; set; }
    }
}

