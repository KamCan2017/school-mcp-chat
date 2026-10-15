using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using School.Infrastructure;
using School.Infrastructure.Data;
using School.Infrastructure.Entities;

namespace School.Tests;

[TestFixture]
public sealed class SchoolRepositoryTests
{
    private SqliteConnection _connection = null!;
    private DbContextOptions<SchoolDbContext> _options = null!;
    private SchoolRepository _repository = null!;

    [SetUp]
    public void SetUp()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();
        _options = new DbContextOptionsBuilder<SchoolDbContext>()
            .UseSqlite(_connection)
            .Options;
        _repository = new SchoolRepository(new TestDbContextFactory(_options));

        using var db = new SchoolDbContext(_options);
        db.Database.EnsureCreated();

        var ada = new Professor
        {
            Name = "Ada Lovelace",
            Department = "Mathematics",
            Email = "ada@school.test"
        };
        var grace = new Professor
        {
            Name = "Grace Hopper",
            Department = "Computer Science",
            Email = "grace@school.test"
        };
        var classroom = new ClassRoom
        {
            Name = "Room 101",
            Building = "North Hall",
            Capacity = 30
        };
        var lab = new ClassRoom
        {
            Name = "Lab 202",
            Building = "South Hall",
            Capacity = 20
        };
        var student = new Student
        {
            Name = "Katherine Johnson",
            Email = "katherine@school.test",
            Year = 3
        };
        var math = new Course
        {
            Name = "Calculus",
            Code = "MATH101",
            Credits = 4,
            Professor = ada,
            ClassRoom = classroom,
            Students = [student]
        };
        var programming = new Course
        {
            Name = "Programming Fundamentals",
            Code = "CS201",
            Credits = 3,
            Professor = grace,
            ClassRoom = lab
        };

        db.Courses.AddRange(math, programming);
        db.SaveChanges();
    }

    [TearDown]
    public void TearDown() => _connection.Dispose();

    [Test]
    public void FindProfessorByName_FindsCaseInsensitivePartialMatch()
    {
        var professor = _repository.FindProfessorByName("ADA LOV");

        Assert.That(professor, Is.Not.Null);
        Assert.That(professor!.Name, Is.EqualTo("Ada Lovelace"));
        Assert.That(professor.Department, Is.EqualTo("Mathematics"));
    }

    [Test]
    public void FindProfessorByName_ReturnsNullWhenNoMatchExists()
    {
        Assert.That(_repository.FindProfessorByName("Unknown"), Is.Null);
    }

    [Test]
    public void FindCourseByNameOrCode_FindsCourseAndMapsRelatedData()
    {
        var result = _repository.FindCourseByNameOrCode("math101");

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Course.Name, Is.EqualTo("Calculus"));
        Assert.That(result.Course.ProfessorName, Is.EqualTo("Ada Lovelace"));
        Assert.That(result.Classroom.Name, Is.EqualTo("Room 101"));
        Assert.That(result.Students, Has.Count.EqualTo(1));
        Assert.That(result.Students[0].Name, Is.EqualTo("Katherine Johnson"));
    }

    [Test]
    public void FindCourseByNameOrCode_ReturnsNullWhenNoMatchExists()
    {
        Assert.That(_repository.FindCourseByNameOrCode("missing"), Is.Null);
    }

    [Test]
    public void FindCoursesByProfessorName_FindsCoursesCaseInsensitively()
    {
        var courses = _repository.FindCoursesByProfessorName("grace hopper");

        Assert.That(courses, Has.Count.EqualTo(1));
        Assert.That(courses[0].Code, Is.EqualTo("CS201"));
        Assert.That(courses[0].ProfessorName, Is.EqualTo("Grace Hopper"));
    }

    [Test]
    public void FindCoursesByProfessorName_ReturnsEmptyListWhenNoMatchExists()
    {
        Assert.That(_repository.FindCoursesByProfessorName("Unknown"), Is.Empty);
    }

    [Test]
    public void GetAllCourses_ReturnsEveryCourse()
    {
        var courses = _repository.GetAllCourses();

        Assert.That(courses, Has.Count.EqualTo(2));
        Assert.That(courses.Select(course => course.Code), Is.EquivalentTo(["MATH101", "CS201"]));
        Assert.That(courses.All(course => !string.IsNullOrEmpty(course.ProfessorName)), Is.True);
    }

    [Test]
    public void GetAllProfessors_ReturnsEveryProfessor()
    {
        var professors = _repository.GetAllProfessors();
        Assert.That(professors, Has.Count.EqualTo(2));
        Assert.That(professors.Select(professor => professor.Name), Is.EquivalentTo(["Ada Lovelace", "Grace Hopper"]));
        Assert.That(professors.All(professor => !string.IsNullOrEmpty(professor.Department)), Is.True);
    }

    private sealed class TestDbContextFactory(DbContextOptions<SchoolDbContext> options)
        : IDbContextFactory<SchoolDbContext>
    {
        public SchoolDbContext CreateDbContext() => new(options);
    }
}
