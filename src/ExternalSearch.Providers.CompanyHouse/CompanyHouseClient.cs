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
            client = new RestClient("https://api.companieshouse.gov.uk");
            request = new RestRequest();
            request.Method = Method.GET;
            request.AddHeader("Authorization", "Basic " + Base64Encode("_Y-9-pOnf-c0o4_bIZpjGASw8FYrNP-nVK2DvEbn"));
        }

        public List<CompanySearchResultItem> GetCompanies(string name)
        {
            request.Resource = $"search/companies?q={name}";
            var result = client.ExecuteAsync<CompanySearchResponse>(request).Result;
            if (result.StatusCode != System.Net.HttpStatusCode.OK)
                return null;

            return result.Data.items;
        }

        public CompanyNew GetCompany(string companyNumber)
        {
            request.Resource = $"company/{companyNumber}";
            var result = client.ExecuteAsync<CompanyNew>(request).Result;
            if (result.StatusCode != System.Net.HttpStatusCode.OK)
                return null;

            return result.Data;
        }

        public List<Contact> GetOfficers(string companyNumber)
        {
            request.Resource = $"company/{companyNumber}/officers";
            var result = client.ExecuteAsync<ContactList>(request).Result;
            if (result.StatusCode != System.Net.HttpStatusCode.OK)
                return null;

            return result.Data?.items;
        }

        public AppointmentResponse GetAppointment(string regNumber)
        {
            request.Resource = $"officers/{regNumber}/appointments";
            var result = client.ExecuteAsync<AppointmentResponse>(request).Result;
            if (result.StatusCode != System.Net.HttpStatusCode.OK)
                return null;

            return result.Data;
        }

        public OfficerResponse GetDisqualifiedNaturalResponse(string regNumber)
        {
            request.Resource = $"disqualified-officers/natural/{regNumber}";
            var result = client.ExecuteAsync<OfficerResponse>(request).Result;
            if (result.StatusCode != System.Net.HttpStatusCode.OK)
                return null;

            return result.Data;
        }

        public OfficerResponse GetDisqualifiedCorporateResponse(string regNumber)
        {
            request.Resource = $"disqualified-officers/corporate/{regNumber}";
            var result = client.ExecuteAsync<OfficerResponse>(request).Result;
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
