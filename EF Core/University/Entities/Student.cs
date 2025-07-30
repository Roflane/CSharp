public class Student {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string BirthDate { get; set; }
    
    public ICollection<StudentCourse> Students { get; set; }
}