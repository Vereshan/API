using Microsoft.EntityFrameworkCore;
using ReportAPI.Models;

namespace ReportAPI.Data
{
    public class ReportContext : DbContext
    {
        public ReportContext(DbContextOptions<ReportContext> options) : base(options) { }

        public DbSet<Report> Reports { get; set; }
    }
}