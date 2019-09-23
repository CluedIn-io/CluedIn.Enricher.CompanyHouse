using System.Collections.Generic;

namespace CluedIn.ExternalSearch.Providers.CompanyHouse.Model
{
    public class CompanySearchResultItem
    {
        public Address address { get; set; }
        public string address_snippet { get; set; }
        public string company_number { get; set; }
        public string company_status { get; set; }
        public string company_type { get; set; }
        public string date_of_cessation { get; set; }
        public string date_of_creation { get; set; }
        public string description { get; set; }
        public List<string> description_identifier { get; set; }
        public string kind { get; set; }
        public Links links { get; set; }
        public Matches matches { get; set; }
        public string snippet { get; set; }
        public string title { get; set; }
    }
}