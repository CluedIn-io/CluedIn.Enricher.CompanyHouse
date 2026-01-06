using CluedIn.Core.Data.Vocabularies;

namespace CluedIn.ExternalSearch.Providers.CompanyHouse.Vocabularies;

public class CompanyHouseOrgCorporateAnnotationVocabulary : SimpleVocabulary
{
    public CompanyHouseOrgCorporateAnnotationVocabulary()
    {
        VocabularyName = "CompanyHouse CorporateAnnotation";
        KeyPrefix = "companyHouse.corporateAnnotation";
        KeySeparator = ".";
        Grouping = "/CorporateAnnotation";
    }
}
