public class Instructor
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime HireDate { get; set; }
    public int? DepartmentId { get; set; }
    
    public Department Department { get; set; }
    public OfficeAssignment OfficeAssignment { get; set; }
    public ICollection<CourseAssignment> CourseAssignments { get; set; }
}