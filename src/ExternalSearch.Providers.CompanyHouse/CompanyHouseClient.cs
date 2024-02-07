using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using CluedIn.ExternalSearch.Providers.CompanyHouse.Model;
using RestSharp;

namespace CluedIn.ExternalSearch.Providers.CompanyHouse
{
    public class CompanyHouseClient
    {
        private readonly RestClient _client;
        private readonly RestRequest _request;

        public CompanyHouseClient(CompanyHouseExternalSearchJobData jobData)
        {
            _client = new RestClient("https://api.companieshouse.gov.uk");
            _request = new RestRequest { Method = Method.GET };
            _request.AddHeader("Authorization", "Basic " + Base64Encode(jobData.ApiKey));
        }

        public List<CompanySearchResultItem> GetCompanies(string name)
        {
            _request.Resource = $"search/companies?q={name}";
            var result = _client.ExecuteAsync<CompanySearchResponse>(_request).Result;
            return result.StatusCode != HttpStatusCode.OK ? null : result.Data.items;
        }

        public CompanyNew GetCompany(string companyNumber)
        {
            _request.Resource = $"company/{companyNumber}";
            var result = _client.ExecuteAsync<CompanyNew>(_request).Result;
            return result.StatusCode != HttpStatusCode.OK ? null : result.Data;
        }

        // TODO: not used
        public List<Contact> GetOfficers(string companyNumber)
        {
            _request.Resource = $"company/{companyNumber}/officers";
            var result = _client.ExecuteAsync<ContactList>(_request).Result;
            return result.StatusCode != HttpStatusCode.OK ? null : result.Data?.items;
        }

        // TODO: not used
        public AppointmentResponse GetAppointment(string regNumber)
        {
            _request.Resource = $"officers/{regNumber}/appointments";
            var result = _client.ExecuteAsync<AppointmentResponse>(_request).Result;
            return result.StatusCode != HttpStatusCode.OK ? null : result.Data;
        }

        // TODO: not used
        public OfficerResponse GetDisqualifiedNaturalResponse(string regNumber)
        {
            _request.Resource = $"disqualified-officers/natural/{regNumber}";
            var result = _client.ExecuteAsync<OfficerResponse>(_request).Result;
            return result.StatusCode != HttpStatusCode.OK ? null : result.Data;
        }

        // TODO: not used
        public OfficerResponse GetDisqualifiedCorporateResponse(string regNumber)
        {
            _request.Resource = $"disqualified-officers/corporate/{regNumber}";
            var result = _client.ExecuteAsync<OfficerResponse>(_request).Result;
            return result.StatusCode != HttpStatusCode.OK ? null : result.Data;
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
    }
}
