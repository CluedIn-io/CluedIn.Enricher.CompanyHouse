// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClearBitExternalSearchProvider.cs" company="Clued In">
//   Copyright Clued In
// </copyright>
// <summary>
//   Defines the ClearBitExternalSearchProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using CluedIn.Core;
using CluedIn.Core.Data;
using CluedIn.Core.Data.Parts;
using CluedIn.Core.Data.Relational;
using CluedIn.Core.ExternalSearch;
using CluedIn.Core.Providers;
using CluedIn.ExternalSearch.Providers.CompanyHouse.Vocabularies;
using CluedIn.ExternalSearch.Filters;
using CluedIn.Crawling.Helpers;
using CluedIn.ExternalSearch.Providers.CompanyHouse.Model;
using EntityType = CluedIn.Core.Data.EntityType;

namespace CluedIn.ExternalSearch.Providers.CompanyHouse
{
    /// <summary>The clear bit external search provider.</summary>
    /// <seealso cref="CluedIn.ExternalSearch.ExternalSearchProviderBase" />
    [Id("2A9E52AE-425B-4351-8AF5-6D374E8CC1A5")]
    [Name("Company House Enricher")]
    [EnrichSource("www.companyhouse.com")]
    [LookupEntityTypes("/Organization")]
    [ReturnedEntityTypes("/Organization", "/Person")]
    [LookupVocabularies("CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization")]
    [ReturnedVocabularies("CompanyHouseVocabulary.Organization", "CompanyHouseVocabulary.Person")]
    [LookupVocabulariesKeys(
        "CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.CodesCompanyHouse",
        "CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.AddressCountryCode",
        "CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.OrganizationName"
        )
    ]
    [ReturnedVocabulariesKeys(
            "CompanyHouseVocabulary.Organization.CompanyNumber",
            "CompanyHouseVocabulary.Organization.Charges",
            "CompanyHouseVocabulary.Organization.CompanyStatus",
            "CompanyHouseVocabulary.Organization.Type",
            "CompanyHouseVocabulary.Organization.Jurisdiction",
            "CompanyHouseVocabulary.Organization.Has_been_liquidated",
            "CompanyHouseVocabulary.Organization.Has_insolvency_history",
            "CompanyHouseVocabulary.Organization.Registered_office_is_in_dispute",
            "CompanyHouseOrgAddressVocabulary.AddressLine1",
            "CompanyHouseOrgAddressVocabulary.AddressLine2",
            "CompanyHouseOrgAddressVocabulary.Locality",
            "CompanyHouseOrgAddressVocabulary.PostCode",
            "CompanyHouseVocabulary.Person.Name",
            "CompanyHouseVocabulary.Person.Officer_role",
            "CompanyHouseVocabulary.Person.Appointed_on",
            "CompanyHouseVocabulary.Person.Date_of_birth",
            "CompanyHouseVocabulary.Person.Country_of_residence",
            "CompanyHouseVocabulary.Person.Occupation",
            "CompanyHouseVocabulary.Person.Nationality",
            "CompanyHouseVocabulary.Person.Resigned_on",
            "CompanyHousePersonAddressVocabulary.CareOf",
            "CompanyHousePersonAddressVocabulary.Region",
            "CompanyHousePersonAddressVocabulary.Postal_code",
            "CompanyHousePersonAddressVocabulary.Premises",
            "CompanyHousePersonAddressVocabulary.Country",
            "CompanyHousePersonAddressVocabulary.Locality",
            "CompanyHousePersonAddressVocabulary.AddressLine1",
            "CompanyHousePersonAddressVocabulary.AddressLine2"
        )
    ]
    public class CompanyHouseExternalSearchProvider : ExternalSearchProviderBase, IExtendedEnricherMetadata
    {
        /**********************************************************************************************************
         * CONSTRUCTORS
         **********************************************************************************************************/
        // TODO: Move Magic GUID to constants
        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyHouseExternalSearchProvider" /> class.
        /// </summary>
        public CompanyHouseExternalSearchProvider()
            : base(new Guid("{2A9E52AE-425B-4351-8AF5-6D374E8CC1A5}"), EntityType.Organization)
        {
         
        }

        /**********************************************************************************************************
         * METHODS
         **********************************************************************************************************/

