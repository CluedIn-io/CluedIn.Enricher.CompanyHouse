using System.Collections.Generic;

namespace CluedIn.ExternalSearch.Providers.CompanyHouse.Model
{
    public class ControlPersonResponse
    {
        public string active_count { get; set; }
        public string ceased_count { get; set; }
        public string etag { get; set; }
        public List<ControlPersonItem> items { get; set; }
        public string items_per_page { get; set; }
        public string kind { get; set; }
        public ControlPersonLinks2 links { get; set; }
        public string start_index { get; set; }
        public string total_results { get; set; }
    }
}
