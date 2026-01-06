// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompanyHouseOrganizationVocabulary.cs" company="Clued In">
//   Copyright Clued In
// </copyright>
// <summary>
//   Defines the CompanyHouseOrganizationVocabulary type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using CluedIn.Core.Data;
using CluedIn.Core.Data.Vocabularies;

namespace CluedIn.ExternalSearch.Providers.CompanyHouse.Vocabularies;

/// <summary>The clear bit organization vocabulary.</summary>
/// <seealso cref="CluedIn.Core.Data.Vocabularies.SimpleVocabulary" />
public class CompanyHouseOrganizationVocabulary : SimpleVocabulary
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CompanyHouseOrganizationVocabulary"/> class.
    /// </summary>
    public CompanyHouseOrganizationVocabulary()
    {
        VocabularyName = "CompanyHouse Organization";
        KeyPrefix = "companyHouse.organization";
        KeySeparator   = ".";
        Grouping       = EntityType.Organization;

        AddGroup("Metadata", group =>
        {
            AnnualReturnLastMadeUpTo = group.Add(new VocabularyKey("annualReturnLastMadeUpTo"));
            AnnualReturnNextDue = group.Add(new VocabularyKey("annualReturnNextDue"));
            AnnualReturnNextMadeUpTo = group.Add(new VocabularyKey("annualReturnNextMadeUpTo"));
            AnnualReturnOverdue = group.Add(new VocabularyKey("annualReturnOverdue"));
            CanFile = group.Add(new VocabularyKey("canFile"));
            CompanyName = group.Add(new VocabularyKey("companyName"));
            CompanyNumber = group.Add(new VocabularyKey("companyNumber"));
            CompanyStatus = group.Add(new VocabularyKey("companyStatus"));
            CompanyStatusDetail = group.Add(new VocabularyKey("companyStatusDetail"));
            ConfirmationStatementLastMadeUpTo = group.Add(new VocabularyKey("confirmationStatementLastMadeUpTo"));
            ConfirmationStatementNextDue = group.Add(new VocabularyKey("confirmationStatementNextDue"));
            ConfirmationStatementNextMadeUpTo = group.Add(new VocabularyKey("confirmationStatementNextMadeUpTo"));
            ConfirmationStatementOverdue = group.Add(new VocabularyKey("confirmationStatementOverdue"));
            BranchCompanyBusinessActivity = group.Add(new VocabularyKey("branchCompanyBusinessActivity"));
            BranchCompanyParentCompanyName = group.Add(new VocabularyKey("branchCompanyParentCompanyName"));
            BranchCompanyParentCompanyNumber = group.Add(new VocabularyKey("branchCompanyParentCompanyNumber"));
            DateOfCessation = group.Add(new VocabularyKey("dateOfCessation"));
            DateOfCreation = group.Add(new VocabularyKey("dateOfCreation"));
            ETag = group.Add(new VocabularyKey("eTag"));
            ExternalRegistrationNumber = group.Add(new VocabularyKey("externalRegistrationNumber"));
            ForeignCompanyAccountingRequirementForeignAccountType = group.Add(new VocabularyKey("foreignCompanyAccountingRequirementForeignAccountType"));
            ForeignCompanyAccountingRequirementTermsOfAccountPublication = group.Add(new VocabularyKey("foreignCompanyAccountingRequirementTermsOfAccountPublication"));
            ForeignCompanyAccountsAccountPeriodFromDay = group.Add(new VocabularyKey("foreignCompanyAccountsAccountPeriodFromDay"));
            ForeignCompanyAccountsAccountPeriodFromMonth = group.Add(new VocabularyKey("foreignCompanyAccountsAccountPeriodFromMonth"));
            ForeignCompanyAccountsAccountPeriodToDay = group.Add(new VocabularyKey("foreignCompanyAccountsAccountPeriodToDay"));
            ForeignCompanyAccountsAccountPeriodToMonth = group.Add(new VocabularyKey("foreignCompanyAccountsAccountPeriodToMonth"));
            ForeignCompanyAccountsMustFileWithinMonths = group.Add(new VocabularyKey("foreignCompanyAccountsMustFileWithinMonths"));
            ForeignCompanyBusinessActivity = group.Add(new VocabularyKey("foreignCompanyBusinessActivity"));
            ForeignCompanyCompanyType = group.Add(new VocabularyKey("foreignCompanyCompanyType"));
            ForeignCompanyGovernedBy = group.Add(new VocabularyKey("foreignCompanyGovernedBy"));
            ForeignCompanyIsACreditFinanceInstitution = group.Add(new VocabularyKey("foreignCompanyIsACreditFinanceInstitution"));
            ForeignCompanyOriginatingRegistryCountry = group.Add(new VocabularyKey("foreignCompanyOriginatingRegistryCountry"));
            ForeignCompanyOriginatingRegistryName = group.Add(new VocabularyKey("foreignCompanyOriginatingRegistryName"));
            ForeignCompanyRegistrationNumber = group.Add(new VocabularyKey("foreignCompanyRegistrationNumber"));
            HasBeenLiquidated = group.Add(new VocabularyKey("has_been_liquidated"));
            Charges = group.Add(new VocabularyKey("charges")); // has_charges
            HasInsolvencyHistory = group.Add(new VocabularyKey("has_insolvency_history"));
            IsCommunityInterestCompany = group.Add(new VocabularyKey("isCommunityInterestCompany"));
            Jurisdiction = group.Add(new VocabularyKey("jurisdiction"));
            LastFullMembersListDate = group.Add(new VocabularyKey("lastFullMembersListDate"));
            PartialDataAvailable = group.Add(new VocabularyKey("partialDataAvailable"));
            RegisteredOfficeIsInDispute = group.Add(new VocabularyKey("registered_office_is_in_dispute"));
            SubType = group.Add(new VocabularyKey("subtype"));
            SuperSecureManagingOfficerCount = group.Add(new VocabularyKey("superSecureManagingOfficerCount"));
            Type = group.Add(new VocabularyKey("type"));
            UndeliverableRegisteredOfficeAddress = group.Add(new VocabularyKey("undeliverableRegisteredOfficeAddress"));
            SicCodes = group.Add(new VocabularyKey("sicCodes"));
        });

        AddGroup("Address", group =>
        {
            Address = group.Add(new CompanyHouseOrgAddressVocabulary().AsCompositeKey("address"));
        });

        AddGroup("Accounts", group =>
        {
            Accounts = group.Add(new CompanyHouseOrgAccountsVocabulary().AsCompositeKey("accounts"));
        });

        AddGroup("CorporateAnnotation", group =>
        {
            CorporateAnnotation = group.Add(new CompanyHouseOrgCorporateAnnotationVocabulary().AsCompositeKey("corporateAnnotation"));
        });

        AddGroup("PreviousCompanyName", group =>
        {
            PreviousCompanyName = group.Add(new CompanyHouseOrgPreviousCompanyNameVocabulary().AsCompositeKey("previousCompanyName"));
        });
    }

    public VocabularyKey AnnualReturnLastMadeUpTo { get; protected set; }
    public VocabularyKey AnnualReturnNextDue { get; protected set; }
    public VocabularyKey AnnualReturnNextMadeUpTo { get; protected set; }
    public VocabularyKey AnnualReturnOverdue { get; protected set; }
    public VocabularyKey CanFile { get; protected set; }
    public VocabularyKey CompanyName { get; protected set; }
    public VocabularyKey CompanyNumber { get; protected set; }
    public VocabularyKey CompanyStatus { get; protected set; }
    public VocabularyKey CompanyStatusDetail { get; protected set; }
    public VocabularyKey ConfirmationStatementLastMadeUpTo { get; protected set; }
    public VocabularyKey ConfirmationStatementNextDue { get; protected set; }
    public VocabularyKey ConfirmationStatementNextMadeUpTo { get; protected set; }
    public VocabularyKey ConfirmationStatementOverdue { get; protected set; }
    public VocabularyKey Charges { get; protected set; }
    public VocabularyKey BranchCompanyBusinessActivity { get; protected set; }
    public VocabularyKey BranchCompanyParentCompanyName { get; protected set; }
    public VocabularyKey BranchCompanyParentCompanyNumber { get; protected set; }
    public VocabularyKey DateOfCessation { get; protected set; }
    public VocabularyKey DateOfCreation { get; protected set; }
    public VocabularyKey ETag { get; protected set; }
    public VocabularyKey ExternalRegistrationNumber { get; protected set; }
    public VocabularyKey ForeignCompanyAccountingRequirementForeignAccountType { get; protected set; }
    public VocabularyKey ForeignCompanyAccountingRequirementTermsOfAccountPublication { get; protected set; }
    public VocabularyKey ForeignCompanyAccountsAccountPeriodFromDay { get; protected set; }
    public VocabularyKey ForeignCompanyAccountsAccountPeriodFromMonth { get; protected set; }
    public VocabularyKey ForeignCompanyAccountsAccountPeriodToDay { get; protected set; }
    public VocabularyKey ForeignCompanyAccountsAccountPeriodToMonth { get; protected set; }
    public VocabularyKey ForeignCompanyAccountsMustFileWithinMonths { get; protected set; }
    public VocabularyKey ForeignCompanyBusinessActivity { get; protected set; }
    public VocabularyKey ForeignCompanyCompanyType { get; protected set; }
    public VocabularyKey ForeignCompanyGovernedBy { get; protected set; }
    public VocabularyKey ForeignCompanyIsACreditFinanceInstitution { get; protected set; }
    public VocabularyKey ForeignCompanyOriginatingRegistryCountry { get; protected set; }
    public VocabularyKey ForeignCompanyOriginatingRegistryName { get; protected set; }
    public VocabularyKey ForeignCompanyRegistrationNumber { get; protected set; }
    public VocabularyKey HasBeenLiquidated { get; protected set; }
    public VocabularyKey HasInsolvencyHistory { get; protected set; }
    public VocabularyKey IsCommunityInterestCompany { get; protected set; }
    public VocabularyKey Jurisdiction { get; protected set; }
    public VocabularyKey LastFullMembersListDate { get; protected set; }
    public VocabularyKey PartialDataAvailable { get; protected set; }
    public VocabularyKey RegisteredOfficeIsInDispute { get; protected set; }
    public VocabularyKey SubType { get; protected set; }
    public VocabularyKey SuperSecureManagingOfficerCount { get; protected set; }
    public VocabularyKey Type { get; protected set; }
    public VocabularyKey UndeliverableRegisteredOfficeAddress { get; protected set; }
    public VocabularyKey SicCodes { get; protected set; }

    public CompanyHouseOrgAddressVocabulary Address { get; protected set; }
    public CompanyHouseOrgAccountsVocabulary Accounts { get; protected set; }
    public CompanyHouseOrgCorporateAnnotationVocabulary CorporateAnnotation { get; protected set; }
    public CompanyHouseOrgPreviousCompanyNameVocabulary PreviousCompanyName { get; protected set; }
}
