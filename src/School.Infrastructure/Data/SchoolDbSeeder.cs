using School.Infrastructure.Entities;

namespace School.Infrastructure.Data;

public static class SchoolDbSeeder
{
    public static void Seed(SchoolDbContext db)
    {
        if (db.Professors.Any())
        {
            return;
        }

        var ada = new Professor { Id = 1, Name = "Ada Lovelace", Department = "Computer Science", Email = "ada.lovelace@school.edu" };
        var alan = new Professor { Id = 2, Name = "Alan Turing", Department = "Computer Science", Email = "alan.turing@school.edu" };
        var grace = new Professor { Id = 3, Name = "Grace Hopper", Department = "Software Engineering", Email = "grace.hopper@school.edu" };
        var edsger = new Professor { Id = 4, Name = "Edsger Dijkstra", Department = "Computer Science", Email = "edsger.dijkstra@school.edu" };

        var marie = new Student { Id = 1, Name = "Marie Curie", Email = "marie.curie@school.edu", Year = 2 };
        var niels = new Student { Id = 2, Name = "Niels Bohr", Email = "niels.bohr@school.edu", Year = 2 };
        var katherine = new Student { Id = 3, Name = "Katherine Johnson", Email = "katherine.johnson@school.edu", Year = 3 };
        var hedy = new Student { Id = 4, Name = "Hedy Lamarr", Email = "hedy.lamarr@school.edu", Year = 1 };
        var tim = new Student { Id = 5, Name = "Tim Berners-Lee", Email = "tim.berners-lee@school.edu", Year = 4 };
        var john = new Student { Id = 6, Name = "John von Neumann", Email = "john.vonneumann@school.edu", Year = 1 };

        var a101 = new ClassRoom { Id = 1, Name = "A-101", Building = "Ada Building", Capacity = 40 };
        var b204 = new ClassRoom { Id = 2, Name = "B-204", Building = "Turing Hall", Capacity = 32 };
        var c12 = new ClassRoom { Id = 3, Name = "C-12", Building = "Hopper Lab", Capacity = 24 };
        var a220 = new ClassRoom { Id = 4, Name = "A-220", Building = "Ada Building", Capacity = 36 };
        var d03 = new ClassRoom { Id = 5, Name = "D-03", Building = "Math Wing", Capacity = 50 };

        db.Professors.AddRange(ada, alan, grace, edsger);
        db.Students.AddRange(marie, niels, katherine, hedy, tim, john);
        db.ClassRooms.AddRange(a101, b204, c12, a220, d03);
        db.Courses.AddRange(
            new Course
            {
                Id = 1,
                Name = "Algorithms",
                Code = "CS201",
                Credits = 6,
                Professor = ada,
                ClassRoom = a101,
                Students = { marie, niels, katherine }
            },
            new Course
            {
                Id = 2,
                Name = "Databases",
                Code = "CS310",
                Credits = 5,
                Professor = alan,
                ClassRoom = b204,
                Students = { katherine, hedy }
            },
            new Course
            {
                Id = 3,
                Name = "Compilers",
                Code = "CS420",
                Credits = 6,
                Professor = grace,
                ClassRoom = c12,
                Students = { niels, tim }
            },
            new Course
            {
                Id = 4,
                Name = "Operating Systems",
                Code = "CS330",
                Credits = 5,
                Professor = edsger,
                ClassRoom = a220,
                Students = { marie, hedy, tim }
            },
            new Course
            {
                Id = 5,
                Name = "Discrete Mathematics",
                Code = "CS110",
                Credits = 4,
                Professor = ada,
                ClassRoom = d03,
                Students = { hedy, john }
            });

        db.SaveChanges();
    }
}
