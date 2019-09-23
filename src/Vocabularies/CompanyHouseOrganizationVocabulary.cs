// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClearBitOrganizationVocabulary.cs" company="Clued In">
//   Copyright Clued In
// </copyright>
// <summary>
//   Defines the ClearBitOrganizationVocabulary type.
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
            this.VocabularyName = "CompanyHouse Organization";
            this.KeyPrefix = "companyHouse.organization";
            this.KeySeparator   = ".";
            this.Grouping       = EntityType.Organization;

            this.CompanyNumber  = this.Add(new VocabularyKey("companyNumber", VocabularyKeyDataType.Text));

            this.LinksSelf      = this.Add(new VocabularyKey("linksSelf", VocabularyKeyDataType.Text));
            this.CompanyStatus  = this.Add(new VocabularyKey("companyStatus", VocabularyKeyDataType.Text));
            this.DateOfCreation = this.Add(new VocabularyKey("dateOfCreation", VocabularyKeyDataType.Text));
        }

        public VocabularyKey CompanyNumber { get; set; }
        //public VocabularyKey AddressLine1 { get; internal set; }
        //public VocabularyKey AddressLine2 { get; internal set; }
        //public VocabularyKey Country { get; internal set; }
        //public VocabularyKey Etag { get; internal set; }
        //public VocabularyKey Locality { get; internal set; }
        //public VocabularyKey PostalCode { get; internal set; }
        //public VocabularyKey PoBox { get; internal set; }
        //public VocabularyKey Premises { get; internal set; }
        //public VocabularyKey Region { get; internal set; }
        //public VocabularyKey AddressSnippet { get; internal set; }
        //public VocabularyKey AcquiredOn { get; internal set; }
        //public VocabularyKey MainCompanyAddress1 { get; internal set; }
        //public VocabularyKey MainCompanyAddress2 { get; internal set; }
        //public VocabularyKey MainCompanyCountry { get; internal set; }
        //public VocabularyKey MainCompanyLocality { get; internal set; }
        //public VocabularyKey MainCompanyPostalCode { get; internal set; }
        //public VocabularyKey MainCompanyPoBox { get; internal set; }
        //public VocabularyKey MainCompanyPremises { get; internal set; }
        //public VocabularyKey MainCompanyRegion { get; internal set; }
        public VocabularyKey LinksSelf { get; internal set; }
        //public VocabularyKey Kind { get; internal set; }
        public VocabularyKey CompanyStatus { get; internal set; }
        //public VocabularyKey CompanyType { get; internal set; }
        public VocabularyKey DateOfCreation { get; internal set; }
    }
}
