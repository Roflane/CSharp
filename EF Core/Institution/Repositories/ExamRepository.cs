using Microsoft.EntityFrameworkCore;

public class ExamRepository
{
    private readonly InstitutionContext _context;

    public ExamRepository(InstitutionContext context)
    {
        _context = context;
    }

    // Задание 6: Exam → Course
    public List<Exam> GetExamsWithCourses()
    {
        return _context.Exams
            .Include(e => e.Course)
            .ToList();
    }

    public Exam LoadExamCourseExplicit(int examId)
    {
        var exam = _context.Exams.Find(examId);
        if (exam != null)
        {
            _context.Entry(exam)
                .Reference(e => e.Course)
                .Load();
        }
        return exam;
    }

    // Задание 7: ExamResult → Exam + Student
    public List<ExamResult> GetExamResultsWithDetails()
    {
        return _context.ExamResults
            .Include(er => er.Exam)
            .Include(er => er.Student)
            .ToList();
    }

    public ExamResult LoadExamResultDetailsExplicit(int examResultId)
    {
        var examResult = _context.ExamResults.Find(examResultId);
        if (examResult != null)
        {
            _context.Entry(examResult)
                .Reference(er => er.Exam)
                .Load();
            
            _context.Entry(examResult)
                .Reference(er => er.Student)
                .Load();
        }
        return examResult;
    }
}