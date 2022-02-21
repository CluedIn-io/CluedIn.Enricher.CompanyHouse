using System.Collections.Generic;

namespace CluedIn.ExternalSearch.Providers.CompanyHouse.Model
{
    public class PscExemptAsTradingOnRegulatedMarket
    {
        public string exemption_type { get; set; }
        public List<ExemptionItemItem3> items { get; set; }
    }
}
