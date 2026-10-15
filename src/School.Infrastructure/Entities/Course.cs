namespace School.Infrastructure.Entities;

public sealed class Course
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int Credits { get; set; }
    public int ProfessorId { get; set; }
    public Professor Professor { get; set; } = null!;
    public int ClassRoomId { get; set; }
    public ClassRoom ClassRoom { get; set; } = null!;
    public ICollection<Student> Students { get; set; } = [];
}
