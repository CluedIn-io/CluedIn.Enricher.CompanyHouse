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
    public class CompanyHousePersonVocabulary : SimpleVocabulary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyHousePersonVocabulary"/> class.
        /// </summary>
        public CompanyHousePersonVocabulary()
        {
            VocabularyName = "CompanyHouse Person";
            KeyPrefix = "companyHouse.person";
            KeySeparator   = ".";
            Grouping       = EntityType.Person;

            Name = Add(new VocabularyKey("name", VocabularyKeyDataType.Text));
            Officer_role = Add(new VocabularyKey("officer_role", VocabularyKeyDataType.Text));
            Appointed_on = Add(new VocabularyKey("appointed_on", VocabularyKeyDataType.Text));
            Date_of_birth = Add(new VocabularyKey("date_of_birth", VocabularyKeyDataType.Text));
            Country_of_residence = Add(new VocabularyKey("country_of_residence", VocabularyKeyDataType.Text));
            Occupation = Add(new VocabularyKey("occupation", VocabularyKeyDataType.Text));
            Nationality = Add(new VocabularyKey("nationality", VocabularyKeyDataType.Text));
            Resigned_on = Add(new VocabularyKey("resigned_on", VocabularyKeyDataType.Text));
            Address = Add(new CompanyHousePersonAddressVocabulary().AsCompositeKey("address"));
        }

        public VocabularyKey Name { get; internal set; }
        public VocabularyKey Officer_role { get; internal set; }
        public VocabularyKey Appointed_on { get; internal set; }
        public VocabularyKey Date_of_birth { get; internal set; }
        public VocabularyKey Country_of_residence { get; internal set; }
        public VocabularyKey Occupation { get; internal set; }
        public VocabularyKey Nationality { get; internal set; }
        public VocabularyKey Resigned_on { get; internal set; }

        public CompanyHousePersonAddressVocabulary Address { get; protected set; }
    }
}
