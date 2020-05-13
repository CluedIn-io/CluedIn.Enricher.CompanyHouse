using System.Collections.Generic;
using CluedIn.ExternalSearch.Providers.CompanyHouse.Model;
using RestSharp;

namespace CluedIn.ExternalSearch.Providers.CompanyHouse
{
    public class CompanyHouseClient
    {
        private RestClient client;
        private RestRequest request;

        public CompanyHouseClient()
        {
            this.client = new RestClient("https://api.companieshouse.gov.uk");
            this.request = new RestRequest();
            this.request.Method = Method.GET;
            this.request.AddHeader("Authorization", "Basic " + Base64Encode("_Y-9-pOnf-c0o4_bIZpjGASw8FYrNP-nVK2DvEbn"));
        }

        public List<CompanySearchResultItem> GetCompanies(string name)
        {
            this.request.Resource = $"search/companies?q={name}";
            var result = this.client.ExecuteAsync<CompanySearchResponse>(this.request).Result;
            if (result.StatusCode != System.Net.HttpStatusCode.OK)
                return null;

            return result.Data.items;
        }

        public CompanyNew GetCompany(string companyNumber)
        {
            this.request.Resource = $"company/{companyNumber}";
            var result = this.client.ExecuteAsync<CompanyNew>(this.request).Result;
            if (result.StatusCode != System.Net.HttpStatusCode.OK)
                return null;

            return result.Data;
        }

        public List<Contact> GetOfficers(string companyNumber)
        {
            this.request.Resource = $"company/{companyNumber}/officers";
            var result = this.client.ExecuteAsync<ContactList>(this.request).Result;
            if (result.StatusCode != System.Net.HttpStatusCode.OK)
                return null;

            return result.Data?.items;
        }

        public AppointmentResponse GetAppointment(string regNumber)
        {
            this.request.Resource = $"officers/{regNumber}/appointments";
            var result = this.client.ExecuteAsync<AppointmentResponse>(this.request).Result;
            if (result.StatusCode != System.Net.HttpStatusCode.OK)
                return null;

            return result.Data;
        }

        public OfficerResponse GetDisqualifiedNaturalResponse(string regNumber)
        {
            this.request.Resource = $"disqualified-officers/natural/{regNumber}";
            var result = this.client.ExecuteAsync<OfficerResponse>(this.request).Result;
            if (result.StatusCode != System.Net.HttpStatusCode.OK)
                return null;

            return result.Data;
        }

        public OfficerResponse GetDisqualifiedCorporateResponse(string regNumber)
        {
            this.request.Resource = $"disqualified-officers/corporate/{regNumber}";
            var result = this.client.ExecuteAsync<OfficerResponse>(this.request).Result;
            if (result.StatusCode != System.Net.HttpStatusCode.OK)
                return null;

            return result.Data;
        }


        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}
