namespace BikeRental.Domain.Entities
{
    public class Notification : BaseEntity
    {
        public string Message { get; set; }
        public DateTime NotificationDate { get; set; }
        public bool IsRead { get; set; }
    }
}