        /// <summary>Builds the queries.</summary>
        /// <param name="context">The context.</param>
        /// <param name="request">The request.</param>
        /// <returns>The search queries.</returns>
        public override IEnumerable<IExternalSearchQuery> BuildQueries(ExecutionContext context, IExternalSearchRequest request)
        {
            if (!this.Accepts(request.EntityMetaData.EntityType))
                yield break;

            var existingResults = request.GetQueryResults<CompanyNew>(this).ToList();

            Func<string, bool> idFilter = value => existingResults.Any(r => string.Equals(r.Data.company_number, value, StringComparison.InvariantCultureIgnoreCase));
            Func<string, bool> nameFilter = value => existingResults.Any(r => string.Equals(r.Data.company_name, value, StringComparison.InvariantCultureIgnoreCase));

            var entityType = request.EntityMetaData.EntityType;
            var companyHouseNumber = request.QueryParameters.GetValue(CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.CodesCompanyHouse, new HashSet<string>());

            var country = request.EntityMetaData.Properties.ContainsKey(CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.AddressCountryCode) ? request.EntityMetaData.Properties[CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.AddressCountryCode].ToLowerInvariant() : string.Empty;

            // TODO: Should put a filter here to only lookup UK based companies.
            if (country.Contains("uk") || country.Contains("gb"))
            {
                var organizationName = request.QueryParameters.GetValue(CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.OrganizationName, new HashSet<string>());

                if (!string.IsNullOrEmpty(request.EntityMetaData.Name))
                    organizationName.Add(request.EntityMetaData.Name);
                if (!string.IsNullOrEmpty(request.EntityMetaData.DisplayName))
                    organizationName.Add(request.EntityMetaData.DisplayName);

                if (organizationName != null)
                {
                    var values = organizationName.Select(NameNormalization.Normalize).ToHashSet();

                    foreach (var value in values.Where(v => !nameFilter(v)))
                        yield return new ExternalSearchQuery(this, entityType, ExternalSearchQueryParameter.Name, value);
                }
            }

            foreach (var value in companyHouseNumber.Where(v => !idFilter(v)))
                yield return new ExternalSearchQuery(this, entityType, ExternalSearchQueryParameter.Identifier, value);
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>Executes the search.</summary>
        /// <param name="context">The context.</param>
        /// <param name="query">The query.</param>
        /// <returns>The results.</returns>
        public override IEnumerable<IExternalSearchQueryResult> ExecuteSearch(ExecutionContext context, IExternalSearchQuery query)
        {
            var name = query.QueryParameters.ContainsKey(ExternalSearchQueryParameter.Name) ? query.QueryParameters[ExternalSearchQueryParameter.Name].FirstOrDefault() : null;
            if (string.IsNullOrEmpty(name))
                yield break;

            var client = new CompanyHouseClient();
            var companies = client.GetCompanies(name);
            if (companies == null)
                yield break;

            foreach (var companyResult in companies)
            {
                var company = new CompanyNew();

                company = client.GetCompany(companyResult.company_number);

                yield return new ExternalSearchQueryResult<CompanyNew>(query, company);
            }

        }

        /// <summary>Builds the clues.</summary>
        /// <param name="context">The context.</param>
        /// <param name="query">The query.</param>
        /// <param name="result">The result.</param>
        /// <param name="request">The request.</param>
        /// <returns>The clues.</returns>
        public override IEnumerable<Clue> BuildClues(ExecutionContext context, IExternalSearchQuery query, IExternalSearchQueryResult result, IExternalSearchRequest request)
        {
            var resultItem = result.As<CompanyNew>();

            var code = new EntityCode(EntityType.Organization, this.GetCodeOrigin(), resultItem.Data.company_number);

            var clue = new Clue(code, context.Organization);
            clue.Data.OriginProviderDefinitionId = this.Id;

            this.PopulateMetadata(clue.Data.EntityData, resultItem.Data);
            yield return clue;
        }

        /// <summary>Gets the primary entity metadata.</summary>
        /// <param name="context">The context.</param>
        /// <param name="result">The result.</param>
        /// <param name="request">The request.</param>
        /// <returns>The primary entity metadata.</returns>
        public override IEntityMetadata GetPrimaryEntityMetadata(ExecutionContext context, IExternalSearchQueryResult result, IExternalSearchRequest request)
        {
            var resultItem = result.As<CompanyNew>();
            return this.CreateMetadata(resultItem);
        }

        /// <summary>Gets the preview image.</summary>
        /// <param name="context">The context.</param>
        /// <param name="result">The result.</param>
        /// <param name="request">The request.</param>
        /// <returns>The preview image.</returns>
        public override IPreviewImage GetPrimaryEntityPreviewImage(ExecutionContext context, IExternalSearchQueryResult result, IExternalSearchRequest request)
        {
            return null;
        }

        /// <summary>Creates the metadata.</summary>
        /// <param name="resultItem">The result item.</param>
        /// <returns>The metadata.</returns>
        private IEntityMetadata CreateMetadata(IExternalSearchQueryResult<CompanyNew> resultItem)
        {
            var metadata = new EntityMetadataPart();

            this.PopulateMetadata(metadata, resultItem.Data);

            return metadata;
        }

        /// <summary>Gets the code origin.</summary>
        /// <returns>The code origin</returns>
        private CodeOrigin GetCodeOrigin()
        {
            return CodeOrigin.CluedIn.CreateSpecific("companyHouse");
        }

        /// <summary>Populates the metadata.</summary>
        /// <param name="metadata">The metadata.</param>
        /// <param name="resultCompany">The result item.</param>
        private void PopulateMetadata(IEntityMetadata metadata, CompanyNew resultCompany)
        {
            var code = new EntityCode(EntityType.Organization, this.GetCodeOrigin(), resultCompany.company_number);

            metadata.EntityType = EntityType.Organization;

            metadata.OriginEntityCode = code;
            metadata.Name = resultCompany.original_query_name.PrintIfAvailable();
            if (!string.IsNullOrEmpty(resultCompany.company_name))
                metadata.Codes.Add(new EntityCode(EntityType.Organization, this.GetCodeOrigin(), resultCompany.company_name));

            metadata.DisplayName = resultCompany.company_name.PrintIfAvailable();

            metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = resultCompany.company_number.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.Charges] = resultCompany.has_charges.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.CompanyStatus] = resultCompany.company_status.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.Type] = resultCompany.type.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.Jurisdiction] = resultCompany.jurisdiction.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.Has_been_liquidated] = resultCompany.has_been_liquidated.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.Has_insolvency_history] = resultCompany.has_insolvency_history.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.Registered_office_is_in_dispute] = resultCompany.registered_office_is_in_dispute.PrintIfAvailable();

            if (!string.IsNullOrEmpty(resultCompany.date_of_creation))
            {
                DateTimeOffset createdDate;

                if (DateTimeOffset.TryParse(resultCompany.date_of_creation, out createdDate))
                {
                    metadata.CreatedDate = createdDate;
                }

                metadata.Properties[CompanyHouseVocabulary.Organization.DateOfCreation] = resultCompany.date_of_creation;
            }

            if (resultCompany.registered_office_address != null)
                this.PopulateOrgAddressMetadata(metadata, CompanyHouseVocabulary.Organization.Address, resultCompany);
        }

        private void PopulateOrgAddressMetadata(IEntityMetadata metadata, CompanyHouseOrgAddressVocabulary vocab, CompanyNew resultCompany)
        {
            metadata.Properties[vocab.AddressLine1] = resultCompany.registered_office_address.address_line_1.PrintIfAvailable();
            metadata.Properties[vocab.AddressLine2] = resultCompany.registered_office_address.address_line_2.PrintIfAvailable();
            metadata.Properties[vocab.Locality] = resultCompany.registered_office_address.locality.PrintIfAvailable();
            metadata.Properties[vocab.PostCode] = resultCompany.registered_office_address.postal_code.PrintIfAvailable();
        }


        private void PopulateContactMetadata(IEntityMetadata metadata, Contact resultItem)
        {
            var code = new EntityCode(EntityType.Person, this.GetCodeOrigin(), resultItem.regNumber);

            metadata.EntityType = EntityType.Person;
            metadata.Name = resultItem.name.PrintIfAvailable();
            metadata.OriginEntityCode = code;

            metadata.Properties[CompanyHouseVocabulary.Person.Name] = resultItem.name.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Person.Officer_role] = resultItem.officer_role.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Person.Appointed_on] = resultItem.appointed_on.PrintIfAvailable();

            if (resultItem.address != null)
            {
                this.PopulatePersonAddressMetadata(metadata, CompanyHouseVocabulary.Person.Address, resultItem.address);
            }

            if (resultItem.date_of_birth != null)
            {
                metadata.Properties[CompanyHouseVocabulary.Person.Date_of_birth] = $"{resultItem.date_of_birth.year}.{resultItem.date_of_birth.month}.1";
            }

            metadata.Properties[CompanyHouseVocabulary.Person.Country_of_residence] = resultItem.country_of_residence.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Person.Occupation] = resultItem.occupation.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = resultItem.nationality.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Person.Resigned_on] = resultItem.resigned_on.PrintIfAvailable();
        }

        private void PopulatePersonAddressMetadata(IEntityMetadata metadata, CompanyHousePersonAddressVocabulary vocab, ContactAddress address)
        {
            metadata.Properties[vocab.CareOf] = address.care_of.PrintIfAvailable();
            metadata.Properties[vocab.Region] = address.region.PrintIfAvailable();
            metadata.Properties[vocab.Postal_code] = address.postal_code.PrintIfAvailable();
            metadata.Properties[vocab.Premises] = address.premises.PrintIfAvailable();
            metadata.Properties[vocab.Country] = address.country.PrintIfAvailable();
            metadata.Properties[vocab.Locality] = address.locality.PrintIfAvailable();
            metadata.Properties[vocab.AddressLine1] = address.address_line_1.PrintIfAvailable();
            metadata.Properties[vocab.AddressLine2] = address.address_line_2.PrintIfAvailable();
        }

        public string Icon { get; } = "Resources.companyhouse.jpg";
        public string Domain { get; } = "https://www.gov.uk/government/organisations/companies-house";
        public string About { get; } = "Company house is an enricher which providers information on UK companies";
        public AuthMethods AuthMethods { get; } = null;
        public IEnumerable<Control> Properties { get; } = null;
        public Guide Guide { get; } = null;
        public IntegrationType Type { get; } = IntegrationType.Cloud;
    }
}
