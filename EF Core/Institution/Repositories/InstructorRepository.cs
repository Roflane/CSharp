using Microsoft.EntityFrameworkCore;

public class InstructorRepository
{
    private readonly InstitutionContext _context;

    public InstructorRepository(InstitutionContext context)
    {
        _context = context;
    }

    // Instructor → CourseAssignment → Course
    public List<Instructor> GetInstructorsWithCourses()
    {
        return _context.Instructors
            .Include(i => i.CourseAssignments)
            .ThenInclude(ca => ca.Course)
            .ToList();
    }

    public Instructor LoadInstructorCoursesExplicit(int instructorId)
    {
        var instructor = _context.Instructors.Find(instructorId);
        if (instructor != null)
        {
            _context.Entry(instructor)
                .Collection(i => i.CourseAssignments)
                .Load();
            
            foreach (var assignment in instructor.CourseAssignments)
            {
                _context.Entry(assignment)
                    .Reference(ca => ca.Course)
                    .Load();
            }
        }
        return instructor;
    }

    // Instructor → OfficeAssignment
    public List<Instructor> GetInstructorsWithOffice()
    {
        return _context.Instructors
            .Include(i => i.OfficeAssignment)
            .ToList();
    }

    public Instructor LoadInstructorOfficeExplicit(int instructorId)
    {
        var instructor = _context.Instructors.Find(instructorId);
        if (instructor != null)
        {
            _context.Entry(instructor)
                .Reference(i => i.OfficeAssignment)
                .Load();
        }
        return instructor;
    }
    
    public List<Instructor> GetInstructorsWithCoursesAndExams()
    {
        return _context.Instructors
            .Include(i => i.CourseAssignments)
            .ThenInclude(ca => ca.Course)
            .ThenInclude(c => c.Exams)
            .ToList();
    }

    public Instructor LoadInstructorCoursesExamsExplicit(int instructorId)
    {
        var instructor = _context.Instructors.Find(instructorId);
        if (instructor != null)
        {
            _context.Entry(instructor)
                .Collection(i => i.CourseAssignments)
                .Load();
            
            foreach (var assignment in instructor.CourseAssignments)
            {
                _context.Entry(assignment)
                    .Reference(ca => ca.Course)
                    .Load();
                
                _context.Entry(assignment.Course)
                    .Collection(c => c.Exams)
                    .Load();
            }
        }
        return instructor;
    }
    
    public List<Instructor> GetInstructorsWithoutCourses()
    {
        return _context.Instructors
            .Where(i => !i.CourseAssignments.Any())
            .Include(i => i.CourseAssignments)
            .ToList();
    }

    public List<Instructor> LoadInstructorsWithoutCoursesExplicit()
    {
        var instructors = _context.Instructors.ToList();
        foreach (var instructor in instructors)
        {
            _context.Entry(instructor)
                .Collection(i => i.CourseAssignments)
                .Load();
        }
        return instructors.Where(i => !i.CourseAssignments.Any()).ToList();
    }
}