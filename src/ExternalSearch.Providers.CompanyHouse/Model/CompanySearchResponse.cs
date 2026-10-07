using System.Collections.Generic;

namespace CluedIn.ExternalSearch.Providers.CompanyHouse.Model
{
    public class CompanySearchResponse
    {
        public string etag { get; set; }
        public List<CompanySearchResultItem> items { get; set; }
        public int items_per_page { get; set; }
        public string kind { get; set; }
        public int start_index { get; set; }
        public int total_results { get; set; }
    }
}
