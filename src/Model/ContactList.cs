using System.Collections.Generic;

namespace CluedIn.ExternalSearch.Providers.CompanyHouse.Model
{
    public class ContactList
    {
        public int items_per_page { get; set; }
        public int start_index { get; set; }
        public int active_count { get; set; }
        public int inactive_count { get; set; }
        public int total_results { get; set; }
        public List<Contact> items { get; set; }
        public string etag { get; set; }
        public ContactLinks2 links { get; set; }
        public string kind { get; set; }
        public int resigned_count { get; set; }
    }
}
