using System;
using System.Collections.Generic;
using CluedIn.Core.Data.Relational;
using CluedIn.Core.Providers;

namespace CluedIn.ExternalSearch.Providers.CompanyHouse
{
    public static class Constants
    {
        public const string ComponentName = "CompanyHouse";
        public const string ProviderName = "Companies House";
        public static readonly Guid ProviderId = new Guid("{2A9E52AE-425B-4351-8AF5-6D374E8CC1A5}");

        public static string About { get; set; } =
            "Companies House is an enricher which provides information on UK companies";

        public static string Icon { get; set; } = "Resources.companyhouse.svg";
        public static string Domain { get; set; } = "https://www.gov.uk/government/organisations/companies-house";

        public static AuthMethods AuthMethods { get; set; } = new AuthMethods()
        {
            token = new List<Control>
            {
                new Control() { displayName = "API Key", type = "password", isRequired = true, name = KeyName.ApiKey },
                new Control()
                {
                    displayName = "Accepted Entity Type",
                    type = "input",
                    isRequired = false,
                    name = KeyName.AcceptedEntityType
                },
                new Control()
                {
                    displayName = "Companies House Number vocab key",
                    type = "input",
                    isRequired = false,
                    name = KeyName.CompanyHouseNumberKey
                },
                new Control()
                {
                    displayName = "Country vocab key",
                    type = "input",
                    isRequired = false,
                    name = KeyName.CountryKey
                },
                new Control()
                {
                    displayName = "Organization Name vocab key",
                    type = "input",
                    isRequired = false,
                    name = KeyName.OrgNameKey
                }
            }
        };

        public static IEnumerable<Control> Properties { get; set; } = new List<Control>
        {
            // NOTE: Leaving this commented as an example - BF
            //new()
            //{
            //    displayName = "Some Data",
            //    type = "input",
            //    isRequired = true,
            //    name = "someData"
            //}
        };

        public static Guide Guide { get; set; } = null;
        public static IntegrationType IntegrationType { get; set; } = IntegrationType.Enrichment;

        public struct KeyName
        {
            public const string ApiKey = nameof(ApiKey);
            public const string AcceptedEntityType = nameof(AcceptedEntityType);
            public const string CompanyHouseNumberKey = nameof(CompanyHouseNumberKey);
            public const string CountryKey = nameof(CountryKey);
            public const string OrgNameKey = nameof(OrgNameKey);
        }
    }
}
