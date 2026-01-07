using CluedIn.Core.Data;
using CluedIn.Core.Data.Vocabularies;

namespace CluedIn.ExternalSearch.Providers.CompanyHouse.Vocabularies;

/// <summary>The accounts vocabulary.</summary>
/// <seealso cref="CluedIn.Core.Data.Vocabularies.SimpleVocabulary" />
public class CompanyHouseOrgAccountsVocabulary : SimpleVocabulary
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CompanyHouseOrgAccountsVocabulary "/> class.
    /// </summary>
    public CompanyHouseOrgAccountsVocabulary()
    {
        VocabularyName = "CompanyHouse Accounts";
        KeyPrefix = "companyHouse.accounts";
        KeySeparator = ".";
        Grouping = EntityType.Account;


        AccountingReferenceDateDay = Add(new VocabularyKey("accountingReferenceDateDay"));
        AccountingReferenceDateMonth = Add(new VocabularyKey("accountingReferenceDateMonth"));
        LastAccountsMadeUpTo = Add(new VocabularyKey("lastAccountsMadeUpTo"));
        LastAccountsPeriodEndOn = Add(new VocabularyKey("lastAccountsPeriodEndOn"));
        LastAccountsPeriodStartOn = Add(new VocabularyKey("lastAccountsPeriodStartOn"));
        LastAccountsType = Add(new VocabularyKey("lastAccountsType"));
        NextAccountsDueOn = Add(new VocabularyKey("nextAccountsDueOn"));
        NextAccountsOverdue = Add(new VocabularyKey("nextAccountsOverdue"));
        NextAccountsPeriodEndOn = Add(new VocabularyKey("nextAccountsPeriodEndOn"));
        NextAccountsPeriodStartOn = Add(new VocabularyKey("nextAccountsPeriodStartOn"));
        NextDue = Add(new VocabularyKey("nextDue"));
        NextMadeUpTo = Add(new VocabularyKey("nextMadeUpTo"));
        Overdue = Add(new VocabularyKey("overdue"));
    }

    public VocabularyKey AccountingReferenceDateDay { get; set; }
    public VocabularyKey AccountingReferenceDateMonth { get; set; }
    public VocabularyKey LastAccountsMadeUpTo { get; set; }
    public VocabularyKey LastAccountsPeriodEndOn { get; set; }
    public VocabularyKey LastAccountsPeriodStartOn { get; set; }
    public VocabularyKey LastAccountsType { get; set; }
    public VocabularyKey NextAccountsDueOn { get; set; }
    public VocabularyKey NextAccountsOverdue { get; set; }
    public VocabularyKey NextAccountsPeriodEndOn { get; set; }
    public VocabularyKey NextAccountsPeriodStartOn { get; set; }
    public VocabularyKey NextDue { get; set; }
    public VocabularyKey NextMadeUpTo { get; set; }
    public VocabularyKey Overdue { get; set; }
}
