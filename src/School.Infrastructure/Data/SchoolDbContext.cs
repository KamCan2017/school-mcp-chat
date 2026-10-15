using Microsoft.EntityFrameworkCore;
using School.Infrastructure.Entities;

namespace School.Infrastructure.Data;

public sealed class SchoolDbContext(DbContextOptions<SchoolDbContext> options) : DbContext(options)
{
    public DbSet<Professor> Professors => Set<Professor>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<ClassRoom> ClassRooms => Set<ClassRoom>();
    public DbSet<Course> Courses => Set<Course>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Professor>(entity =>
        {
            entity.Property(p => p.Name).HasMaxLength(200).IsRequired();
            entity.Property(p => p.Department).HasMaxLength(200).IsRequired();
            entity.Property(p => p.Email).HasMaxLength(200).IsRequired();
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.Property(s => s.Name).HasMaxLength(200).IsRequired();
            entity.Property(s => s.Email).HasMaxLength(200).IsRequired();
        });

        modelBuilder.Entity<ClassRoom>(entity =>
        {
            entity.Property(c => c.Name).HasMaxLength(100).IsRequired();
            entity.Property(c => c.Building).HasMaxLength(200).IsRequired();
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.Property(c => c.Name).HasMaxLength(200).IsRequired();
            entity.Property(c => c.Code).HasMaxLength(50).IsRequired();
            entity.HasOne(c => c.Professor)
                .WithMany(p => p.Courses)
                .HasForeignKey(c => c.ProfessorId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(c => c.ClassRoom)
                .WithMany(r => r.Courses)
                .HasForeignKey(c => c.ClassRoomId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasMany(c => c.Students)
                .WithMany(s => s.Courses);
        });
    }
}
