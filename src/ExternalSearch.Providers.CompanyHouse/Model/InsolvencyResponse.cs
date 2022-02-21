using System.Collections.Generic;

namespace CluedIn.ExternalSearch.Providers.CompanyHouse.Model
{
    public class InsolvencyResponse
    {
        public List<Case> cases { get; set; }
        public string etag { get; set; }
        public List<object> status { get; set; }
    }
}
