using System.Collections.Generic;
using CluedIn.Core.Crawling;

namespace CluedIn.ExternalSearch.Providers.CompanyHouse
{
    public class CompanyHouseExternalSearchJobData : CrawlJobData
    {
        public CompanyHouseExternalSearchJobData(IDictionary<string, object> configuration)
        {
        }

        public IDictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>();
        }
        
    }
}
