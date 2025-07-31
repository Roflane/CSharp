using Microsoft.EntityFrameworkCore;

public class CourseRepository
{
    private readonly InstitutionContext _context;

    public CourseRepository(InstitutionContext context)
    {
        _context = context;
    }
    
    public List<Course> GetCoursesWithDepartments()
    {
        return _context.Courses
            .Include(c => c.Department)
            .ToList();
    }

    public Course LoadCourseDepartmentExplicit(int courseId)
    {
        var course = _context.Courses.Find(courseId);
        if (course != null)
        {
            _context.Entry(course)
                .Reference(c => c.Department)
                .Load();
        }
        return course;
    }
    
    public List<Course> GetCoursesWithExamResultsAndStudents()
    {
        return _context.Courses
            .Include(c => c.Exams)
            .ThenInclude(e => e.ExamResults)
            .ThenInclude(er => er.Student)
            .ToList();
    }

    public Course LoadExamResultsChainExplicit(int courseId)
    {
        var course = _context.Courses.Find(courseId);
        if (course != null)
        {
            _context.Entry(course)
                .Collection(c => c.Exams)
                .Load();
            
            foreach (var exam in course.Exams)
            {
                _context.Entry(exam)
                    .Collection(e => e.ExamResults)
                    .Load();
                
                foreach (var result in exam.ExamResults)
                {
                    _context.Entry(result)
                        .Reference(er => er.Student)
                        .Load();
                }
            }
        }
        return course;
    }
    
    public List<Course> GetCoursesWithoutExams()
    {
        return _context.Courses
            .Where(c => !c.Exams.Any())
            .Include(c => c.Exams)
            .ToList();
    }

    public List<Course> LoadCoursesWithoutExamsExplicit()
    {
        var courses = _context.Courses.ToList();
        foreach (var course in courses)
        {
            _context.Entry(course)
                .Collection(c => c.Exams)
                .Load();
        }
        return courses.Where(c => !c.Exams.Any()).ToList();
    }
    
    
}