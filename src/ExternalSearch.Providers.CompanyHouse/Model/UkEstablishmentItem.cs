namespace CluedIn.ExternalSearch.Providers.CompanyHouse.Model
{
    public class UkEstablishmentItem
    {
        public string company_name { get; set; }
        public string company_number { get; set; }
        public string company_status { get; set; }
        public UkEstablishmentLinks links { get; set; }
        public string locality { get; set; }
    }
}
