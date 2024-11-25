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
            Token = new List<Control>
            {
                new Control() {
                    DisplayName = "API Key",
                    Type = "password",
                    IsRequired = true,
                    Name = KeyName.ApiKey,
                    Help = "The key to authenticate access to the Companies House API"
                },
                new Control()
                {
                    DisplayName = "Accepted Entity Type",
                    Type = "entityTypeSelector",
                    IsRequired = true,
                    Name = KeyName.AcceptedEntityType,
                    Help = "The entity type that defines the golden records you want to enrich (e.g., /Organization)."
                },
                new Control()
                {
                    DisplayName = "Companies House Number Vocabulary Key",
                    Type = "vocabularyKeySelector",
                    IsRequired = false,
                    Name = KeyName.CompanyHouseNumberKey,
                    Help = "The vocabulary key that contains the Company House Number of companies you want to enrich (e.g., organization.companyshousenumber)"
                },
                new Control()
                {
                    DisplayName = "Country Vocabulary Key",
                    Type = "vocabularyKeySelector",
                    IsRequired = false,
                    Name = KeyName.CountryKey,
                    Help = "The vocabulary key that contains the countries of companies you want to enrich (e.g., organization.country)."
                },
                new Control()
                {
                    DisplayName = "Organization Name Vocabulary Key",
                    Type = "vocabularyKeySelector",
                    IsRequired = false,
                    Name = KeyName.OrgNameKey,
                    Help = "The vocabulary key that contains the names of companies you want to enrich (e.g., organization.name)."
                },
                new()
                {
                    DisplayName = "Skip Entity Code Creation (Company House Number)",
                    Type = "checkbox",
                    IsRequired = false,
                    Name =  KeyName.SkipCompanyHouseNumberEntityCodeCreation,
                    Help = "Toggle to control the creation of new entity codes using the Company House Number."
                },
                new()
                {
                    DisplayName = "Skip Entity Code Creation (Company Name)",
                    Type = "checkbox",
                    IsRequired = false,
                    Name =  KeyName.SkipCompanyHouseNameEntityCodeCreation,
                    Help = "Toggle to control the creation of new entity codes using the Company Name."
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
            public const string SkipCompanyHouseNumberEntityCodeCreation = nameof(SkipCompanyHouseNumberEntityCodeCreation);
            public const string SkipCompanyHouseNameEntityCodeCreation = nameof(SkipCompanyHouseNameEntityCodeCreation);
        }
    }
}
