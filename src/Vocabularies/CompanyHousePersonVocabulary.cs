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
            this.VocabularyName = "CompanyHouse Person";
            this.KeyPrefix = "companyHouse.person";
            this.KeySeparator   = ".";
            this.Grouping       = EntityType.Person;

            this.Nationality        = this.Add(new VocabularyKey("nationality", VocabularyKeyDataType.Text));
            this.NotifiedOn         = this.Add(new VocabularyKey("notifiedOn", VocabularyKeyDataType.Text));
            this.AddressLine1       = this.Add(new VocabularyKey("addressLine1", VocabularyKeyDataType.Text));
            this.AddressLine2       = this.Add(new VocabularyKey("addressLine2", VocabularyKeyDataType.Text));
            this.Country            = this.Add(new VocabularyKey("country", VocabularyKeyDataType.Text));
            this.Locality           = this.Add(new VocabularyKey("locality", VocabularyKeyDataType.Text));
            this.PostalCode         = this.Add(new VocabularyKey("postalCode", VocabularyKeyDataType.Text));
            this.Premises           = this.Add(new VocabularyKey("premises", VocabularyKeyDataType.Text));
            this.Region             = this.Add(new VocabularyKey("region", VocabularyKeyDataType.Text));
            this.CeasedOn           = this.Add(new VocabularyKey("ceasedOn", VocabularyKeyDataType.Text));
            this.CountryOfResidence = this.Add(new VocabularyKey("countryOfResidence", VocabularyKeyDataType.Text));
            this.DateOfBirthDay     = this.Add(new VocabularyKey("dateOfBirthDay", VocabularyKeyDataType.Text));
            this.DateOfBirthMonth   = this.Add(new VocabularyKey("dateOfBirthMonth", VocabularyKeyDataType.Text));
            this.DateOfBirthYear    = this.Add(new VocabularyKey("dateOfBirthYear", VocabularyKeyDataType.Text));
            this.Forename           = this.Add(new VocabularyKey("forename", VocabularyKeyDataType.Text));
            this.OtherForeNames     = this.Add(new VocabularyKey("otherForeNames", VocabularyKeyDataType.Text));
            this.Surname            = this.Add(new VocabularyKey("surname", VocabularyKeyDataType.Text));
            this.Title              = this.Add(new VocabularyKey("title", VocabularyKeyDataType.Text));
        }

        public VocabularyKey Nationality { get; internal set; }
        public VocabularyKey NotifiedOn { get; internal set; }
        public VocabularyKey AddressLine1 { get; internal set; }
        public VocabularyKey AddressLine2 { get; internal set; }
        public VocabularyKey Country { get; internal set; }
        public VocabularyKey Locality { get; internal set; }
        public VocabularyKey PostalCode { get; internal set; }
        public VocabularyKey Premises { get; internal set; }
        public VocabularyKey Region { get; internal set; }
        public VocabularyKey CeasedOn { get; internal set; }
        public VocabularyKey CountryOfResidence { get; internal set; }
        public VocabularyKey DateOfBirthDay { get; internal set; }
        public VocabularyKey DateOfBirthMonth { get; internal set; }
        public VocabularyKey DateOfBirthYear { get; internal set; }
        public VocabularyKey Forename { get; internal set; }
        public VocabularyKey OtherForeNames { get; internal set; }
        public VocabularyKey Surname { get; internal set; }
        public VocabularyKey Title { get; internal set; }
    }
}
