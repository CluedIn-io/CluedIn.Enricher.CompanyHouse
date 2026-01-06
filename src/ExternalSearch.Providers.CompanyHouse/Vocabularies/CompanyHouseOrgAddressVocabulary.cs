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

namespace CluedIn.ExternalSearch.Providers.CompanyHouse.Vocabularies;

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


        RegisteredOfficeAddressLine1 = Add(new VocabularyKey("registeredOfficeAddressLine1"));
        RegisteredOfficeAddressLine2 = Add(new VocabularyKey("registeredOfficeAddressLine2"));
        RegisteredOfficeCareOf = Add(new VocabularyKey("registeredOfficeCareOf"));
        RegisteredOfficeCountry = Add(new VocabularyKey("registeredOfficeCountry"));
        RegisteredOfficeLocality = Add(new VocabularyKey("registeredOfficeLocality"));
        RegisteredOfficePoBox = Add(new VocabularyKey("registeredOfficePoBox"));
        RegisteredOfficePostalCode = Add(new VocabularyKey("registeredOfficePostalCode"));
        RegisteredOfficePremises = Add(new VocabularyKey("registeredOfficePremises"));
        RegisteredOfficeRegion = Add(new VocabularyKey("registeredOfficeRegion"));
        ServiceAddressLine1 = Add(new VocabularyKey("serviceAddressLine1"));
        ServiceAddressLine2 = Add(new VocabularyKey("serviceAddressLine2"));
        ServiceAddressCareOf = Add(new VocabularyKey("serviceAddressCareOf"));
        ServiceAddressCountry = Add(new VocabularyKey("serviceAddressCountry"));
        ServiceAddressLocality = Add(new VocabularyKey("serviceAddressLocality"));
        ServiceAddressPoBox = Add(new VocabularyKey("serviceAddressPoBox"));
        ServiceAddressPostalCode = Add(new VocabularyKey("serviceAddressPostalCode"));
        ServiceAddressRegion = Add(new VocabularyKey("serviceAddressRegion"));
    }

    public VocabularyKey RegisteredOfficeAddressLine1 { get; set; }
    public VocabularyKey RegisteredOfficeAddressLine2 { get; set; }
    public VocabularyKey RegisteredOfficeCareOf { get; set; }
    public VocabularyKey RegisteredOfficeCountry { get; set; }
    public VocabularyKey RegisteredOfficeLocality { get; set; }
    public VocabularyKey RegisteredOfficePoBox { get; set; }
    public VocabularyKey RegisteredOfficePostalCode { get; set; }
    public VocabularyKey RegisteredOfficePremises{ get; set; }
    public VocabularyKey RegisteredOfficeRegion { get; set; }
    public VocabularyKey ServiceAddressLine1 { get; set; }
    public VocabularyKey ServiceAddressLine2 { get; set; }
    public VocabularyKey ServiceAddressCareOf { get; set; }
    public VocabularyKey ServiceAddressCountry { get; set; }
    public VocabularyKey ServiceAddressLocality { get; set; }
    public VocabularyKey ServiceAddressPoBox { get; set; }
    public VocabularyKey ServiceAddressPostalCode { get; set; }
    public VocabularyKey ServiceAddressRegion { get; set; }
}
