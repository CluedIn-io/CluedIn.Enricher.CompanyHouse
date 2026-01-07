#nullable enable

namespace CluedIn.ExternalSearch.Providers.CompanyHouse.Model;

public class ForeignCompanyDetails
{
    public AccountingRequirement? accounting_requirement { get; set; }
    public ForeignCompanyDetailsAccounts? accounts { get; set; }
    public string? business_activity { get; set; }
    public string? company_type { get; set; }
    public string? governed_by { get; set; }
    public bool? is_a_credit_finance_institution { get; set; }
    public OriginatingRegistry? originating_registry { get; set; }
    public string? registration_number { get; set; }
}
