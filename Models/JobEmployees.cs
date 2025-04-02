namespace PainterPalApi.Models
{
    public class JobEmployee
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public Job Job { get; set; }
        public int EmployeeId { get; set; }
        public User Employee { get; set; }
        public DateTime AssignedDate { get; set; }
    }
}