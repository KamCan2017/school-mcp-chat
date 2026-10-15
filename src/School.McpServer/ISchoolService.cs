using School.Contracts;

namespace School.McpServer;

public interface ISchoolService
{
    ProfessorDto FindProfessorByName(string name);
    List<StudentDto> FindStudentsByCourseName(string courseName);
    List<CourseDto> FindCoursesByProfessorName(string professorName);
    ClassRoomDto FindClassroomByCourseName(string courseName);
    List<CourseDto> GetAllCourses();
    List<ProfessorDto> GetAllProfessors();
}
