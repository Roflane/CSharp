public class Course {
    public int Id { get; set; }
    public string Title { get; set; }
    public int Credit { get; set; }
    public ICollection<StudentCourse> Courses { get; set; }
}