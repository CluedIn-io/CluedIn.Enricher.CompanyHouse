namespace CluedIn.ExternalSearch.Providers.CompanyHouse.Model
{
    public class ConfirmationStatement
    {
        public string next_made_up_to { get; set; }
        public string last_made_up_to { get; set; }
        public string next_due { get; set; }
        public bool overdue { get; set; }
    }
}