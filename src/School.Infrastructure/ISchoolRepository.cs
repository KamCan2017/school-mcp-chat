using School.Contracts;

namespace School.Infrastructure;

public interface ISchoolRepository
{
    ProfessorDto? FindProfessorByName(string name);
    CourseReadModel? FindCourseByNameOrCode(string courseName);
    List<CourseDto> FindCoursesByProfessorName(string professorName);
    List<CourseDto> GetAllCourses();
    List<ProfessorDto> GetAllProfessors();
}
