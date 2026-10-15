namespace School.Contracts;

public sealed record ProfessorDto(int Id, string Name, string Department, string Email);

public sealed record StudentDto(int Id, string Name, string Email, int Year);

public sealed record CourseDto(int Id, string Name, string Code, string ProfessorName, int Credits);

public sealed record ClassRoomDto(int Id, string Name, string Building, int Capacity);
