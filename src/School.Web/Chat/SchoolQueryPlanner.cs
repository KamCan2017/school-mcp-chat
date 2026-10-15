using System.Text.RegularExpressions;

namespace School.Web.Chat;

public sealed class SchoolQueryPlanner
{
    private static readonly string[] Professors =
    [
        "Ada Lovelace", "Alan Turing", "Grace Hopper", "Edsger Dijkstra"
    ];

    private static readonly string[] Courses =
    [
        "Discrete Mathematics", "Operating Systems", "Algorithms", "Databases", "Compilers"
    ];

    public PlannedRequest Plan(string userMessage)
    {
        var text = userMessage.Trim();
        if (string.IsNullOrWhiteSpace(text))
        {
            return PlannedRequest.Unknown("Type a question about professors, courses, students, or classrooms.");
        }

        if (LooksLikeAllCourses(text))
        {
            return new PlannedRequest(
                "GetAllCourses",
                new Dictionary<string, object?>(),
                "List every course in the school.");
        }

        if (LooksLikeAllProfessors(text))
        {
            return new PlannedRequest(
                "GetAllProfessors",
                new Dictionary<string, object?>(),
                "List every professor in the school.");
        }

        if (LooksLikeClassroom(text))
        {
            var course = ExtractCourse(text);
            return course is null
                ? PlannedRequest.Unknown("Which course's classroom should I look up? Try: “Where is Algorithms taught?”")
                : new PlannedRequest(
                    "FindClassroomByCourseName",
                    new Dictionary<string, object?> { ["courseName"] = course },
                    $"Find the classroom for {course}.");
        }

        if (LooksLikeStudents(text))
        {
            var course = ExtractCourse(text);
            return course is null
                ? PlannedRequest.Unknown("Which course should I look up students for? Try: “Who is enrolled in Databases?”")
                : new PlannedRequest(
                    "FindStudentsByCourseName",
                    new Dictionary<string, object?> { ["courseName"] = course },
                    $"Find students enrolled in {course}.");
        }

        if (LooksLikeProfessorCourses(text))
        {
            var professor = ExtractProfessor(text);
            return professor is null
                ? PlannedRequest.Unknown("Which professor's courses should I look up? Try: “What courses does Ada Lovelace teach?”")
                : new PlannedRequest(
                    "FindCoursesByProfessorName",
                    new Dictionary<string, object?> { ["professorName"] = professor },
                    $"Find courses taught by {professor}.");
        }

        if (LooksLikeProfessor(text) || ExtractProfessor(text) is not null)
        {
            var professor = ExtractProfessor(text);
            return professor is null
                ? PlannedRequest.Unknown("Which professor should I look up? Try: “Find professor Alan Turing.”")
                : new PlannedRequest(
                    "FindProfessorByName",
                    new Dictionary<string, object?> { ["name"] = professor },
                    $"Find professor {professor}.");
        }

        var courseOnly = ExtractCourse(text);
        if (courseOnly is not null)
        {
            return new PlannedRequest(
                "FindClassroomByCourseName",
                new Dictionary<string, object?> { ["courseName"] = courseOnly },
                $"Find the classroom for {courseOnly}.");
        }

        return PlannedRequest.Unknown(
            "I can look up professors, courses, enrolled students, and classrooms. Try one of the suggested questions.");
    }

    private static bool LooksLikeAllCourses(string text) =>
        Regex.IsMatch(text, @"\b(all|list|show|every)\b.*\bcourses?\b", RegexOptions.IgnoreCase)
        || Regex.IsMatch(text, @"\bwhat courses (are there|do you (have|offer))\b", RegexOptions.IgnoreCase);

    private static bool LooksLikeAllProfessors(string text) =>
        Regex.IsMatch(text, @"\b(all|list|show|every)\b.*\b(professors?|teachers?)\b", RegexOptions.IgnoreCase)
        || Regex.IsMatch(text, @"\bwhat (professors?|teachers?) (are there|do you (have|offer))\b", RegexOptions.IgnoreCase);

    private static bool LooksLikeClassroom(string text) =>
        Regex.IsMatch(text, @"\b(classroom|class room|room|where|taught|location)\b", RegexOptions.IgnoreCase);

    private static bool LooksLikeStudents(string text) =>
        Regex.IsMatch(text, @"\b(students?|enrolled|who (is|are) (in|taking)|takes)\b", RegexOptions.IgnoreCase);

    private static bool LooksLikeProfessorCourses(string text) =>
        Regex.IsMatch(text, @"\b(courses?|teach(es|ing)?|classes)\b", RegexOptions.IgnoreCase)
        && (Regex.IsMatch(text, @"\b(professor|taught|teach)\b", RegexOptions.IgnoreCase)
            || ExtractProfessor(text) is not null);

    private static bool LooksLikeProfessor(string text) =>
        Regex.IsMatch(text, @"\b(professor|teacher|find|who is)\b", RegexOptions.IgnoreCase);

    private static string? ExtractProfessor(string text) =>
        Professors.FirstOrDefault(name => text.Contains(name, StringComparison.OrdinalIgnoreCase))
        ?? Professors.FirstOrDefault(name =>
            name.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Any(part => part.Length > 3 && text.Contains(part, StringComparison.OrdinalIgnoreCase)));

    private static string? ExtractCourse(string text) =>
        Courses.FirstOrDefault(name => text.Contains(name, StringComparison.OrdinalIgnoreCase));
}

public sealed record PlannedRequest(
    string? ToolName,
    IReadOnlyDictionary<string, object?> Arguments,
    string Summary)
{
    public bool IsToolCall => ToolName is not null;

    public static PlannedRequest Unknown(string summary) =>
        new(null, new Dictionary<string, object?>(), summary);
}
