namespace CluedIn.ExternalSearch.Providers.CompanyHouse.Model
{
    public class Registers
    {
        public Directors directors { get; set; }
        public LlpMembers llp_members { get; set; }
        public LlpUsualResidentialAddress llp_usual_residential_address { get; set; }
        public Members members { get; set; }
        public PersonsWithSignificantControl persons_with_significant_control { get; set; }
        public Secretaries secretaries { get; set; }
        public UsualResidentialAddress usual_residential_address { get; set; }
    }
}
