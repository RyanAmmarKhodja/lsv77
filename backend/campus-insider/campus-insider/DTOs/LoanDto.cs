namespace campus_insider.DTOs
{
    public class LoanDto
    {
        public long Id { get; set; }
        public long EquipmentId { get; set; }
        public long BorrowerId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt {  get; set; }
    }
}
