namespace CluedIn.ExternalSearch.Providers.CompanyHouse.Model
{
    public class Contact
    {
        public ContactLinks links { get; set; }
        public string name { get; set; }
        public string officer_role { get; set; }
        public string appointed_on { get; set; }
        public ContactAddress address { get; set; }
        public ContactIdentification identification { get; set; }
        public DateOfBirth date_of_birth { get; set; }
        public string country_of_residence { get; set; }
        public string occupation { get; set; }
        public string nationality { get; set; }
        public string resigned_on { get; set; }
        public AppointmentResponse appointmentResponse { get; set; }
        public OfficerResponse disqualifiedNaturalResponse { get; set; }
        public OfficerResponse disqualifiedCorporateResponse { get; set; }
    }
}