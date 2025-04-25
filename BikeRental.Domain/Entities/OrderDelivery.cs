namespace BikeRental.Domain.Entities
{
    public class OrderDelivery : BaseEntity
    {
        public DateTime DeliveryDate { get; set; }
        public decimal Value { get; set; }

        public Guid RentalId { get; set; }

        public Rental Rental { get; set; }

        public bool IsCompleted { get; set; } = false;

    }
}
