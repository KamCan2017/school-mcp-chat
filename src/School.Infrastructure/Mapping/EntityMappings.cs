using School.Contracts;
using School.Infrastructure.Entities;

namespace School.Infrastructure.Mapping;

internal static class EntityMappings
{
    public static ProfessorDto ToDto(this Professor professor) =>
        new(professor.Id, professor.Name, professor.Department, professor.Email);

    public static StudentDto ToDto(this Student student) =>
        new(student.Id, student.Name, student.Email, student.Year);

    public static ClassRoomDto ToDto(this ClassRoom classRoom) =>
        new(classRoom.Id, classRoom.Name, classRoom.Building, classRoom.Capacity);

    public static CourseDto ToDto(this Course course) =>
        new(course.Id, course.Name, course.Code, course.Professor.Name, course.Credits);
}
