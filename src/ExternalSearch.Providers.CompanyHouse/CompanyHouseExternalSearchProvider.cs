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

            var companyNumber = query.QueryParameters.ContainsKey(ExternalSearchQueryParameter.Identifier)
                ? query.QueryParameters[ExternalSearchQueryParameter.Identifier].FirstOrDefault()
                : null;

            var client = new CompanyHouseClient(jobData);

            if (!string.IsNullOrEmpty(name))
            {
                var companies = client.GetCompanies(name);
                if (companies != null)
                {
                    foreach (var company in companies.Select(companyResult => client.GetCompany(companyResult.company_number)))
                    {
                        yield return new ExternalSearchQueryResult<CompanyNew>(query, company);
                    }
                }
            }

            if (string.IsNullOrEmpty(companyNumber))
            {
                yield break;
            }

            var companySearchByNumber = client.GetCompany(companyNumber);

            if (companySearchByNumber == null)
            {
                yield break;
            }

            yield return new ExternalSearchQueryResult<CompanyNew>(query, companySearchByNumber);
        }

        public IEnumerable<Clue> BuildClues(ExecutionContext context, IExternalSearchQuery query,
            IExternalSearchQueryResult result, IExternalSearchRequest request, IDictionary<string, object> config,
            IProvider provider)
        {
            var resultItem = result.As<CompanyNew>();
            var code = new EntityCode(request.EntityMetaData.OriginEntityCode.Type, GetCodeOrigin(), resultItem.Data.company_number);
            var clue = new Clue(code, context.Organization) { Data = { OriginProviderDefinitionId = Id } };

            PopulateMetadata(clue.Data.EntityData, resultItem.Data, request);
            yield return clue;
        }

        public IEntityMetadata GetPrimaryEntityMetadata(ExecutionContext context, IExternalSearchQueryResult result,
            IExternalSearchRequest request, IDictionary<string, object> config, IProvider provider)
        {
            var resultItem = result.As<CompanyNew>();
            return CreateMetadata(resultItem, request);
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

            bool existingCompanyHouseNumberDataFilter(string value) =>
                existingResults.Any(r => r?.Data?.company_number != null && string.Equals(r.Data.company_number, value, StringComparison.InvariantCultureIgnoreCase));

            bool existingNameDataFilter(string value) =>
                existingResults.Any(r => r?.Data?.company_name != null && string.Equals(r.Data.company_name, value, StringComparison.InvariantCultureIgnoreCase));

            var entityType = request.EntityMetaData.EntityType;
            var entityName = !string.IsNullOrEmpty(request.EntityMetaData.Name) ? request.EntityMetaData.Name : request.EntityMetaData.DisplayName;

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

            if (jobData.SearchWithEntityName)
            {
                if (!string.IsNullOrEmpty(request.EntityMetaData.Name))
                {
                    organizationName.Add(request.EntityMetaData.Name);
                }

                if (!string.IsNullOrEmpty(request.EntityMetaData.DisplayName))
                {
                    organizationName.Add(request.EntityMetaData.DisplayName);
                }
            }

            var queriesGenerated = false;
            var normalizeOrganizationName = organizationName.Select(NameNormalization.Normalize).ToHashSet();

            foreach (var value in normalizeOrganizationName.Where(v => !existingNameDataFilter(v)))
            {
                queriesGenerated = true;
                yield return new ExternalSearchQuery(this, entityType, ExternalSearchQueryParameter.Name, value);
            }

            foreach (var value in companyHouseNumber.Where(v => !existingCompanyHouseNumberDataFilter(v)))
            {
                queriesGenerated = true;
                yield return new ExternalSearchQuery(this, entityType, ExternalSearchQueryParameter.Identifier, value);
            }

            // Throw error when queries not generated
            ThrowExceptions(queriesGenerated, companyHouseNumber, normalizeOrganizationName, entityName, jobData);
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
            var request = new RestRequest { Method = Method.Get };

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

        private ConnectionVerificationResult ConstructVerifyConnectionResponse(RestResponse response)
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

        private IEntityMetadata CreateMetadata(IExternalSearchQueryResult<CompanyNew> resultItem, IExternalSearchRequest request)
        {
            var metadata = new EntityMetadataPart();

            PopulateMetadata(metadata, resultItem.Data, request);

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
        private void PopulateMetadata(IEntityMetadata metadata, CompanyNew resultCompany, IExternalSearchRequest request)
        {
            var code = new EntityCode(request.EntityMetaData.OriginEntityCode.Type, GetCodeOrigin(), resultCompany.company_number);

            metadata.EntityType = request.EntityMetaData.EntityType;
            metadata.OriginEntityCode = code;
            metadata.Name = request.EntityMetaData.Name;
            metadata.Codes.Add(request.EntityMetaData.OriginEntityCode);

            metadata.DisplayName = resultCompany.company_name.PrintIfAvailable();

            metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = resultCompany.company_number.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.Charges] = resultCompany.has_charges.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.CompanyStatus] = resultCompany.company_status.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.Type] = resultCompany.type.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.IsCommunityInterestCompany] = resultCompany.is_community_interest_company.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.Jurisdiction] = resultCompany.jurisdiction.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.HasBeenLiquidated] = resultCompany.has_been_liquidated.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.HasInsolvencyHistory] = resultCompany.has_insolvency_history.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.LastFullMembersListDate] = resultCompany.last_full_members_list_date.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.PartialDataAvailable] = resultCompany.partial_data_available.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.SubType] = resultCompany.subtype.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.SuperSecureManagingOfficerCount] = resultCompany.super_secure_managing_officer_count.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.RegisteredOfficeIsInDispute] = resultCompany.registered_office_is_in_dispute.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.UndeliverableRegisteredOfficeAddress] = resultCompany.undeliverable_registered_office_address.PrintIfAvailable();

            metadata.Properties[CompanyHouseVocabulary.Organization.AnnualReturnLastMadeUpTo] = resultCompany.annual_return?.last_made_up_to.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.AnnualReturnNextDue] = resultCompany.annual_return?.next_due.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.AnnualReturnNextMadeUpTo] = resultCompany.annual_return?.next_made_up_to.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.AnnualReturnOverdue] = resultCompany.annual_return?.overdue.PrintIfAvailable();

            metadata.Properties[CompanyHouseVocabulary.Organization.CanFile] = resultCompany.can_file.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.CompanyName] = resultCompany.company_name.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.CompanyStatusDetail] = resultCompany.company_status_detail.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.ConfirmationStatementLastMadeUpTo] = resultCompany.confirmation_statement?.last_made_up_to.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.ConfirmationStatementNextDue] = resultCompany.confirmation_statement?.next_due.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.ConfirmationStatementNextMadeUpTo] = resultCompany.confirmation_statement?.next_made_up_to.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.ConfirmationStatementOverdue] = resultCompany.confirmation_statement?.overdue.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.BranchCompanyBusinessActivity] = resultCompany.branch_company_details?.business_activity.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.BranchCompanyParentCompanyName] = resultCompany.branch_company_details?.parent_company_name.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.BranchCompanyParentCompanyNumber] = resultCompany.branch_company_details?.parent_company_number.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.DateOfCessation] = resultCompany.date_of_cessation.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.DateOfCreation] = resultCompany.date_of_creation.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.ETag] = resultCompany.etag.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.ExternalRegistrationNumber] = resultCompany.external_registration_number.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.ForeignCompanyAccountingRequirementForeignAccountType] = resultCompany.foreign_company_details?.accounting_requirement?.foreign_account_type.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.ForeignCompanyAccountingRequirementTermsOfAccountPublication] = resultCompany.foreign_company_details?.accounting_requirement?.terms_of_account_publication.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.ForeignCompanyAccountsAccountPeriodFromDay] = resultCompany.foreign_company_details?.accounts?.account_period_from?.day.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.ForeignCompanyAccountsAccountPeriodFromMonth] = resultCompany.foreign_company_details?.accounts?.account_period_from?.month.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.ForeignCompanyAccountsAccountPeriodToDay] = resultCompany.foreign_company_details?.accounts?.account_period_to?.day.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.ForeignCompanyAccountsAccountPeriodToMonth] = resultCompany.foreign_company_details?.accounts?.account_period_to?.month.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.ForeignCompanyAccountsMustFileWithinMonths] = resultCompany.foreign_company_details?.accounts?.must_file_within?.months.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.ForeignCompanyBusinessActivity] = resultCompany.foreign_company_details?.business_activity.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.ForeignCompanyCompanyType] = resultCompany.foreign_company_details?.company_type.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.ForeignCompanyGovernedBy] = resultCompany.foreign_company_details?.governed_by.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.ForeignCompanyIsACreditFinanceInstitution] = resultCompany.foreign_company_details?.is_a_credit_finance_institution.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.ForeignCompanyOriginatingRegistryCountry] = resultCompany.foreign_company_details?.originating_registry?.country.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.ForeignCompanyOriginatingRegistryName] = resultCompany.foreign_company_details?.originating_registry?.name.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.ForeignCompanyRegistrationNumber] = resultCompany.foreign_company_details?.registration_number.PrintIfAvailable();

            if (resultCompany.sic_codes != null && resultCompany.sic_codes.Any())
            {
                metadata.Properties[CompanyHouseVocabulary.Organization.SicCodes] = string.Join(", ", resultCompany.sic_codes);
            }

            if (resultCompany.registered_office_address != null)
            {
                PopulateOrgAddressMetadata(metadata, resultCompany);
            }

            if (resultCompany.accounts != null)
            {
                PopulateOrgAccountsMetadata(metadata, resultCompany);
            }

            if (resultCompany.corporate_annotation != null && resultCompany.corporate_annotation.Any())
            {
                PopulateCorporateAnnotation(metadata, resultCompany);
            }

            if (resultCompany.previous_company_names != null && resultCompany.previous_company_names.Any())
            {
                PopulatePreviousCompanyNames(metadata, resultCompany);
            }
        }

        private void PopulateOrgAddressMetadata(IEntityMetadata metadata, CompanyNew resultCompany)
        {
            metadata.Properties[CompanyHouseVocabulary.Organization.Address.RegisteredOfficeAddressLine1] = resultCompany.registered_office_address?.address_line_1.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.Address.RegisteredOfficeAddressLine2] = resultCompany.registered_office_address?.address_line_2.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.Address.RegisteredOfficeCareOf] = resultCompany.registered_office_address?.care_of.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.Address.RegisteredOfficeCountry] = resultCompany.registered_office_address?.country.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.Address.RegisteredOfficeLocality] = resultCompany.registered_office_address?.locality.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.Address.RegisteredOfficePoBox] = resultCompany.registered_office_address?.po_box.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.Address.RegisteredOfficePostalCode] = resultCompany.registered_office_address?.postal_code.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.Address.RegisteredOfficePremises] = resultCompany.registered_office_address?.premises.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.Address.RegisteredOfficeRegion] = resultCompany.registered_office_address?.region.PrintIfAvailable();

            metadata.Properties[CompanyHouseVocabulary.Organization.Address.ServiceAddressLine1] = resultCompany.service_address?.address_line_1.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.Address.ServiceAddressLine2] = resultCompany.service_address?.address_line_2.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.Address.ServiceAddressCareOf] = resultCompany.service_address?.care_of.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.Address.ServiceAddressCountry] = resultCompany.service_address?.country.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.Address.ServiceAddressLocality] = resultCompany.service_address?.locality.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.Address.ServiceAddressPoBox] = resultCompany.service_address?.po_box.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.Address.ServiceAddressPostalCode] = resultCompany.service_address?.postal_code.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.Address.ServiceAddressRegion] = resultCompany.service_address?.region.PrintIfAvailable();
        }

        private void PopulateOrgAccountsMetadata(IEntityMetadata metadata, CompanyNew resultCompany)
        {
            metadata.Properties[CompanyHouseVocabulary.Organization.Accounts.AccountingReferenceDateDay] = resultCompany.accounts?.accounting_reference_date?.day.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.Accounts.AccountingReferenceDateMonth] = resultCompany.accounts?.accounting_reference_date?.month.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.Accounts.LastAccountsMadeUpTo] = resultCompany.accounts?.last_accounts?.made_up_to.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.Accounts.LastAccountsPeriodEndOn] = resultCompany.accounts?.last_accounts?.period_end_on.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.Accounts.LastAccountsPeriodStartOn] = resultCompany.accounts?.last_accounts?.period_start_on.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.Accounts.LastAccountsType] = resultCompany.accounts?.last_accounts?.type.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.Accounts.NextAccountsDueOn] = resultCompany.accounts?.next_accounts?.due_on.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.Accounts.NextAccountsOverdue] = resultCompany.accounts?.next_accounts?.overdue.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.Accounts.NextAccountsPeriodEndOn] = resultCompany.accounts?.next_accounts?.period_end_on.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.Accounts.NextAccountsPeriodStartOn] = resultCompany.accounts?.next_accounts?.period_start_on.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.Accounts.NextDue] = resultCompany.accounts?.next_due.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.Accounts.NextMadeUpTo] = resultCompany.accounts?.next_made_up_to.PrintIfAvailable();
            metadata.Properties[CompanyHouseVocabulary.Organization.Accounts.Overdue] = resultCompany.accounts?.overdue.PrintIfAvailable();
        }

        private static void PopulateCorporateAnnotation(IEntityMetadata metadata, CompanyNew resultCompany)
        {
            var corporateAnnotationIndex = 1;
            foreach (var corporateAnnotation in resultCompany.corporate_annotation ?? Enumerable.Empty<CorporateAnnotation>())
            {
                metadata.Properties[$"{CompanyHouseVocabulary.Organization.CorporateAnnotation.KeyPrefix}{CompanyHouseVocabulary.Organization.CorporateAnnotation.KeySeparator}{corporateAnnotationIndex}.createdOn"] = corporateAnnotation.created_on.PrintIfAvailable();
                metadata.Properties[$"{CompanyHouseVocabulary.Organization.CorporateAnnotation.KeyPrefix}{CompanyHouseVocabulary.Organization.CorporateAnnotation.KeySeparator}{corporateAnnotationIndex}.description"] = corporateAnnotation.description.PrintIfAvailable();
                metadata.Properties[$"{CompanyHouseVocabulary.Organization.CorporateAnnotation.KeyPrefix}{CompanyHouseVocabulary.Organization.CorporateAnnotation.KeySeparator}{corporateAnnotationIndex}.type"] = corporateAnnotation.type.PrintIfAvailable();

                corporateAnnotationIndex++;
            }
        }

        private static void PopulatePreviousCompanyNames(IEntityMetadata metadata, CompanyNew resultCompany)
        {
            var previousCompanyNameIndex = 1;
            foreach (var previousCompanyName in resultCompany.previous_company_names ?? Enumerable.Empty<PreviousCompanyName>())
            {
                metadata.Properties[$"{CompanyHouseVocabulary.Organization.PreviousCompanyName.KeyPrefix}{CompanyHouseVocabulary.Organization.PreviousCompanyName.KeySeparator}{previousCompanyNameIndex}.ceasedOn"] = previousCompanyName.ceased_on.PrintIfAvailable();
                metadata.Properties[$"{CompanyHouseVocabulary.Organization.PreviousCompanyName.KeyPrefix}{CompanyHouseVocabulary.Organization.PreviousCompanyName.KeySeparator}{previousCompanyNameIndex}.effectiveFrom"] = previousCompanyName.effective_from.PrintIfAvailable();
                metadata.Properties[$"{CompanyHouseVocabulary.Organization.PreviousCompanyName.KeyPrefix}{CompanyHouseVocabulary.Organization.PreviousCompanyName.KeySeparator}{previousCompanyNameIndex}.name"] = previousCompanyName.name.PrintIfAvailable();

                previousCompanyNameIndex++;
            }
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

        private static void ThrowExceptions(bool queriesGenerated, HashSet<string> companyHouseNumber, HashSet<string> normalizeOrganizationName, string entityName, CompanyHouseExternalSearchJobData jobData)
        {
            switch (queriesGenerated)
            {
                case false when !string.IsNullOrWhiteSpace(jobData.CompanyHouseNumberKey) && string.IsNullOrWhiteSpace(jobData.OrgNameKey) && !companyHouseNumber.Any():
                    throw new Exception($"Unable to generate queries for {entityName}. Company House number is empty.");
                case false when !string.IsNullOrWhiteSpace(jobData.OrgNameKey) && string.IsNullOrWhiteSpace(jobData.CompanyHouseNumberKey) && !normalizeOrganizationName.Any():
                    throw new Exception($"Unable to generate queries for {entityName}. Name is empty.");
                case false when !companyHouseNumber.Any() && !normalizeOrganizationName.Any():
                    throw new Exception($"Unable to generate queries for {entityName}. Both Company House number and name are empty.");
            }
        }
    }
}
