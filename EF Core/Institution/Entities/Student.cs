public class Student
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime BirthDate { get; set; }
    
    public StudentProfile Profile { get; set; }
    public ICollection<Enrollment> Enrollments { get; set; }
    public ICollection<ExamResult> ExamResults { get; set; }
}