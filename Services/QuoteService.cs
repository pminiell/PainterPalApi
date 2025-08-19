using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PainterPalApi.Data;
using PainterPalApi.DTOs;
using PainterPalApi.Interfaces;
using PainterPalApi.Models;

namespace PainterPalApi.Services
{
    public class QuoteService : IQuoteService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public QuoteService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<QuoteDTO>> GetQuotesAsync(QuoteStatus? status = null)
        {
            var query = _context.Quotes.AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(q => q.QuoteStatus == status.Value);
            }

            var quotes = await query
                .Include(q => q.Customer)
                .Include(q => q.PrefferredColours)
                .ToListAsync();

            return _mapper.Map<IEnumerable<QuoteDTO>>(quotes);
        }

        public async Task<QuoteDTO> GetQuoteByIdAsync(int id)
        {
            var quote = await _context.Quotes
                .Include(q => q.Customer)
                .Include(q => q.PrefferredColours)
                .FirstOrDefaultAsync(q => q.Id == id);

            return _mapper.Map<QuoteDTO>(quote);
        }

        public async Task<QuoteDTO> CreateQuoteAsync(QuoteDTO quoteDto)
        {
            var quote = _mapper.Map<Quote>(quoteDto);
            _context.Quotes.Add(quote);
            await _context.SaveChangesAsync();
            return _mapper.Map<QuoteDTO>(quote);
        }

        public async Task<QuoteDTO> UpdateQuoteAsync(int id, QuoteDTO quoteDto)
        {
            var quote = await _context.Quotes.FindAsync(id);
            if (quote == null)
            {
                return null;
            }

            _mapper.Map(quoteDto, quote);
            await _context.SaveChangesAsync();
            return _mapper.Map<QuoteDTO>(quote);
        }

        public async Task<bool> DeleteQuoteAsync(int id)
        {
            var quote = await _context.Quotes.FindAsync(id);
            if (quote == null)
            {
                return false;
            }

            _context.Quotes.Remove(quote);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<JobDTO> ConvertQuoteToJobAsync(int id)
        {
            var quote = await _context.Quotes.FindAsync(id);
            if (quote == null)
            {
                return null;
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

            return _mapper.Map<JobDTO>(job);
        }
    }
}
