namespace CluedIn.ExternalSearch.Providers.CompanyHouse.Model
{
    public class CompanyRegisterResponse
    {
        public string company_number { get; set; }
        public string etag { get; set; }
        public string kind { get; set; }
        public Links links { get; set; }
        public Registers registers { get; set; }
    }
}
