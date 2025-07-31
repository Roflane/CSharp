public class Exam
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public DateTime Date { get; set; }
    public int MaxScore { get; set; }
    
    public Course Course { get; set; }
    public ICollection<ExamResult> ExamResults { get; set; }
}