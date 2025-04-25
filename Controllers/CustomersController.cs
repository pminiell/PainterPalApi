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
    public class CustomersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CustomersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers([FromQuery] string search = null)
        {
            var query = _context.Customers.AsQueryable();

            // Filter by search term if provided
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(c =>
                    c.CustomerName.ToLower().Contains(search) ||
                    c.CustomerEmail.ToLower().Contains(search) ||
                    c.CustomerPhone.Contains(search));
            }

            return await query.ToListAsync();
        }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return customer;
        }

        // GET: api/Customers/5/jobs
        [HttpGet("{id}/jobs")]
        public async Task<ActionResult<IEnumerable<Job>>> GetCustomerJobs(int id)
        {
            if (!await CustomerExists(id))
            {
                return NotFound("Customer not found");
            }

            var jobs = await _context.Jobs
                .Where(j => j.CustomerId == id)
                .ToListAsync();

            return jobs;
        }

        // GET: api/Customers/5/quotes
        [HttpGet("{id}/quotes")]
        public async Task<ActionResult<IEnumerable<Quote>>> GetCustomerQuotes(int id)
        {
            if (!await CustomerExists(id))
            {
                return NotFound("Customer not found");
            }

            var quotes = await _context.Quotes
                .Where(q => q.CustomerId == id)
                .ToListAsync();

            return quotes;
        }

        // POST: api/Customers
        [HttpPost]
        [Authorize(Policy = "BusinessOwnerPolicy")] // Only business owners can create customers
        public async Task<ActionResult<Customer>> CreateCustomer(Customer customer)
        {
            // Validate email is unique
            if (!string.IsNullOrEmpty(customer.CustomerEmail) &&
                await _context.Customers.AnyAsync(c => c.CustomerEmail == customer.CustomerEmail))
            {
                return BadRequest("A customer with this email already exists");
            }

            // Validate phone is unique
            if (!string.IsNullOrEmpty(customer.CustomerPhone) &&
                await _context.Customers.AnyAsync(c => c.CustomerPhone == customer.CustomerPhone))
            {
                return BadRequest("A customer with this phone number already exists");
            }

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
        }

        // PUT: api/Customers/5
        [HttpPut("{id}")]
        [Authorize(Policy = "BusinessOwnerPolicy")] // Only business owners can update customers
        public async Task<IActionResult> UpdateCustomer(int id, Customer customer)
        {
            if (id != customer.Id)
            {
                return BadRequest();
            }

            // Check for email uniqueness (excluding current customer)
            if (!string.IsNullOrEmpty(customer.CustomerEmail) &&
                await _context.Customers.AnyAsync(c =>
                    c.CustomerEmail == customer.CustomerEmail && c.Id != id))
            {
                return BadRequest("A customer with this email already exists");
            }

            // Check for phone uniqueness (excluding current customer)
            if (!string.IsNullOrEmpty(customer.CustomerPhone) &&
                await _context.Customers.AnyAsync(c =>
                    c.CustomerPhone == customer.CustomerPhone && c.Id != id))
            {
                return BadRequest("A customer with this phone number already exists");
            }

            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CustomerExists(id))
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

        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "BusinessOwnerPolicy")] // Only business owners can delete customers
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            // Check if customer has jobs
            var hasJobs = await _context.Jobs.AnyAsync(j => j.CustomerId == id);
            if (hasJobs)
            {
                return BadRequest("Cannot delete customer with associated jobs");
            }

            // Check if customer has quotes
            var hasQuotes = await _context.Quotes.AnyAsync(q => q.CustomerId == id);
            if (hasQuotes)
            {
                return BadRequest("Cannot delete customer with associated quotes");
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Helper method to check if customer exists
        private async Task<bool> CustomerExists(int id)
        {
            return await _context.Customers.AnyAsync(e => e.Id == id);
        }
    }
}
