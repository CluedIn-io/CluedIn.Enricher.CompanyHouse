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
    public class CompanyHouseExternalSearchProvider : ExternalSearchProviderBase, IExtendedEnricherMetadata, IConfigurableExternalSearchProvider
    {
        private static readonly EntityType[] AcceptedEntityTypes = { EntityType.Organization };

        /**********************************************************************************************************
         * CONSTRUCTORS
         **********************************************************************************************************/
        // TODO: Move Magic GUID to constants
        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyHouseExternalSearchProvider" /> class.
        /// </summary>
        public CompanyHouseExternalSearchProvider()
            : base(Constants.ProviderId, AcceptedEntityTypes)
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
            if (!Accepts(request.EntityMetaData.EntityType))
                yield break;

            var existingResults = request.GetQueryResults<CompanyNew>(this).ToList();

            Func<string, bool> idFilter = value => existingResults.Any(r => string.Equals(r.Data.company_number, value, StringComparison.InvariantCultureIgnoreCase));
            Func<string, bool> nameFilter = value => existingResults.Any(r => string.Equals(r.Data.company_name, value, StringComparison.InvariantCultureIgnoreCase));

            var entityType = request.EntityMetaData.EntityType;
            var companyHouseNumber = request.QueryParameters.GetValue(Core.Data.Vocabularies.Vocabularies.CluedInOrganization.CodesCompanyHouse, new HashSet<string>());

            var country = request.EntityMetaData.Properties.ContainsKey(Core.Data.Vocabularies.Vocabularies.CluedInOrganization.AddressCountryCode) ? request.EntityMetaData.Properties[Core.Data.Vocabularies.Vocabularies.CluedInOrganization.AddressCountryCode].ToLowerInvariant() : string.Empty;

            // TODO: Should put a filter here to only lookup UK based companies.
            if (country.Contains("uk") || country.Contains("gb"))
            {
                var organizationName = request.QueryParameters.GetValue(Core.Data.Vocabularies.Vocabularies.CluedInOrganization.OrganizationName, new HashSet<string>());

                if (!string.IsNullOrEmpty(request.EntityMetaData.Name))
                    organizationName.Add(request.EntityMetaData.Name);
                if (!string.IsNullOrEmpty(request.EntityMetaData.DisplayName))
                    organizationName.Add(request.EntityMetaData.DisplayName);

                if (organizationName != null)
                {
                    var values = organizationName.Select(NameNormalization.Normalize).ToHashSetEx();

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
            return Convert.ToBase64String(plainTextBytes);
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

            var code = new EntityCode(EntityType.Organization, GetCodeOrigin(), resultItem.Data.company_number);

            var clue = new Clue(code, context.Organization);
            clue.Data.OriginProviderDefinitionId = Id;

            PopulateMetadata(clue.Data.EntityData, resultItem.Data);
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
            return CreateMetadata(resultItem);
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

            PopulateMetadata(metadata, resultItem.Data);

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
            var code = new EntityCode(EntityType.Organization, GetCodeOrigin(), resultCompany.company_number);

            metadata.EntityType = EntityType.Organization;

            metadata.OriginEntityCode = code;
            metadata.Name = resultCompany.original_query_name.PrintIfAvailable();
            if (!string.IsNullOrEmpty(resultCompany.company_name))
                metadata.Codes.Add(new EntityCode(EntityType.Organization, GetCodeOrigin(), resultCompany.company_name));

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
                PopulateOrgAddressMetadata(metadata, CompanyHouseVocabulary.Organization.Address, resultCompany);
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
            var code = new EntityCode(EntityType.Person, GetCodeOrigin(), resultItem.regNumber);

            metadata.EntityType = EntityType.Person;
            metadata.Name = resultItem.name.PrintIfAvailable();
            metadata.OriginEntityCode = code;

            metadata.Properties[CompanyHouseVocabulary.Person.Name] = resultItem.name.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Person.Officer_role] = resultItem.officer_role.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Person.Appointed_on] = resultItem.appointed_on.PrintIfAvailable();

            if (resultItem.address != null)
            {
                PopulatePersonAddressMetadata(metadata, CompanyHouseVocabulary.Person.Address, resultItem.address);
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

        public IEnumerable<EntityType> Accepts(IDictionary<string, object> config, IProvider provider)
        {
            return AcceptedEntityTypes;
        }

        public IEnumerable<IExternalSearchQuery> BuildQueries(ExecutionContext context, IExternalSearchRequest request, IDictionary<string, object> config, IProvider provider)
        {
            return BuildQueries(context, request);
        }

        public IEnumerable<IExternalSearchQueryResult> ExecuteSearch(ExecutionContext context, IExternalSearchQuery query, IDictionary<string, object> config, IProvider provider)
        {
            return ExecuteSearch(context, query);
        }

        public IEnumerable<Clue> BuildClues(ExecutionContext context, IExternalSearchQuery query, IExternalSearchQueryResult result, IExternalSearchRequest request, IDictionary<string, object> config, IProvider provider)
        {
            return BuildClues(context, query, result, request);
        }

        public IEntityMetadata GetPrimaryEntityMetadata(ExecutionContext context, IExternalSearchQueryResult result, IExternalSearchRequest request, IDictionary<string, object> config, IProvider provider)
        {
            return GetPrimaryEntityMetadata(context, result, request);
        }

        public IPreviewImage GetPrimaryEntityPreviewImage(ExecutionContext context, IExternalSearchQueryResult result, IExternalSearchRequest request, IDictionary<string, object> config, IProvider provider)
        {
            return GetPrimaryEntityPreviewImage(context, result, request);
        }

        public string Icon { get; } = Constants.Icon;
        public string Domain { get; } = Constants.Domain;
        public string About { get; } = Constants.About;

        public AuthMethods AuthMethods { get; } = Constants.AuthMethods;
        public IEnumerable<Control> Properties { get; } = Constants.Properties;
        public Guide Guide { get; } = Constants.Guide;
        public IntegrationType Type { get; } = Constants.IntegrationType;
    }
}
