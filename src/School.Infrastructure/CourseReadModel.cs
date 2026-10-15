using School.Contracts;

namespace School.Infrastructure;

public sealed record CourseReadModel(
    CourseDto Course,
    ClassRoomDto Classroom,
    IReadOnlyList<StudentDto> Students);
