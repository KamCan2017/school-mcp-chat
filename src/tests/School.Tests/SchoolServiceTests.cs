using NUnit.Framework;
using School.Contracts;
using School.Infrastructure;
using School.McpServer;

namespace School.Tests;

[TestFixture]
public sealed class SchoolServiceTests
{
    private static readonly ProfessorDto Professor = new(1, "Ada Lovelace", "Mathematics", "ada@school.test");
    private static readonly StudentDto Student = new(2, "Katherine Johnson", "katherine@school.test", 3);
    private static readonly ClassRoomDto Classroom = new(3, "Room 101", "North Hall", 30);
    private static readonly CourseDto Course = new(4, "Calculus", "MATH101", Professor.Name, 4);

    [Test]
    public void FindProfessorByName_ReturnsProfessor()
    {
        var service = CreateService(new FakeSchoolRepository { Professor = Professor });

        Assert.That(service.FindProfessorByName("Ada"), Is.EqualTo(Professor));
    }

    [Test]
    public void FindProfessorByName_ThrowsWhenProfessorDoesNotExist()
    {
        var service = CreateService(new FakeSchoolRepository());

        var exception = Assert.Throws<KeyNotFoundException>(() => service.FindProfessorByName("Unknown"));

        Assert.That(exception!.Message, Is.EqualTo("No professor found named 'Unknown'."));
    }

    [Test]
    public void FindStudentsByCourseName_ReturnsCourseStudents()
    {
        var service = CreateService(new FakeSchoolRepository
        {
            CourseReadModel = new CourseReadModel(Course, Classroom, [Student])
        });

        Assert.That(service.FindStudentsByCourseName("Calculus"), Is.EqualTo(new[] { Student }));
    }

    [Test]
    public void FindStudentsByCourseName_ThrowsWhenCourseDoesNotExist()
    {
        var service = CreateService(new FakeSchoolRepository());

        var exception = Assert.Throws<KeyNotFoundException>(() => service.FindStudentsByCourseName("Unknown"));

        Assert.That(exception!.Message, Is.EqualTo("No course found named 'Unknown'."));
    }

    [Test]
    public void FindCoursesByProfessorName_RequiresProfessorAndReturnsCourses()
    {
        var repository = new FakeSchoolRepository
        {
            Professor = Professor,
            Courses = [Course]
        };
        var service = CreateService(repository);

        Assert.That(service.FindCoursesByProfessorName(Professor.Name), Is.EqualTo([Course ]));
        Assert.That(repository.LastProfessorQuery, Is.EqualTo(Professor.Name));
    }

    [Test]
    public void FindCoursesByProfessorName_ThrowsWhenProfessorDoesNotExist()
    {
        var repository = new FakeSchoolRepository { Courses = [Course] };
        var service = CreateService(repository);

        Assert.Throws<KeyNotFoundException>(() => service.FindCoursesByProfessorName("Unknown"));
        Assert.That(repository.CoursesQueryCount, Is.Zero);
    }

    [Test]
    public void FindClassroomByCourseName_ReturnsCourseClassroom()
    {
        var service = CreateService(new FakeSchoolRepository
        {
            CourseReadModel = new CourseReadModel(Course, Classroom, [Student])
        });

        Assert.That(service.FindClassroomByCourseName("Calculus"), Is.EqualTo(Classroom));
    }

    [Test]
    public void GetAllCourses_ReturnsRepositoryCourses()
    {
        var repository = new FakeSchoolRepository { Courses = [Course] };
        var service = CreateService(repository);

        Assert.That(service.GetAllCourses(), Is.EqualTo([Course]));
    }

    private static SchoolService CreateService(FakeSchoolRepository repository) => new(repository);

    private sealed class FakeSchoolRepository : ISchoolRepository
    {
        public ProfessorDto? Professor { get; init; }
        public CourseReadModel? CourseReadModel { get; init; }
        public List<CourseDto> Courses { get; init; } = [];
        public string? LastProfessorQuery { get; private set; }
        public int CoursesQueryCount { get; private set; }

        public ProfessorDto? FindProfessorByName(string name)
        {
            LastProfessorQuery = name;
            return Professor;
        }

        public CourseReadModel? FindCourseByNameOrCode(string courseName) => CourseReadModel;

        public List<CourseDto> FindCoursesByProfessorName(string professorName)
        {
            CoursesQueryCount++;
            return Courses;
        }

        public List<CourseDto> GetAllCourses() => Courses;

        public List<ProfessorDto> GetAllProfessors() => Professor is not null ? new List<ProfessorDto> { Professor } : new List<ProfessorDto>();
    }
}
