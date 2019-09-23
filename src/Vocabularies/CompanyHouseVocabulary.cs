// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClearBitVocabulary.cs" company="Clued In">
//   Copyright Clued In
// </copyright>
// <summary>
//   Defines the ClearBitVocabulary type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CluedIn.ExternalSearch.Providers.CompanyHouse.Vocabularies
{
    /// <summary>The clear bit vocabulary.</summary>
    public static class CompanyHouseVocabulary
    {
        /// <summary>
        /// Initializes static members of the <see cref="CompanyHouseVocabulary" /> class.
        /// </summary>
        static CompanyHouseVocabulary()
        {
            Organization = new CompanyHouseOrganizationVocabulary();
            Person = new CompanyHousePersonVocabulary();
        }

        /// <summary>Gets the organization.</summary>
        /// <value>The organization.</value>
        public static CompanyHouseOrganizationVocabulary Organization { get; private set; }

        /// <summary>Gets the person.</summary>
        /// <value>The person.</value>
        public static CompanyHousePersonVocabulary Person { get; private set; }
    }
}