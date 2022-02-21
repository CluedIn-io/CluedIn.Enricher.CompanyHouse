// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompanyHouseOrgAddressVocabulary.cs" company="Clued In">
//   Copyright Clued In
// </copyright>
// <summary>
//   Defines the CompanyHouseOrgAddressVocabulary type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using CluedIn.Core.Data;
using CluedIn.Core.Data.Vocabularies;

namespace CluedIn.ExternalSearch.Providers.CompanyHouse.Vocabularies
{
    /// <summary>The address vocabulary.</summary>
    /// <seealso cref="CluedIn.Core.Data.Vocabularies.SimpleVocabulary" />
    public class CompanyHouseOrgAddressVocabulary : SimpleVocabulary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyHouseOrgAddressVocabulary "/> class.
        /// </summary>
        public CompanyHouseOrgAddressVocabulary()
        {
            VocabularyName = "CompanyHouse Address";
            KeyPrefix = "companyHouse.address";
            KeySeparator = ".";
            Grouping = EntityType.Geography;


            AddressLine1 = Add(new VocabularyKey("addressLine1", VocabularyKeyDataType.Text));
            AddressLine2 = Add(new VocabularyKey("addressLine2", VocabularyKeyDataType.Text));
            Locality = Add(new VocabularyKey("locality", VocabularyKeyDataType.Text));
            PostCode = Add(new VocabularyKey("postCode", VocabularyKeyDataType.Text));
        }

        public VocabularyKey AddressLine1 { get; set; }
        public VocabularyKey AddressLine2 { get; set; }
        public VocabularyKey Locality { get; set; }
        public VocabularyKey PostCode { get; set; }
    }
}
