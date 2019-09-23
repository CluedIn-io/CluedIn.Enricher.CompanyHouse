using System.Collections.Generic;

namespace CluedIn.ExternalSearch.Providers.CompanyHouse.Model
{
    public class Company
    {
        public string kind { get; set; }
        public string company_type { get; set; }
        public Matches matches { get; set; }
        public string description { get; set; }
        public CompanyAddress address { get; set; }
        public CompanyAddress mainCompanyAddress { get; set; }
        public string date_of_creation { get; set; }
        public string company_status { get; set; }
        public string company_number { get; set; }
        public string title { get; set; }
        public Links links { get; set; }
        public ExceptionResponse exceptionResponse { get; set; }
        public CompanyRegisterResponse companyRegisterResponse { get; set; }
        public PersonControlStatements personControlStatements { get; set; }
        public ControlPersonResponse controlPersonResponse { get; set; }
        public ChargeResponse chargeResponse { get; set; }
        public InsolvencyResponse insolvencyResponse { get; set; }
        public UkEstablishmentResponse ukEstablishmentResponse { get; set; }
        public string address_snippet { get; set; }
        public string snippet { get; set; }
        public FilingHistory filingHistory { get; set; }
        public List<string> description_identifier { get; set; }
    }
}