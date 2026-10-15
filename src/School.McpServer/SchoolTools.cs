using System.ComponentModel;
using ModelContextProtocol.Server;
using School.Contracts;

namespace School.McpServer;

[McpServerToolType]
internal class SchoolTools(ISchoolService service)
{
    [McpServerTool(Name = "FindProfessorByName")]
    [Description("Find a professor by name.")]
    public ProfessorDto FindProfessorByName(
        [Description("Name of the professor")] string name)
    {
        return service.FindProfessorByName(name);
    }

    [McpServerTool(Name = "FindStudentsByCourseName")]
    [Description("Find students enrolled in a specific course.")]
    public List<StudentDto> FindStudentsByCourseName(
        [Description("Name of the course")] string courseName)
    {
        return service.FindStudentsByCourseName(courseName);
    }

    [McpServerTool(Name = "FindCoursesByProfessorName")]
    [Description("Find the courses for a specific professor.")]
    public List<CourseDto> FindCoursesByProfessorName(
        [Description("Name of the professor")] string professorName)
    {
        return service.FindCoursesByProfessorName(professorName);
    }

    [McpServerTool(Name = "FindClassroomByCourseName")]
    [Description("Find the classroom for a specific course.")]
    public ClassRoomDto FindClassroomByCourseName(
        [Description("Name of the course")] string courseName)
    {
        return service.FindClassroomByCourseName(courseName);
    }

    [McpServerTool(Name = "GetAllCourses")]
    [Description("Get a list of all courses.")]
    public List<CourseDto> GetAllCourses()
    {
        return service.GetAllCourses();
    }

    [McpServerTool(Name = "GetAllProfessors")]  
    [Description("Get a list of all professors.")]
    public List<ProfessorDto> GetAllProfessors()
    {
        return service.GetAllProfessors();
    }
}
