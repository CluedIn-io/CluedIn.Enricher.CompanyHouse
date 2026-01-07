#nullable enable

using System.Collections.Generic;

namespace CluedIn.ExternalSearch.Providers.CompanyHouse.Model
{
    public class CompanyNew
    {
        public Accounts? accounts { get; set; }
        public AnnualReturn? annual_return { get; set; }
        public BranchCompanyDetails? branch_company_details { get; set; }
        public bool can_file { get; set; }
        public string? company_name { get; set; }
        public string? company_number { get; set; }
        public string? original_query_name { get; set; }
        public string? company_status { get; set; }
        public string? company_status_detail { get; set; }
        public ConfirmationStatement? confirmation_statement { get; set; }
        public List<CorporateAnnotation>? corporate_annotation { get; set; }
        public string? date_of_cessation { get; set; }
        public string? date_of_creation { get; set; }
        public string? etag { get; set; }
        public string? external_registration_number { get; set; }
        public ForeignCompanyDetails? foreign_company_details { get; set; }
        public bool? has_been_liquidated { get; set; }
        public bool? has_charges { get; set; }
        public bool? has_insolvency_history { get; set; }
        public bool? is_community_interest_company { get; set; }
        public string? jurisdiction { get; set; }
        public string? last_full_members_list_date { get; set; }
        public Links? links { get; set; }
        public string? partial_data_available { get; set; }
        public List<PreviousCompanyName>? previous_company_names { get; set; }
        public RegisteredOfficeAddress? registered_office_address { get; set; }
        public bool? registered_office_is_in_dispute { get; set; }
        public ServiceAddress? service_address { get; set; }
        public List<string>? sic_codes { get; set; }
        public List<string>? subtype { get; set; }
        public int? super_secure_managing_officer_count { get; set; }
        public string? type { get; set; }
        public bool? undeliverable_registered_office_address { get; set; }
    }
}
