using Microsoft.EntityFrameworkCore;
using School.Contracts;
using School.Infrastructure.Data;
using School.Infrastructure.Mapping;
using System.Xml.Linq;

namespace School.Infrastructure;

public sealed class SchoolRepository(IDbContextFactory<SchoolDbContext> dbContextFactory) : ISchoolRepository
{
    public ProfessorDto? FindProfessorByName(string name)
    {
        using var db = dbContextFactory.CreateDbContext();
        return db.Professors
            .AsNoTracking()
            .AsEnumerable()
            .FirstOrDefault(p => Matches(p.Name, name))
            ?.ToDto();
    }

    public CourseReadModel? FindCourseByNameOrCode(string courseName)
    {
        using var db = dbContextFactory.CreateDbContext();
        var course = db.Courses
            .AsNoTracking()
            .Include(c => c.Professor)
            .Include(c => c.ClassRoom)
            .Include(c => c.Students)
            .AsEnumerable()
            .FirstOrDefault(c => Matches(c.Name, courseName) || Matches(c.Code, courseName));

        return course is null
            ? null
            : new CourseReadModel(
                course.ToDto(),
                course.ClassRoom.ToDto(),
                course.Students.Select(s => s.ToDto()).ToList());
    }

    public List<CourseDto> FindCoursesByProfessorName(string professorName)
    {
        using var db = dbContextFactory.CreateDbContext();
        return db.Courses
            .AsNoTracking()
            .Include(c => c.Professor)
            .AsEnumerable()
            .Where(c => Matches(c.Professor.Name, professorName))
            .Select(c => c.ToDto())
            .ToList();
    }

    public List<CourseDto> GetAllCourses()
    {
        using var db = dbContextFactory.CreateDbContext();
        return db.Courses
            .AsNoTracking()
            .Include(c => c.Professor)
            .AsEnumerable()
            .Select(c => c.ToDto())
            .ToList();
    }

    private static bool Matches(string value, string query) =>
        value.Contains(query, StringComparison.OrdinalIgnoreCase);

    public List<ProfessorDto> GetAllProfessors()
    {
        using var db = dbContextFactory.CreateDbContext();
        return db.Professors
            .AsNoTracking()
            .AsEnumerable()
            .Select(p => p.ToDto())
            .ToList();
    }
}
