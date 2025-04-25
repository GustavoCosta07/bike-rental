namespace BikeRental.Domain.Entities
{
    public class DeliveryPerson : BaseEntity
    {
        public string Name { get; set; }
        public string CNPJ { get; set; }
        public DateTime BirthDate { get; set; }
        public string DriverLicenseNumber { get; set; }
        public LicenseType DriverLicenseType { get; set; }
        public string DriverLicenseImageUrl { get; set; }

        public ICollection<Rental> Rentals { get; set; }
    }
}
