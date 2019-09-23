using System.Collections.Generic;

namespace CluedIn.ExternalSearch.Providers.CompanyHouse.Model
{
    public class AppointmentResponse
    {
        public DateOfBirth date_of_birth { get; set; }
        public string etag { get; set; }
        public string is_corporate_officer { get; set; }
        public List<AppointmentItem> items { get; set; }
        public string items_per_page { get; set; }
        public string kind { get; set; }
        public AppointmentLinks2 links { get; set; }
        public string name { get; set; }
        public string start_index { get; set; }
        public string total_results { get; set; }
    }
}