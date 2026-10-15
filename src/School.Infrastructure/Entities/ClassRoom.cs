namespace School.Infrastructure.Entities;

public sealed class ClassRoom
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Building { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public ICollection<Course> Courses { get; set; } = [];
}
