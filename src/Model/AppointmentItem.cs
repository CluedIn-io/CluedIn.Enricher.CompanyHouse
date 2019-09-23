using System.Collections.Generic;

namespace CluedIn.ExternalSearch.Providers.CompanyHouse.Model
{
    public class AppointmentItem
    {
        public AppointmentAddress address { get; set; }
        public string appointed_before { get; set; }
        public string appointed_on { get; set; }
        public AppointedTo appointed_to { get; set; }
        public string country_of_residence { get; set; }
        public List<FormerName> former_names { get; set; }
        public Identification identification { get; set; }
        public string is_pre_1992_appointment { get; set; }
        public AppointmentLinks links { get; set; }
        public string name { get; set; }
        public AppointmentNameElements name_elements { get; set; }
        public string nationality { get; set; }
        public string occupation { get; set; }
        public string officer_role { get; set; }
        public string resigned_on { get; set; }
    }
}