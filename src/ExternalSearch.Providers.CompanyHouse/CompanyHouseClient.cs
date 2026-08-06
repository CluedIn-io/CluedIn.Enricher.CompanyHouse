using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using CluedIn.ExternalSearch.Providers.CompanyHouse.Model;
using RestSharp;
using RestSharp.Serializers.Json;

namespace CluedIn.ExternalSearch.Providers.CompanyHouse
{
    public class CompanyHouseClient
    {
        private readonly RestClient _client;
        private readonly RestRequest _request;

        public CompanyHouseClient(CompanyHouseExternalSearchJobData jobData)
        {
            _client = new RestClient("https://api.companieshouse.gov.uk",
                configureSerialization: s => s.UseSystemTextJson(new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString
                }));
            _request = new RestRequest { Method = Method.Get };
            _request.AddHeader("Authorization", "Basic " + Base64Encode(jobData.ApiKey));
        }

        public List<CompanySearchResultItem> GetCompanies(string name)
        {
            _request.Resource = $"search/companies?q={name}";
            var result = ExecuteWithRateLimitHandling<CompanySearchResponse>(_request);
            return result.StatusCode != HttpStatusCode.OK ? null : result.Data?.items;
        }

        public CompanyNew GetCompany(string companyNumber)
        {
            _request.Resource = $"company/{companyNumber}";
            var result = ExecuteWithRateLimitHandling<CompanyNew>(_request);
            return result.StatusCode != HttpStatusCode.OK ? null : result.Data;
        }

        // TODO: not used
        public List<Contact> GetOfficers(string companyNumber)
        {
            _request.Resource = $"company/{companyNumber}/officers";
            var result = ExecuteWithRateLimitHandling<ContactList>(_request);
            return result.StatusCode != HttpStatusCode.OK ? null : result.Data?.items;
        }

        // TODO: not used
        public AppointmentResponse GetAppointment(string regNumber)
        {
            _request.Resource = $"officers/{regNumber}/appointments";
            var result = ExecuteWithRateLimitHandling<AppointmentResponse>(_request);
            return result.StatusCode != HttpStatusCode.OK ? null : result.Data;
        }

        // TODO: not used
        public OfficerResponse GetDisqualifiedNaturalResponse(string regNumber)
        {
            _request.Resource = $"disqualified-officers/natural/{regNumber}";
            var result = ExecuteWithRateLimitHandling<OfficerResponse>(_request);
            return result.StatusCode != HttpStatusCode.OK ? null : result.Data;
        }

        // TODO: not used
        public OfficerResponse GetDisqualifiedCorporateResponse(string regNumber)
        {
            _request.Resource = $"disqualified-officers/corporate/{regNumber}";
            var result = ExecuteWithRateLimitHandling<OfficerResponse>(_request);
            return result.StatusCode != HttpStatusCode.OK ? null : result.Data;
        }

        private RestResponse<T> ExecuteWithRateLimitHandling<T>(RestRequest request, int maxRetries = 3)
        {
            for (var attempt = 0; attempt <= maxRetries; attempt++)
            {
                var result = _client.ExecuteAsync<T>(request).Result;

                if (result.StatusCode != HttpStatusCode.TooManyRequests)
                {
                    return result;
                }

                if (attempt == maxRetries)
                {
                    throw new WebException("TooManyRequests");
                }

                var waitTime = GetRetryAfterDelay(result);
                Thread.Sleep(waitTime);
            }

            return null;
        }

        private static TimeSpan GetRetryAfterDelay<T>(RestResponse<T> response)
        {
            // ReSharper disable once StringLiteralTypo
            var resetHeader = response.Headers?.FirstOrDefault(h => string.Equals(h.Name, "X-Ratelimit-Reset", StringComparison.OrdinalIgnoreCase));
            if (resetHeader?.Value is { } resetValue && long.TryParse(resetValue, out var resetEpoch))
            {
                var resetTime = DateTimeOffset.FromUnixTimeSeconds(resetEpoch);
                var delay = resetTime - DateTimeOffset.UtcNow;
                if (delay > TimeSpan.Zero)
                {
                    return delay;
                }
            }

            // ReSharper disable once CommentTypo
            // Fallback: use X-Ratelimit-Window or default to 60 seconds
            // ReSharper disable once StringLiteralTypo
            var windowHeader = response.Headers?.FirstOrDefault(h => string.Equals(h.Name, "X-Ratelimit-Window", StringComparison.OrdinalIgnoreCase));
            if (windowHeader?.Value is not { } windowValue)
            {
                return TimeSpan.FromSeconds(60);
            }

            if (windowValue.EndsWith('m') && int.TryParse(windowValue.TrimEnd('m'), out var minutes))
            {
                return TimeSpan.FromMinutes(minutes);
            }

            if (windowValue.EndsWith('s') && int.TryParse(windowValue.TrimEnd('s'), out var seconds))
            {
                return TimeSpan.FromSeconds(seconds);
            }

            return TimeSpan.FromSeconds(60);
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
    }
}
