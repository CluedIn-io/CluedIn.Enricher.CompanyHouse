namespace CluedIn.ExternalSearch.Providers.CompanyHouse.Model
{
    public class ExceptionResponse
    {
        public string etag { get; set; }
        public Exemptions exemptions { get; set; }
        public string kind { get; set; }
        public Links links { get; set; }
    }
}
