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

namespace CluedIn.ExternalSearch.Providers.CompanyHouse.Vocabularies
{
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
                CompanyNumber = group.Add(new VocabularyKey("companyNumber", VocabularyKeyDataType.Text, VocabularyKeyVisibility.Hidden));
                Type = group.Add(new VocabularyKey("type", VocabularyKeyDataType.Text));
                Charges = group.Add(new VocabularyKey("charges", VocabularyKeyDataType.Text));
                CompanyStatus = group.Add(new VocabularyKey("companyStatus", VocabularyKeyDataType.Text));
                DateOfCreation = group.Add(new VocabularyKey("dateOfCreation", VocabularyKeyDataType.Text));
                Jurisdiction = group.Add(new VocabularyKey("jurisdiction", VocabularyKeyDataType.Text));
                Has_been_liquidated = group.Add(new VocabularyKey("has_been_liquidated", VocabularyKeyDataType.Text));
                Has_insolvency_history = group.Add(new VocabularyKey("has_insolvency_history", VocabularyKeyDataType.Text));
                Registered_office_is_in_dispute = group.Add(new VocabularyKey("registered_office_is_in_dispute", VocabularyKeyDataType.Text));
            });

            AddGroup("Address", group =>
            {
                Address = group.Add(new CompanyHouseOrgAddressVocabulary().AsCompositeKey("address"));
            });
        }

        public VocabularyKey CompanyNumber { get; protected set; }
        public VocabularyKey Type { get; protected set; }
        public VocabularyKey Charges { get; protected set; }
        public VocabularyKey CompanyStatus { get; protected set; }
        public VocabularyKey DateOfCreation { get; protected set; }
        public VocabularyKey Jurisdiction { get; protected set; }
        public VocabularyKey Has_been_liquidated { get; protected set; }
        public VocabularyKey Has_insolvency_history { get; protected set; }
        public VocabularyKey Registered_office_is_in_dispute { get; protected set; }

        public CompanyHouseOrgAddressVocabulary Address { get; protected set; }
    }
}
