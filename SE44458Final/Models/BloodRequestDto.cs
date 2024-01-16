namespace SE44458Final.Models
{
    public class BloodRequestDto
    {
        public string Requester { get; set; }
        public string BloodType { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public string Town { get; set; }
        public int NumberOfUnits { get; set; }
        public int DurationOfSearch { get; set; }
        public string Reason { get; set; }
    }
}
