using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PainterPalApi.DTOs;
using PainterPalApi.Interfaces;
using PainterPalApi.Models;

namespace PainterPalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Basic authorization for all endpoints
    public class QuotesController : ControllerBase
    {
        private readonly IQuoteService _quoteService;

        public QuotesController(IQuoteService quoteService)
        {
            _quoteService = quoteService;
        }

        // GET: api/Quotes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<QuoteDTO>>> GetQuotes([FromQuery] QuoteStatus? status = null)
        {
            var quotes = await _quoteService.GetQuotesAsync(status);
            return Ok(quotes);
        }

        // GET: api/Quotes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<QuoteDTO>> GetQuote(int id)
        {
            var quote = await _quoteService.GetQuoteByIdAsync(id);
            if (quote == null)
            {
                return NotFound();
            }
            return Ok(quote);
        }

        // POST: api/Quotes
        [HttpPost]
        [Authorize(Policy = "BusinessOwnerPolicy")] // Only business owners can create quotes
        public async Task<ActionResult<QuoteDTO>> CreateQuote(QuoteDTO quoteDto)
        {
            var createdQuote = await _quoteService.CreateQuoteAsync(quoteDto);
            return CreatedAtAction(nameof(GetQuote), new { id = createdQuote.Id }, createdQuote);
        }

        // PUT: api/Quotes/5
        [HttpPut("{id}")]
        [Authorize(Policy = "BusinessOwnerPolicy")] // Only business owners can update quotes
        public async Task<IActionResult> UpdateQuote(int id, QuoteDTO quoteDto)
        {
            if (id != quoteDto.Id)
            {
                return BadRequest("ID mismatch");
            }

            var updatedQuote = await _quoteService.UpdateQuoteAsync(id, quoteDto);
            if (updatedQuote == null)
            {
                return NotFound();
            }
            return NoContent();
        }

        // PUT: api/Quotes/5/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateQuoteStatus(int id, [FromBody] QuoteStatusUpdateModel model)
        {
            var quote = await _quoteService.GetQuoteByIdAsync(id);
            if (quote == null)
            {
                return NotFound();
            }

            // Update the status using the service
            quote.QuoteStatus = model.Status;
            var updatedQuote = await _quoteService.UpdateQuoteAsync(id, quote);

            if (updatedQuote == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: api/Quotes/5/convert
        [HttpPost("{id}/convert")]
        [Authorize(Policy = "BusinessOwnerPolicy")] // Only business owners can convert quotes to jobs
        public async Task<ActionResult<JobDTO>> ConvertQuoteToJob(int id)
        {
            var job = await _quoteService.ConvertQuoteToJobAsync(id);
            if (job == null)
            {
                return NotFound();
            }
            return CreatedAtAction("GetJob", "Jobs", new { id = job.Id }, job);
        }

        // DELETE: api/Quotes/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "BusinessOwnerPolicy")] // Only business owners can delete quotes
        public async Task<IActionResult> DeleteQuote(int id)
        {
            var result = await _quoteService.DeleteQuoteAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }

    public class QuoteStatusUpdateModel
    {
        public QuoteStatus Status { get; set; }
    }
}

