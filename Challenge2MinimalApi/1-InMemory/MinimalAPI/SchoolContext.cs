using Microsoft.EntityFrameworkCore;

public class SchoolContext : DbContext
{
    public DbSet<StudentInfo> Students { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=school.db");
    }
}
