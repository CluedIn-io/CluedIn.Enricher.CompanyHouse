using System.Collections.Generic;

namespace CluedIn.ExternalSearch.Providers.CompanyHouse.Model
{
    public class ChargeResponse
    {
        public string etag { get; set; }
        public List<ChargeItem> items { get; set; }
        public string part_satisfied_count { get; set; }
        public string satisfied_count { get; set; }
        public string total_count { get; set; }
        public string unfiletered_count { get; set; }
    }
}