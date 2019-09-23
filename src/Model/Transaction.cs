namespace CluedIn.ExternalSearch.Providers.CompanyHouse.Model
{
    public class Transaction
    {
        public string delivered_on { get; set; }
        public string filing_type { get; set; }
        public string insolvency_case_number { get; set; }
        public ChargeLinks3 links { get; set; }
        public string transaction_id { get; set; }
    }
}