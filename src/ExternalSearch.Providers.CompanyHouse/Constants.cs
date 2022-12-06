using System;
using System.Collections.Generic;
using CluedIn.Core.Data.Relational;
using CluedIn.Core.Providers;

namespace CluedIn.ExternalSearch.Providers.CompanyHouse
{
    public static class Constants
    {
        public const string ComponentName = "CompanyHouse";
        public const string ProviderName = "Company House";
        public static readonly Guid ProviderId = new Guid("{2A9E52AE-425B-4351-8AF5-6D374E8CC1A5}");

        public struct KeyName
        {
            public const string AcceptedEntityType = "acceptedEntityType";
            public const string CompanyHouseNumberKey = "companyHouseNumberKey";
            public const string CountryKey = "countryKey";
            public const string OrgNameKey = "orgNameKey";
        }

        public static string About { get; set; } = "Company House is an enricher which provides information on UK companies";
        public static string Icon { get; set; } = "Resources.companyhouse.svg";
        public static string Domain { get; set; } = "https://www.gov.uk/government/organisations/companies-house";

        public static AuthMethods AuthMethods { get; set; } = new AuthMethods
        {
            token = new List<Control>()
            {
                new Control()
                {
                    displayName = "Accepted Entity Type",
                    type = "input",
                    isRequired = false,
                    name = KeyName.AcceptedEntityType
                },
                new Control()
                {
                    displayName = "Company House Number vocab key",
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

        public static IEnumerable<Control> Properties { get; set; } = new List<Control>()
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
    }
}
