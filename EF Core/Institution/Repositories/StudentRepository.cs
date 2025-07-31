using Microsoft.EntityFrameworkCore;

public class StudentRepository {
    private readonly InstitutionContext _context;

    public StudentRepository(InstitutionContext context) {
        _context = context;
    }

    // Eager Loading: Student → Profile
    public List<Student> GetStudentsWithProfiles() {
        return _context.Students
            .Include(s => s.Profile)
            .ToList();
    }

    // Explicit Loading: Student → Enrollments → Course
    public Student LoadStudentWithCourses(int studentId) {
        var student = _context.Students.Find(studentId);
        
        if (student != null) {
            _context.Entry(student)
                .Collection(s => s.Enrollments)
                .Load();

            foreach (var enrollment in student.Enrollments) {
                _context.Entry(enrollment)
                    .Reference(e => e.Course)
                    .Load();
            }
        }

        return student;
    }
    
    // Задание 12: Студенты с экзаменами по конкретному курсу
    public List<Student> GetStudentsWithExamsByCourse(int courseId)
    {
        return _context.Students
            .Where(s => s.ExamResults.Any(er => er.Exam.CourseId == courseId))
            .Include(s => s.ExamResults)
            .ThenInclude(er => er.Exam)
            .ToList();
    }

    public List<Student> LoadStudentsWithExamsByCourseExplicit(int courseId)
    {
        var students = _context.Students.ToList();
        foreach (var student in students)
        {
            _context.Entry(student)
                .Collection(s => s.ExamResults)
                .Query().Include(examResult => examResult.Exam)
                .Where(er => er.Exam.CourseId == courseId)
                .Include(er => er.Exam)
                .Load();
        }
        return students.Where(s => s.ExamResults.Any()).ToList();
    }
    
    public List<Student> GetStudentsWithMoreThan3Exams()
    {
        return _context.Students
            .Where(s => s.ExamResults.Count > 3)
            .Include(s => s.ExamResults)
            .ToList();
    }

    public List<Student> LoadStudentsWithMoreThan3ExamsExplicit()
    {
        var students = _context.Students.ToList();
        foreach (var student in students)
        {
            _context.Entry(student)
                .Collection(s => s.ExamResults)
                .Load();
        }
        return students.Where(s => s.ExamResults.Count > 3).ToList();
    }
    
    
}