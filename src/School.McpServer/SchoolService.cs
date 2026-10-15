using School.Contracts;
using School.Infrastructure;

namespace School.McpServer;

public sealed class SchoolService(ISchoolRepository repository) : ISchoolService
{
    public ProfessorDto FindProfessorByName(string name) =>
        repository.FindProfessorByName(name)
        ?? throw new KeyNotFoundException($"No professor found named '{name}'.");

    public List<StudentDto> FindStudentsByCourseName(string courseName) =>
        FindCourse(courseName).Students.ToList();

    public List<CourseDto> FindCoursesByProfessorName(string professorName)
    {
        FindProfessorByName(professorName);
        return repository.FindCoursesByProfessorName(professorName);
    }

    public ClassRoomDto FindClassroomByCourseName(string courseName) =>
        FindCourse(courseName).Classroom;

    public List<CourseDto> GetAllCourses() =>
        repository.GetAllCourses();

    public List<ProfessorDto> GetAllProfessors() =>
        repository.GetAllProfessors();

    private CourseReadModel FindCourse(string courseName) =>
        repository.FindCourseByNameOrCode(courseName)
        ?? throw new KeyNotFoundException($"No course found named '{courseName}'.");
}
