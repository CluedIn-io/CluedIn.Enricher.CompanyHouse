using System.Collections.Generic;

namespace CluedIn.ExternalSearch.Providers.CompanyHouse.Model
{
    public class OfficerResponse
    {
        public string date_of_birth { get; set; }
        public List<Disqualification> disqualifications { get; set; }
        public string etag { get; set; }
        public string forename { get; set; }
        public string honours { get; set; }
        public string kind { get; set; }
        public OfficerLinks links { get; set; }
        public string nationality { get; set; }
        public string other_forenames { get; set; }
        public List<PermissionsToAct> permissions_to_act { get; set; }
        public string surname { get; set; }
        public string title { get; set; }
    }
}