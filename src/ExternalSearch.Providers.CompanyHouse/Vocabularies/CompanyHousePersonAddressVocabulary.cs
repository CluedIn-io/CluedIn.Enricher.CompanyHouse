// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompanyHousePersonAddressVocabulary.cs" company="Clued In">
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
    public class CompanyHousePersonAddressVocabulary : SimpleVocabulary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyHousePersonAddressVocabulary "/> class.
        /// </summary>
        public CompanyHousePersonAddressVocabulary()
        {
            VocabularyName = "CompanyHouse Person Address";
            KeyPrefix = "companyHouse.person.address";
            KeySeparator = ".";
            Grouping = EntityType.Geography;


            CareOf = Add(new VocabularyKey("careOf", VocabularyKeyDataType.Text));
            Region = Add(new VocabularyKey("region", VocabularyKeyDataType.Text));
            Postal_code = Add(new VocabularyKey("postal_code", VocabularyKeyDataType.Text));
            Premises = Add(new VocabularyKey("premises", VocabularyKeyDataType.Text));
            Country = Add(new VocabularyKey("country", VocabularyKeyDataType.Text));
            Locality = Add(new VocabularyKey("locality", VocabularyKeyDataType.Text));
            AddressLine1 = Add(new VocabularyKey("addressLine1", VocabularyKeyDataType.Text));
            AddressLine2 = Add(new VocabularyKey("addressLine2", VocabularyKeyDataType.Text));
        }

        public VocabularyKey CareOf { get; set; }
        public VocabularyKey Region { get; set; }
        public VocabularyKey Postal_code { get; set; }
        public VocabularyKey Premises { get; set; }
        public VocabularyKey Country { get; set; }
        public VocabularyKey Locality { get; set; }
        public VocabularyKey AddressLine1 { get; set; }
        public VocabularyKey AddressLine2 { get; set; }
    }
}
