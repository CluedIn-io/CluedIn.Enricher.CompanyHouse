using System.Collections.Generic;

namespace CluedIn.ExternalSearch.Providers.CompanyHouse.Model
{
    public class Case
    {
        public List<Date> dates { get; set; }
        public InsolvencyLinks links { get; set; }
        public List<string> notes { get; set; }
        public string number { get; set; }
        public List<Practitioner> practitioners { get; set; }
        public string type { get; set; }
    }
}
