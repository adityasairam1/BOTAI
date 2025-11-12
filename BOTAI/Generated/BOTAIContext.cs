using Microsoft.EntityFrameworkCore;

namespace BOTAI.Generated
{
    public partial class BOTAIContext : DbContext
    {
        public BOTAIContext()
        {

        }

        public BOTAIContext(DbContextOptions<BOTAIContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("name=Scaffolding");
            }
        }
    }
}
