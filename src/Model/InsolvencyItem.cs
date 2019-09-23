using System.Collections.Generic;

namespace CluedIn.ExternalSearch.Providers.CompanyHouse.Model
{
    public class InsolvencyItem
    {
        public List<Annotation> annotations { get; set; }
        public List<AssociatedFiling> associated_filings { get; set; }
        public string barcode { get; set; }
        public string category { get; set; }
        public string date { get; set; }
        public string description { get; set; }
        public InsolvencyLinks2 links { get; set; }
        public string pages { get; set; }
        public string paper_filed { get; set; }
        public List<Resolution> resolutions { get; set; }
        public string subcategory { get; set; }
        public string transaction_id { get; set; }
        public string type { get; set; }
    }
}