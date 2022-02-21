using System.Collections.Generic;

namespace CluedIn.ExternalSearch.Providers.CompanyHouse.Model
{
    public class CompanyAndContacts
    {
        public CompanyNew Company { get; set; }

        public List<Contact> contact { get; set; }
    }
}
