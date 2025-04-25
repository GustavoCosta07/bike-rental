namespace BikeRental.Domain.Entities
{
    public class Rental : BaseEntity
    {
        public Guid MotorcycleId { get; set; }
        public Guid DeliveryPersonId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime ExpectedEndDate { get; set; }
        public decimal DailyCost { get; set; }
        public RentalPlan Plan { get; set; }
        public bool IsActive { get; set; }
        public DateTime? ReturnDate { get; set; } 

        public Motorcycle Motorcycle { get; set; }
        public DeliveryPerson DeliveryPerson { get; set; }
        public ICollection<OrderDelivery> OrderDeliveries { get; set; } = new List<OrderDelivery>();
    }
}