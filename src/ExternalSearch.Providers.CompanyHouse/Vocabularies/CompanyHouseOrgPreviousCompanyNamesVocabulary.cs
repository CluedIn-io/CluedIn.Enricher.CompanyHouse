using CluedIn.Core.Data.Vocabularies;

namespace CluedIn.ExternalSearch.Providers.CompanyHouse.Vocabularies;

public class CompanyHouseOrgPreviousCompanyNameVocabulary : SimpleVocabulary
{
    public CompanyHouseOrgPreviousCompanyNameVocabulary()
    {
        VocabularyName = "CompanyHouse PreviousCompanyName";
        KeyPrefix = "companyHouse.previousCompanyName";
        KeySeparator = ".";
        Grouping = "/PreviousCompanyName";
    }
}
