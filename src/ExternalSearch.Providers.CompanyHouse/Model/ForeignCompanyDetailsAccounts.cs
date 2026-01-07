#nullable enable

namespace CluedIn.ExternalSearch.Providers.CompanyHouse.Model;

public class ForeignCompanyDetailsAccounts
{
    public AccountPeriodFrom? account_period_from { get; set; }
    public AccountPeriodTo? account_period_to { get; set; }
    public MustFileWithin? must_file_within { get; set; }
}
