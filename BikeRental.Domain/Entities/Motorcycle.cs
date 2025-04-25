namespace BikeRental.Domain.Entities
{
    public class Motorcycle : BaseEntity
    {
        public int Year { get; set; }
        public string Model { get; set; }
        public string LicensePlate { get; set; }

        public ICollection<Rental> Rentals { get; set; }
    }
}
