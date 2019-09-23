using System.Collections.Generic;

namespace CluedIn.ExternalSearch.Providers.CompanyHouse.Model
{
    public class PscExemptAsSharesAdmittedOnMarket
    {
        public string exemption_type { get; set; }
        public List<ExemptionItemItem2> items { get; set; }
    }
}