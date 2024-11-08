using System.Collections.Generic;
using CluedIn.Core.Crawling;

namespace CluedIn.ExternalSearch.Providers.CompanyHouse
{
    public class CompanyHouseExternalSearchJobData : CrawlJobData
    {
        public CompanyHouseExternalSearchJobData(IDictionary<string, object> configuration)
        {
            ApiKey = GetValue<string>(configuration, Constants.KeyName.ApiKey);
            AcceptedEntityType = GetValue<string>(configuration, Constants.KeyName.AcceptedEntityType);
            CompanyHouseNumberKey = GetValue<string>(configuration, Constants.KeyName.CompanyHouseNumberKey);
            CountryKey = GetValue<string>(configuration, Constants.KeyName.CountryKey);
            OrgNameKey = GetValue<string>(configuration, Constants.KeyName.OrgNameKey);
            SkipCompanyHouseNumberEntityCodeCreation = GetValue<bool>(configuration, Constants.KeyName.SkipCompanyHouseNumberEntityCodeCreation);
            SkipCompanyHouseNameEntityCodeCreation = GetValue<bool>(configuration, Constants.KeyName.SkipCompanyHouseNameEntityCodeCreation);
        }

        public string ApiKey { get; set; }
        public string AcceptedEntityType { get; set; }
        public string CompanyHouseNumberKey { get; set; }
        public string CountryKey { get; set; }
        public string OrgNameKey { get; set; }
        public bool SkipCompanyHouseNumberEntityCodeCreation { get; set; }
        public bool SkipCompanyHouseNameEntityCodeCreation { get; set; }

        public IDictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>
            {
                { Constants.KeyName.ApiKey, ApiKey },
                { Constants.KeyName.AcceptedEntityType, AcceptedEntityType },
                { Constants.KeyName.CompanyHouseNumberKey, CompanyHouseNumberKey },
                { Constants.KeyName.CountryKey, CountryKey },
                { Constants.KeyName.OrgNameKey, OrgNameKey },
                { Constants.KeyName.SkipCompanyHouseNumberEntityCodeCreation, SkipCompanyHouseNumberEntityCodeCreation },
                { Constants.KeyName.SkipCompanyHouseNameEntityCodeCreation, SkipCompanyHouseNameEntityCodeCreation },
            };
        }
    }
}
