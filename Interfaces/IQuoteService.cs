using PainterPalApi.DTOs;
using PainterPalApi.Models;

namespace PainterPalApi.Interfaces
{
    public interface IQuoteService
    {
        Task<IEnumerable<QuoteDTO>> GetQuotesAsync(QuoteStatus? status = null);
        Task<QuoteDTO> GetQuoteByIdAsync(int id);
        Task<QuoteDTO> CreateQuoteAsync(QuoteDTO quoteDto);
        Task<QuoteDTO> UpdateQuoteAsync(int id, QuoteDTO quoteDto);
        Task<bool> DeleteQuoteAsync(int id);
        Task<JobDTO> ConvertQuoteToJobAsync(int id);
    }
}
