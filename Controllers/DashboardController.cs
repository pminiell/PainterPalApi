using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PainterPalApi.Data;
using PainterPalApi.Models;

namespace PainterPalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // This ensures only authenticated users can access
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<DashboardSummary>> GetDashboardData()
        {
            // Count active jobs
            var activeJobsCount = await _context.Jobs
                .Where(j => j.JobStatus == JobStatus.InProgress)
                .CountAsync();

            // Count total customers
            var customersCount = await _context.Customers.CountAsync();

            // Count pending quotes
            var pendingQuotesCount = await _context.Quotes
                .Where(q => q.QuoteStatus == QuoteStatus.Pending)
                .CountAsync();

            // Get recent jobs
            var recentJobs = await _context.Jobs
                .OrderByDescending(j => j.StartDate)
                .Take(5)
                .Select(j => new JobSummary
                {
                    Id = j.Id,
                    Name = j.JobName,
                    Status = j.JobStatus,
                    StartDate = j.StartDate,
                    CustomerId = j.CustomerId
                })
                .ToListAsync();

            return new DashboardSummary
            {
                ActiveJobsCount = activeJobsCount,
                CustomersCount = customersCount,
                PendingQuotesCount = pendingQuotesCount,
                RecentJobs = recentJobs
            };
        }

        [HttpGet("stats")]
        [Authorize(Policy = "BusinessOwnerPolicy")] // Only business owners can see stats
        public async Task<ActionResult<DashboardStats>> GetDashboardStats()
        {
            // Calculate job completion rate
            var totalJobs = await _context.Jobs.CountAsync();
            var completedJobs = await _context.Jobs
                .Where(j => j.JobStatus == JobStatus.Completed)
                .CountAsync();

            var completionRate = totalJobs > 0 ? (double)completedJobs / totalJobs : 0;

            // Get the total number of jobs per month (for the last 6 months)
            var sixMonthsAgo = DateTime.Now.AddMonths(-6);

            var jobsByMonth = await _context.Jobs
                .Where(j => j.CreatedAt >= sixMonthsAgo)
                .GroupBy(j => new { j.CreatedAt.Year, j.CreatedAt.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Count = g.Count()
                })
                .OrderBy(r => r.Year)
                .ThenBy(r => r.Month)
                .ToListAsync();

            return new DashboardStats
            {
                JobCompletionRate = completionRate,
                JobsByMonth = jobsByMonth.Select(j => new MonthlyJobCount
                {
                    Year = j.Year,
                    Month = j.Month,
                    Count = j.Count
                }).ToList()
            };
        }
    }

    // Models for dashboard responses
    public class DashboardSummary
    {
        public int ActiveJobsCount { get; set; }
        public int CustomersCount { get; set; }
        public int PendingQuotesCount { get; set; }
        public List<JobSummary> RecentJobs { get; set; }
    }

    public class JobSummary
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public JobStatus Status { get; set; }
        public DateTime StartDate { get; set; }
        public int CustomerId { get; set; }
    }

    public class DashboardStats
    {
        public double JobCompletionRate { get; set; }
        public List<MonthlyJobCount> JobsByMonth { get; set; }
    }

    public class MonthlyJobCount
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Count { get; set; }
    }
}
