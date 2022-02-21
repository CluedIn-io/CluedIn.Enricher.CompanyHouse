using System.Collections.Generic;

namespace CluedIn.ExternalSearch.Providers.CompanyHouse.Model
{
    public class CompanyResult
    {
        public string kind { get; set; }
        public int page_number { get; set; }
        public List<CompanyNew> items { get; set; }
        public int start_index { get; set; }
        public int total_results { get; set; }
        public int items_per_page { get; set; }
    }
}
