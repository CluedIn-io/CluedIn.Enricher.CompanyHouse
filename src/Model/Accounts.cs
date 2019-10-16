namespace CluedIn.ExternalSearch.Providers.CompanyHouse.Model
{
    public class Accounts
    {
        public NextAccounts next_accounts { get; set; }
        public string next_made_up_to { get; set; }
        public LastAccounts last_accounts { get; set; }
        public bool overdue { get; set; }
        public string next_due { get; set; }
        public AccountingReferenceDate accounting_reference_date { get; set; }
    }
}
