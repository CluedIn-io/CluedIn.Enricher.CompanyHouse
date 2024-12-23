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
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using CluedIn.Core;
using CluedIn.Core.Connectors;
using CluedIn.Core.Data;
using CluedIn.Core.Data.Parts;
using CluedIn.Core.Data.Relational;
using CluedIn.Core.ExternalSearch;
using CluedIn.Core.Providers;
using CluedIn.Crawling.Helpers;
using CluedIn.ExternalSearch.Filters;
using CluedIn.ExternalSearch.Providers.CompanyHouse.Model;
using CluedIn.ExternalSearch.Providers.CompanyHouse.Vocabularies;
using RestSharp;
using EntityType = CluedIn.Core.Data.EntityType;

namespace CluedIn.ExternalSearch.Providers.CompanyHouse
{
    /// <summary>The clear bit external search provider.</summary>
    /// <seealso cref="CluedIn.ExternalSearch.ExternalSearchProviderBase" />
    public class CompanyHouseExternalSearchProvider : ExternalSearchProviderBase, IExtendedEnricherMetadata,
        IConfigurableExternalSearchProvider, IExternalSearchProviderWithVerifyConnection
    {
        private static readonly EntityType[] DefaultAcceptedEntityTypes = { };

        /**********************************************************************************************************
         * CONSTRUCTORS
         **********************************************************************************************************/
        // TODO: Move Magic GUID to constants
        /// <summary>
        ///     Initializes a new instance of the <see cref="CompanyHouseExternalSearchProvider" /> class.
        /// </summary>
        public CompanyHouseExternalSearchProvider()
            : base(Constants.ProviderId, DefaultAcceptedEntityTypes)
        {
        }

        public IEnumerable<EntityType> Accepts(IDictionary<string, object> config, IProvider provider) => Accepts(config);

        private IEnumerable<EntityType> Accepts(IDictionary<string, object> config)
        {
            if (config.TryGetValue(Constants.KeyName.AcceptedEntityType, out var acceptedEntityTypeObj) && acceptedEntityTypeObj is string acceptedEntityType && !string.IsNullOrWhiteSpace(acceptedEntityType))
            {
                // If configured, only accept the configured entity types
                return new EntityType[] { acceptedEntityType };
            }

            // Fallback to default accepted entity types
            return DefaultAcceptedEntityTypes;
        }

        private bool Accepts(IDictionary<string, object> config, EntityType entityTypeToEvaluate)
        {
            var configurableAcceptedEntityTypes = this.Accepts(config).ToArray();

            return configurableAcceptedEntityTypes.Any(entityTypeToEvaluate.Is);
        }

        public IEnumerable<IExternalSearchQuery> BuildQueries(ExecutionContext context, IExternalSearchRequest request,
            IDictionary<string, object> config, IProvider provider)
        {
            return InternalBuildQueries(context, request, config);
        }

        public IEnumerable<IExternalSearchQueryResult> ExecuteSearch(ExecutionContext context, IExternalSearchQuery query,
            IDictionary<string, object> config, IProvider provider)
        {
            var jobData = new CompanyHouseExternalSearchJobData(config);

            var name = query.QueryParameters.ContainsKey(ExternalSearchQueryParameter.Name)
                ? query.QueryParameters[ExternalSearchQueryParameter.Name].FirstOrDefault()
                : null;

            if (string.IsNullOrEmpty(name))
            {
                yield break;
            }

            var client = new CompanyHouseClient(jobData);
            var companies = client.GetCompanies(name);
            if (companies == null)
            {
                yield break;
            }

            foreach (var companyResult in companies)
            {
                var company = client.GetCompany(companyResult.company_number);
                yield return new ExternalSearchQueryResult<CompanyNew>(query, company);
            }
        }

        public IEnumerable<Clue> BuildClues(ExecutionContext context, IExternalSearchQuery query,
            IExternalSearchQueryResult result, IExternalSearchRequest request, IDictionary<string, object> config,
            IProvider provider)
        {
            var resultItem = result.As<CompanyNew>();

            var clue = new Clue(request.EntityMetaData.OriginEntityCode, context.Organization) { Data = { OriginProviderDefinitionId = Id } };

            PopulateMetadata(clue.Data.EntityData, resultItem.Data, request, config);
            yield return clue;
        }

        public IEntityMetadata GetPrimaryEntityMetadata(ExecutionContext context, IExternalSearchQueryResult result,
            IExternalSearchRequest request, IDictionary<string, object> config, IProvider provider)
        {
            var resultItem = result.As<CompanyNew>();
            return CreateMetadata(resultItem, request, config);
        }

        public IPreviewImage GetPrimaryEntityPreviewImage(ExecutionContext context, IExternalSearchQueryResult result,
            IExternalSearchRequest request, IDictionary<string, object> config, IProvider provider)
        {
            return null;
        }

        public string Icon { get; } = Constants.Icon;
        public string Domain { get; } = Constants.Domain;
        public string About { get; } = Constants.About;

        public AuthMethods AuthMethods { get; } = Constants.AuthMethods;
        public IEnumerable<Control> Properties { get; } = Constants.Properties;
        public Guide Guide { get; } = Constants.Guide;
        public IntegrationType Type { get; } = Constants.IntegrationType;


        /**********************************************************************************************************
         * METHODS
         **********************************************************************************************************/

        /// <summary>Builds the queries.</summary>
        /// <param name="context">The context.</param>
        /// <param name="request">The request.</param>
        /// <returns>The search queries.</returns>
        public override IEnumerable<IExternalSearchQuery> BuildQueries(ExecutionContext context,
            IExternalSearchRequest request)
        {
            return Enumerable.Empty<IExternalSearchQuery>();
        }

        private IEnumerable<IExternalSearchQuery> InternalBuildQueries(ExecutionContext context,
            IExternalSearchRequest request, IDictionary<string, object> config = null)
        {
            var jobData = new CompanyHouseExternalSearchJobData(config);

            if (!string.IsNullOrWhiteSpace(jobData.AcceptedEntityType))
            {
                if (!request.EntityMetaData.EntityType.Is(jobData.AcceptedEntityType))
                {
                    yield break;
                }
            }
            else if (!Accepts(config, request.EntityMetaData.EntityType))
            {
                yield break;
            }

            var existingResults = request.GetQueryResults<CompanyNew>(this).ToList();

            bool idFilter(string value) =>
                existingResults.Any(r => string.Equals(r.Data.company_number, value, StringComparison.InvariantCultureIgnoreCase));

            bool nameFilter(string value) =>
                existingResults.Any(r => string.Equals(r.Data.company_name, value, StringComparison.InvariantCultureIgnoreCase));

            var entityType = request.EntityMetaData.EntityType;

            HashSet<string> companyHouseNumber;

            string country;

            if (!string.IsNullOrWhiteSpace(jobData.CompanyHouseNumberKey))
            {
                companyHouseNumber =
                    request.QueryParameters.GetValue<string, HashSet<string>>(
                        jobData.CompanyHouseNumberKey, new HashSet<string>());
            }
            else
            {
                companyHouseNumber = request.QueryParameters.GetValue(
                    Core.Data.Vocabularies.Vocabularies.CluedInOrganization.CodesCompanyHouse, new HashSet<string>());
            }

            if (!string.IsNullOrWhiteSpace(jobData.CountryKey))
            {
                country = request.EntityMetaData.Properties.ContainsKey(jobData.CountryKey)
                    ? request.EntityMetaData.Properties[jobData.CountryKey]
                    : string.Empty;
            }
            else
            {
                country =
                    request.EntityMetaData.Properties.ContainsKey(Core.Data.Vocabularies.Vocabularies.CluedInOrganization.AddressCountryCode)
                        ? request.EntityMetaData
                            .Properties[Core.Data.Vocabularies.Vocabularies.CluedInOrganization.AddressCountryCode]
                            .ToLowerInvariant()
                        : string.Empty;
            }

            HashSet<string> organizationName;
            if (!string.IsNullOrWhiteSpace(jobData.OrgNameKey))
            {
                organizationName =
                    request.QueryParameters.GetValue<string, HashSet<string>>(jobData.OrgNameKey, new HashSet<string>());
            }
            else
            {
                organizationName = request.QueryParameters.GetValue(
                    Core.Data.Vocabularies.Vocabularies.CluedInOrganization.OrganizationName, new HashSet<string>());
            }

            if (!string.IsNullOrEmpty(request.EntityMetaData.Name))
            {
                organizationName.Add(request.EntityMetaData.Name);
            }

            if (!string.IsNullOrEmpty(request.EntityMetaData.DisplayName))
            {
                organizationName.Add(request.EntityMetaData.DisplayName);
            }

            if (organizationName != null)
            {
                var values = organizationName.Select(NameNormalization.Normalize).ToHashSet();

                foreach (var value in values.Where(v => !nameFilter(v)))
                {
                    yield return new ExternalSearchQuery(this, entityType, ExternalSearchQueryParameter.Name, value);
                }
            }

            foreach (var value in companyHouseNumber.Where(v => !idFilter(v)))
            {
                var externalSearchQuery =
                    new ExternalSearchQuery(this, entityType, ExternalSearchQueryParameter.Identifier, value);
                yield return externalSearchQuery;
            }
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public ConnectionVerificationResult VerifyConnection(ExecutionContext context, IReadOnlyDictionary<string, object> config)
        {
            IDictionary<string, object> configDict = config.ToDictionary(entry => entry.Key, entry => entry.Value);
            var jobData = new CompanyHouseExternalSearchJobData(configDict);

            var client = new RestClient("https://api.companieshouse.gov.uk");
            var request = new RestRequest { Method = Method.GET };

            request.AddHeader("Authorization", "Basic " + Base64Encode(jobData.ApiKey));
            request.Resource = $"search/companies?q=Google";
            var companiesResponse = client.ExecuteAsync<CompanySearchResponse>(request).Result;

            if (!companiesResponse.IsSuccessful)
            {
                return ConstructVerifyConnectionResponse(companiesResponse);
            }

            if (companiesResponse.StatusCode == HttpStatusCode.OK)
            {
                foreach (var companyResult in companiesResponse.Data.items)
                {
                    request.Resource = $"company/{companyResult.company_number}";
                    var companyResponse = client.ExecuteAsync<CompanyNew>(request).Result;

                    if (!companyResponse.IsSuccessful)
                    {
                        return ConstructVerifyConnectionResponse(companyResponse);
                    }
                }
            }

            return new ConnectionVerificationResult(true, string.Empty);
        }

        public override bool Accepts(EntityType entityType) => throw new NotSupportedException();

        public override IEnumerable<IExternalSearchQueryResult> ExecuteSearch(ExecutionContext context, IExternalSearchQuery query) => throw new NotSupportedException();
     
        public override IEnumerable<Clue> BuildClues(ExecutionContext context, IExternalSearchQuery query, IExternalSearchQueryResult result, IExternalSearchRequest request) => throw new NotSupportedException();

        public override IEntityMetadata GetPrimaryEntityMetadata(ExecutionContext context, IExternalSearchQueryResult result, IExternalSearchRequest request) => throw new NotSupportedException();

        public override IPreviewImage GetPrimaryEntityPreviewImage(ExecutionContext context, IExternalSearchQueryResult result, IExternalSearchRequest request) => throw new NotSupportedException();

        private ConnectionVerificationResult ConstructVerifyConnectionResponse(IRestResponse response)
        {
            var errorMessageBase = $"{Constants.ProviderName} returned \"{(int)response.StatusCode} {response.StatusDescription}\".";
            if (response.ErrorException != null)
            {
                return new ConnectionVerificationResult(false, $"{errorMessageBase} {(!string.IsNullOrWhiteSpace(response.ErrorException.Message) ? response.ErrorException.Message : "This could be due to breaking changes in the external system")}.");
            }

            if (response.StatusCode is HttpStatusCode.Unauthorized)
            {
                return new ConnectionVerificationResult(false, $"{errorMessageBase} This could be due to invalid API key.");
            }

            var regex = new Regex(@"\<(html|head|body|div|span|img|p\>|a href)", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
            var isHtml = regex.IsMatch(response.Content);

            var errorMessage = response.IsSuccessful ? string.Empty
                : string.IsNullOrWhiteSpace(response.Content) || isHtml
                    ? $"{errorMessageBase} This could be due to breaking changes in the external system."
                    : $"{errorMessageBase} {response.Content}.";

            return new ConnectionVerificationResult(response.IsSuccessful, errorMessage);
        }

        private IEntityMetadata CreateMetadata(IExternalSearchQueryResult<CompanyNew> resultItem, IExternalSearchRequest request, IDictionary<string, object> config)
        {
            var metadata = new EntityMetadataPart();

            PopulateMetadata(metadata, resultItem.Data, request, config);

            return metadata;
        }

        /// <summary>Gets the code origin.</summary>
        /// <returns>The code origin</returns>
        private CodeOrigin GetCodeOrigin()
        {
            return CodeOrigin.CluedIn.CreateSpecific("companiesHouse");
        }

        /// <summary>Populates the metadata.</summary>
        /// <param name="metadata">The metadata.</param>
        /// <param name="resultCompany">The result item.</param>
        private void PopulateMetadata(IEntityMetadata metadata, CompanyNew resultCompany, IExternalSearchRequest request, IDictionary<string, object> config)
        {
            var jobData = new CompanyHouseExternalSearchJobData(config);
            var code = request.EntityMetaData.OriginEntityCode;

            metadata.EntityType = request.EntityMetaData.EntityType;
            metadata.OriginEntityCode = code;
            metadata.Name = request.EntityMetaData.Name;

            if (!jobData.SkipCompanyHouseNumberEntityCodeCreation)
            {
                metadata.Codes.Add(new EntityCode(request.EntityMetaData.EntityType, GetCodeOrigin(), resultCompany.company_number));
            }

            if (!jobData.SkipCompanyHouseNameEntityCodeCreation && !string.IsNullOrEmpty(resultCompany.company_name))
            {
                metadata.Codes.Add(new EntityCode(request.EntityMetaData.EntityType, GetCodeOrigin(), resultCompany.company_name));
            }

            metadata.DisplayName = resultCompany.company_name.PrintIfAvailable();

            metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] =
                resultCompany.company_number.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.Charges] = resultCompany.has_charges.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.CompanyStatus] =
                resultCompany.company_status.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.Type] = resultCompany.type.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.Jurisdiction] =
                resultCompany.jurisdiction.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.Has_been_liquidated] =
                resultCompany.has_been_liquidated.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.Has_insolvency_history] =
                resultCompany.has_insolvency_history.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.Registered_office_is_in_dispute] =
                resultCompany.registered_office_is_in_dispute.PrintIfAvailable();

            if (!string.IsNullOrEmpty(resultCompany.date_of_creation))
            {
                //if (DateTimeOffset.TryParse(resultCompany.date_of_creation, out var createdDate))
                //{
                //    metadata.CreatedDate = createdDate;
                //}

                metadata.Properties[CompanyHouseVocabulary.Organization.DateOfCreation] = resultCompany.date_of_creation;
            }

            if (resultCompany.registered_office_address != null)
            {
                PopulateOrgAddressMetadata(metadata, CompanyHouseVocabulary.Organization.Address, resultCompany);
            }
        }

        private void PopulateOrgAddressMetadata(IEntityMetadata metadata, CompanyHouseOrgAddressVocabulary vocab,
            CompanyNew resultCompany)
        {
            metadata.Properties[vocab.AddressLine1] =
                resultCompany.registered_office_address.address_line_1.PrintIfAvailable();
            metadata.Properties[vocab.AddressLine2] =
                resultCompany.registered_office_address.address_line_2.PrintIfAvailable();
            metadata.Properties[vocab.Locality] = resultCompany.registered_office_address.locality.PrintIfAvailable();
            metadata.Properties[vocab.PostCode] = resultCompany.registered_office_address.postal_code.PrintIfAvailable();
        }


        // TODO: not used
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
                metadata.Properties[CompanyHouseVocabulary.Person.Date_of_birth] =
                    $"{resultItem.date_of_birth.year}.{resultItem.date_of_birth.month}.1";
            }

            metadata.Properties[CompanyHouseVocabulary.Person.Country_of_residence] =
                resultItem.country_of_residence.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Person.Occupation] = resultItem.occupation.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = resultItem.nationality.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Person.Resigned_on] = resultItem.resigned_on.PrintIfAvailable();
        }

        private void PopulatePersonAddressMetadata(IEntityMetadata metadata, CompanyHousePersonAddressVocabulary vocab,
            ContactAddress address)
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
    }
}
