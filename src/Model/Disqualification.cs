using System.Collections.Generic;

namespace CluedIn.ExternalSearch.Providers.CompanyHouse.Model
{
    public class Disqualification
    {
        public OfficerAddress address { get; set; }
        public string case_identifier { get; set; }
        public List<string> company_names { get; set; }
        public string court_name { get; set; }
        public string disqualification_type { get; set; }
        public string disqualified_from { get; set; }
        public string disqualified_until { get; set; }
        public string heard_on { get; set; }
        public LastVariation last_variation { get; set; }
        public Reason reason { get; set; }
        public string undertaken_on { get; set; }
    }
}