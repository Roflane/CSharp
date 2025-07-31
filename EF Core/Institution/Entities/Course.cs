public class Course
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int Credit { get; set; }
    public int DepartmentId { get; set; }
    
    public Department Department { get; set; }
    public ICollection<Enrollment> Enrollments { get; set; }
    public ICollection<CourseAssignment> CourseAssignments { get; set; }
    public ICollection<Exam> Exams { get; set; }
}