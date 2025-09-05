namespace Order.Application.Dtos
{
    public class AddressDto
    {
        public string Province { get; set; }=string.Empty;

        public string District { get; set; }=string.Empty;

        public string Street { get; set; }=string.Empty;

        public string ZipCode { get; set; }=string.Empty;

        public string Line { get; set; }=string.Empty;
    }
}