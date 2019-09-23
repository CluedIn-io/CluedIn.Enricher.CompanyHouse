using System.Collections.Generic;

namespace CluedIn.ExternalSearch.Providers.CompanyHouse.Model
{
    public class ControlPersonItem
    {
        public Address address { get; set; }
        public string ceased_on { get; set; }
        public string country_of_residence { get; set; }
        public DateOfBirth date_of_birth { get; set; }
        public string etag { get; set; }
        public ControlPersonLinks links { get; set; }
        public string name { get; set; }
        public NameElements name_elements { get; set; }
        public string nationality { get; set; }
        public List<string> natures_of_control { get; set; }
        public string notified_on { get; set; }
    }
}