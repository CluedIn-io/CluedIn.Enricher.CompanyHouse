using System.Collections.Generic;

namespace CluedIn.ExternalSearch.Providers.CompanyHouse.Model
{
    public class FilingHistory
    {
        public string etag { get; set; }
        public string filing_history_status { get; set; }
        public List<Item> items { get; set; }
        public string items_per_page { get; set; }
        public string kind { get; set; }
        public string start_index { get; set; }
        public string total_count { get; set; }
    }
}
