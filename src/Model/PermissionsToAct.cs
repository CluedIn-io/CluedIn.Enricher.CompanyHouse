using System.Collections.Generic;

namespace CluedIn.ExternalSearch.Providers.CompanyHouse.Model
{
    public class PermissionsToAct
    {
        public List<string> company_names { get; set; }
        public string court_name { get; set; }
        public string expires_on { get; set; }
        public string granted_on { get; set; }
    }
}
