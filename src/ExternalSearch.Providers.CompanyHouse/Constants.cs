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
        public static readonly string Instruction = $$"""
            [
              {
                "type": "bulleted-list",
                "children": [
                  {
                    "type": "list-item",
                    "children": [
                      {
                        "text": "Add the {{Core.Constants.DomainLabels.EntityType.ToLower()}} to specify the golden records you want to enrich. Only golden records belonging to that {{Core.Constants.DomainLabels.EntityType.ToLower()}} will be enriched."
                      }
                    ]
                  },
                  {
                    "type": "list-item",
                    "children": [
                      {
                        "text": "Add the vocabulary keys to provide the input for the enricher to search for additional information. For example, if you provide the website vocabulary key for the Web enricher, it will use specific websites to look for information about companies. In some cases, vocabulary keys are not required. If you don't add them, the enricher will use default vocabulary keys."
                      }
                    ]
                  },
                  {
                    "type": "list-item",
                    "children": [
                      {
                        "text": "Add the API key to enable the enricher to retrieve information from a specific API. For example, the Vatlayer enricher requires an access key to authenticate with the Vatlayer API."
                      }
                    ]
                  }
                ]
              }
            ]
            """;

        public static string About { get; set; } =
            "Companies House is an enricher which provides information on UK companies";

        public static string Icon { get; set; } = "Resources.companyhouse.svg";
        public static string Domain { get; set; } = "https://www.gov.uk/government/organisations/companies-house";

        public static AuthMethods AuthMethods { get; set; } = new AuthMethods()
        {
            Token = new List<Control>
            {
                new()
                {
                    DisplayName = "API Key",
                    Type = "password",
                    IsRequired = true,
                    Name = KeyName.ApiKey,
                    Help = "The key to authenticate access to the Companies House API",
                    ValidationRules = new List<Dictionary<string, string>>()
                    {
                        new() {
                            { "regex", "\\s" },
                            { "message", "Spaces are not allowed" }
                        }
                    },
                },
                new()
                {
                    DisplayName = $"Accepted {Core.Constants.DomainLabels.EntityType}",
                    Type = "entityTypeSelector",
                    IsRequired = true,
                    Name = KeyName.AcceptedEntityType,
                    Help = $"The {Core.Constants.DomainLabels.EntityType.ToLower()} that defines the golden records you want to enrich (e.g., /Organization)."
                },
                new()
                {
                    DisplayName = "Companies House Number Vocabulary Key",
                    Type = "vocabularyKeySelector",
                    IsRequired = false,
                    Name = KeyName.CompanyHouseNumberKey,
                    Help = "The vocabulary key that contains the Company House Number of companies you want to enrich (e.g., organization.companyshousenumber)"
                },
                new()
                {
                    DisplayName = "Country Vocabulary Key",
                    Type = "vocabularyKeySelector",
                    IsRequired = false,
                    Name = KeyName.CountryKey,
                    Help = "The vocabulary key that contains the countries of companies you want to enrich (e.g., organization.country)."
                },
                new()
                {
                    DisplayName = "Organization Name Vocabulary Key",
                    Type = "vocabularyKeySelector",
                    IsRequired = false,
                    Name = KeyName.OrgNameKey,
                    Help = "The vocabulary key that contains the names of companies you want to enrich (e.g., organization.name)."
                },
                new()
                {
                    DisplayName = $"Skip {Core.Constants.DomainLabels.EntityCode} Creation (Company House Number)",
                    Type = "checkbox",
                    IsRequired = false,
                    Name =  KeyName.SkipCompanyHouseNumberEntityCodeCreation,
                    Help = $"Toggle to control the creation of new {Core.Constants.DomainLabels.EntityCodes.ToLower()} using the Company House Number."
                },
                new()
                {
                    DisplayName = $"Skip {Core.Constants.DomainLabels.EntityCode} Creation (Company Name)",
                    Type = "checkbox",
                    IsRequired = false,
                    Name =  KeyName.SkipCompanyHouseNameEntityCodeCreation,
                    Help = $"Toggle to control the creation of new {Core.Constants.DomainLabels.EntityCodes.ToLower()} using the Company Name."
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

        public static Guide Guide { get; set; } = new Guide
        {
            Instructions = Instruction
        };
        public static IntegrationType IntegrationType { get; set; } = IntegrationType.Enrichment;

        public struct KeyName
        {
            public const string ApiKey = "apiKey";
            public const string AcceptedEntityType = "acceptedEntityType";
            public const string CompanyHouseNumberKey = "companyHouseNumberKey";
            public const string CountryKey = "countryKey";
            public const string OrgNameKey = "orgNameKey";
            public const string SkipCompanyHouseNumberEntityCodeCreation = "skipCompanyHouseNumberEntityCodeCreation";
            public const string SkipCompanyHouseNameEntityCodeCreation = "skipCompanyHouseNameEntityCodeCreation";
        }
    }
}
