using System.Collections.Generic;

namespace CluedIn.ExternalSearch.Providers.CompanyHouse.Model
{
    public class UkEstablishmentResponse
    {
        public string etag { get; set; }
        public List<UkEstablishmentItem> items { get; set; }
        public string kind { get; set; }
        public UkEstablishmentLinks2 links { get; set; }
    }
}
