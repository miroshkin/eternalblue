using Microsoft.EntityFrameworkCore;

namespace EternalBlue.Models
{
    public partial class IFSContext : DbContext
    {
        public IFSContext()
        {
        }

        public IFSContext(DbContextOptions<IFSContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<ProcessedCandidate> ProcessedCandidates { get; set; }

        public DbSet<ProcessedCandidateSkill> ProcessedCandidateSkills { get; set; }
        
    }
}
