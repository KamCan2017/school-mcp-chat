namespace School.Infrastructure.Entities;

public sealed class Student
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Year { get; set; }
    public ICollection<Course> Courses { get; set; } = [];
}
