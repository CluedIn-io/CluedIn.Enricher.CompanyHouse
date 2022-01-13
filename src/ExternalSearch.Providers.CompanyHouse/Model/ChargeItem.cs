using System.Collections.Generic;

namespace CluedIn.ExternalSearch.Providers.CompanyHouse.Model
{
    public class ChargeItem
    {
        public string acquired_on { get; set; }
        public string assets_ceased_released { get; set; }
        public string charge_code { get; set; }
        public string charge_number { get; set; }
        public Classification classification { get; set; }
        public string covering_instrument_date { get; set; }
        public string created_on { get; set; }
        public string delivered_on { get; set; }
        public string etag { get; set; }
        public string id { get; set; }
        public List<InsolvencyCas> insolvency_cases { get; set; }
        public ChargeLinks2 links { get; set; }
        public string more_than_four_persons_entitled { get; set; }
        public Particulars particulars { get; set; }
        public List<PersonsEntitled> persons_entitled { get; set; }
        public string resolved_on { get; set; }
        public string satisfied_on { get; set; }
        public ScottishAlterations scottish_alterations { get; set; }
        public SecuredDetails secured_details { get; set; }
        public string status { get; set; }
        public List<Transaction> transactions { get; set; }
    }
}
