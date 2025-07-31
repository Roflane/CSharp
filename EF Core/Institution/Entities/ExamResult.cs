public class ExamResult
{
    public int Id { get; set; }
    public int ExamId { get; set; }
    public int StudentId { get; set; }
    public int Score { get; set; }
    
    public Exam Exam { get; set; }
    public Student Student { get; set; }
}