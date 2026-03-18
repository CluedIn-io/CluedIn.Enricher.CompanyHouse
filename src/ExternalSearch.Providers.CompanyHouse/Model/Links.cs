using Newtonsoft.Json;

namespace CluedIn.ExternalSearch.Providers.CompanyHouse.Model;

public class Links
{
    public string charges { get; set; }
    public string exemptions { get; set; }
    public string filing_history { get; set; }
    public string insolvency { get; set; }
    public string officers { get; set; }
    public string overseas { get; set; }
    public string persons_with_significant_control { get; set; }
    public string persons_with_significant_control_statements { get; set; }
    public string registers { get; set; }
    public string self { get; set; }
    [JsonProperty(PropertyName = "uk-establishments")]
    public string uk_establishments { get; set; }
}
