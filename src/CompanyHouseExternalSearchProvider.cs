// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClearBitExternalSearchProvider.cs" company="Clued In">
//   Copyright Clued In
// </copyright>
// <summary>
//   Defines the ClearBitExternalSearchProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using CluedIn.Core;
using CluedIn.Core.Data;
using CluedIn.Core.Data.Parts;
using CluedIn.ExternalSearch.Providers.CompanyHouse.Vocabularies;
using RestSharp;
using CluedIn.ExternalSearch.Providers.CompanyHouse.Model;

namespace CluedIn.ExternalSearch.Providers.CompanyHouse
{
    /// <summary>The clear bit external search provider.</summary>
    /// <seealso cref="CluedIn.ExternalSearch.ExternalSearchProviderBase" />
    public class CompanyHouseExternalSearchProvider : ExternalSearchProviderBase
    {
        /**********************************************************************************************************
         * CONSTRUCTORS
         **********************************************************************************************************/
        // TODO: Move Magic GUID to constants
        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyHouseExternalSearchProvider" /> class.
        /// </summary>
        public CompanyHouseExternalSearchProvider()
            : base(new Guid("{2A9E52AE-425B-4351-8AF5-6D374E8CC1A5}"), EntityType.Organization)
        {
        }

        /**********************************************************************************************************
         * METHODS
         **********************************************************************************************************/

        /// <summary>Builds the queries.</summary>
        /// <param name="context">The context.</param>
        /// <param name="request">The request.</param>
        /// <returns>The search queries.</returns>
        public override IEnumerable<IExternalSearchQuery> BuildQueries(ExecutionContext context, IExternalSearchRequest request)
        {
            yield break;

            #region TODO CompanyHouseExternalSearchProvider.BuildQueries(ExecutionContext context, IExternalSearchRequest request) is disabled in code ... review

            //if (!this.Accepts(request.EntityMetaData.EntityType))
            //    yield break;

            //var existingResults = request.GetQueryResults<Company>(this).ToList();

            //Func<string, bool> idFilter = value => existingResults.Any(r => string.Equals(r.Data.company_number, value, StringComparison.InvariantCultureIgnoreCase));
            //Func<string, bool> nameFilter = value => existingResults.Any(r => string.Equals(r.Data.title, value, StringComparison.InvariantCultureIgnoreCase));

            //var entityType = request.EntityMetaData.EntityType;
            //var companyHouseNumber = request.QueryParameters.GetValue(CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.CodesCompanyHouse, new HashSet<string>());

            //var country = request.EntityMetaData.Properties.ContainsKey(CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.AddressCountryCode) ? request.EntityMetaData.Properties[CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.AddressCountryCode].ToLowerInvariant() : string.Empty;
            //var state = request.EntityMetaData.Properties.ContainsKey(CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.AddressState) ? request.EntityMetaData.Properties[CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.AddressState].ToLowerInvariant() : string.Empty;
            //var city = request.EntityMetaData.Properties.ContainsKey(CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.AddressCity) ? request.EntityMetaData.Properties[CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.AddressCity].ToLowerInvariant() : string.Empty;

            //// TODO: Should put a filter here to only lookup UK based companies.
            //if (country.Contains("uk") || country.Contains("united kingdom") || state.Contains("england") || city.Contains("london"))
            //{
            //    var organizationName = request.QueryParameters.GetValue(CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.OrganizationName, new HashSet<string>());

            //    if (!string.IsNullOrEmpty(request.EntityMetaData.Name))
            //        organizationName.Add(request.EntityMetaData.Name);
            //    if (!string.IsNullOrEmpty(request.EntityMetaData.DisplayName))
            //        organizationName.Add(request.EntityMetaData.DisplayName);

            //    if (organizationName != null)
            //    {
            //        var values = organizationName.Select(NameNormalization.Normalize).ToHashSet();

            //        foreach (var value in values.Where(v => !nameFilter(v)))
            //            yield return new ExternalSearchQuery(this, entityType, ExternalSearchQueryParameter.Name, value);
            //    }
            //}

            //foreach (var value in companyHouseNumber.Where(v => !idFilter(v)))
            //    yield return new ExternalSearchQuery(this, entityType, ExternalSearchQueryParameter.Identifier, value);

            #endregion
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>Executes the search.</summary>
        /// <param name="context">The context.</param>
        /// <param name="query">The query.</param>
        /// <returns>The results.</returns>
        public override IEnumerable<IExternalSearchQueryResult> ExecuteSearch(ExecutionContext context, IExternalSearchQuery query)
        {
            var id = query.QueryParameters.ContainsKey(ExternalSearchQueryParameter.Identifier) ? query.QueryParameters[ExternalSearchQueryParameter.Identifier].FirstOrDefault() : null;
            if (string.IsNullOrEmpty(id))
            {
                var name = query.QueryParameters.ContainsKey(ExternalSearchQueryParameter.Name) ? query.QueryParameters[ExternalSearchQueryParameter.Name].FirstOrDefault() : null;
                if (!string.IsNullOrEmpty(name))
                {
                    var client = new RestClient("https://api.companieshouse.gov.uk");
                    var request = new RestRequest(string.Format("search/companies?q={0}", name), Method.GET);
                    request.AddHeader("Authorization", "Basic " + Base64Encode("_Y-9-pOnf-c0o4_bIZpjGASw8FYrNP-nVK2DvEbn"));
                    var response = client.ExecuteTaskAsync<CompanySearchResponse>(request).Result;
                    if (response.Data.items != null)
                        foreach (var searchResult in response.Data.items)
                        {
                            var companyAndContact = new CompanyAndContacts();

                            var companyRequest = new RestRequest(string.Format("company/{0}", searchResult.company_number), Method.GET);
                            companyRequest.AddHeader("Authorization", "Basic " + Base64Encode("_Y-9-pOnf-c0o4_bIZpjGASw8FYrNP-nVK2DvEbn"));
                            var companyResponse = client.ExecuteTaskAsync<CompanyNew>(companyRequest).Result;
                            companyAndContact.Company = companyResponse.Data;
                            yield return new ExternalSearchQueryResult<CompanyAndContacts>(query, companyAndContact);

                            var lRequest = new RestRequest(string.Format("/company/{0}/officers", searchResult.company_number), Method.GET);
                            lRequest.AddHeader("Authorization", "Basic " + Base64Encode("_Y-9-pOnf-c0o4_bIZpjGASw8FYrNP-nVK2DvEbn"));
                            lRequest.AddParameter("items_per_page", 100);
                            lRequest.AddParameter("start_index", 0);
                            var lResponse = client.ExecuteTaskAsync<ContactList>(lRequest).Result;
                            if (lResponse.StatusCode == HttpStatusCode.OK)
                            {
                                if (lResponse.Data != null)
                                {
                                    var officers = new List<Contact>();
                                    if (lResponse.Data.items != null)
                                        foreach (var officer in lResponse.Data.items)
                                        {
                                            {
                                                var officerRequest = new RestRequest(string.Format("officers/{0}/appointments", officer.identification.registration_number), Method.GET);
                                                officerRequest.AddHeader("Authorization", "Basic " + Base64Encode("_Y-9-pOnf-c0o4_bIZpjGASw8FYrNP-nVK2DvEbn"));
                                                officerRequest.AddParameter("items_per_page", 100);
                                                officerRequest.AddParameter("start_index", 0);
                                                var officerResponse = client.ExecuteTaskAsync<AppointmentResponse>(officerRequest).Result;
                                                if (officerResponse.StatusCode == HttpStatusCode.OK)
                                                {
                                                    if (officerResponse.Data != null)
                                                    {
                                                        //Clues
                                                        officer.appointmentResponse = officerResponse.Data;
                                                    }
                                                }
                                            }

                                            {
                                                var officerRequest = new RestRequest(string.Format("disqualified-officers/natural/{0}", officer.identification.registration_number), Method.GET);
                                                officerRequest.AddHeader("Authorization", "Basic " + Base64Encode("_Y-9-pOnf-c0o4_bIZpjGASw8FYrNP-nVK2DvEbn"));
                                                var officerResponse = client.ExecuteTaskAsync<OfficerResponse>(officerRequest).Result;
                                                if (officerResponse.StatusCode == HttpStatusCode.OK)
                                                {
                                                    if (officerResponse.Data != null)
                                                    {
                                                        //Clues
                                                        officer.disqualifiedNaturalResponse = officerResponse.Data;
                                                    }
                                                }
                                            }

                                            {
                                                var officerRequest = new RestRequest(string.Format("disqualified-officers/corporate/{0}", officer.identification.registration_number), Method.GET);
                                                officerRequest.AddHeader("Authorization", "Basic " + Base64Encode("_Y-9-pOnf-c0o4_bIZpjGASw8FYrNP-nVK2DvEbn"));
                                                var officerResponse = client.ExecuteTaskAsync<OfficerResponse>(officerRequest).Result;
                                                if (officerResponse.StatusCode == HttpStatusCode.OK)
                                                {
                                                    if (officerResponse.Data != null)
                                                    {
                                                        //Clues
                                                        officer.disqualifiedCorporateResponse = officerResponse.Data;
                                                    }
                                                }
                                            }

                                            officers.Add(officer);
                                        }

                                    companyAndContact.contact = officers;
                                }
                            }

                            {
                                var listRequest = new RestRequest(string.Format("/company/{0}/registered-office-address", searchResult.company_number), Method.GET);
                                listRequest.AddHeader("Authorization", "Basic " + Base64Encode("_Y-9-pOnf-c0o4_bIZpjGASw8FYrNP-nVK2DvEbn"));
                                var listResponse = client.ExecuteTaskAsync<CompanyAddress>(listRequest).Result;
                                if (listResponse.StatusCode == HttpStatusCode.OK)
                                {
                                    if (listResponse.Data != null)
                                    {
                                        // result.mainCompanyAddress = listResponse.Data;
                                    }
                                }
                            }

                            {
                                var listRequest = new RestRequest(string.Format("/company/{0}/filing-history", searchResult.company_number), Method.GET);
                                listRequest.AddHeader("Authorization", "Basic " + Base64Encode("_Y-9-pOnf-c0o4_bIZpjGASw8FYrNP-nVK2DvEbn"));
                                listRequest.AddParameter("items_per_page", 100);
                                listRequest.AddParameter("start_index", 0);
                                var listResponse = client.ExecuteTaskAsync<FilingHistory>(listRequest).Result;

                                if (listResponse.StatusCode == HttpStatusCode.OK)
                                {
                                    if (listResponse.Data != null)
                                    {
                                        //result.filingHistory = listResponse.Data;
                                    }
                                }
                            }

                            {
                                var listRequest = new RestRequest(string.Format("/company/{0}/insolvency", searchResult.company_number), Method.GET);
                                listRequest.AddHeader("Authorization", "Basic " + Base64Encode("_Y-9-pOnf-c0o4_bIZpjGASw8FYrNP-nVK2DvEbn"));
                                var listResponse = client.ExecuteTaskAsync<InsolvencyResponse>(listRequest).Result;
                                if (listResponse.StatusCode == HttpStatusCode.OK)
                                {
                                    if (listResponse.Data != null)
                                    {
                                        //result.insolvencyResponse = listResponse.Data;
                                    }
                                }
                            }

                            {
                                var listRequest = new RestRequest(string.Format("/company/{0}/charges", searchResult.company_number), Method.GET);
                                listRequest.AddHeader("Authorization", "Basic " + Base64Encode("_Y-9-pOnf-c0o4_bIZpjGASw8FYrNP-nVK2DvEbn"));
                                listRequest.AddParameter("items_per_page", 100);
                                listRequest.AddParameter("start_index", 0);
                                var listResponse = client.ExecuteTaskAsync<ChargeResponse>(listRequest).Result;
                                if (listResponse.StatusCode == HttpStatusCode.OK)
                                {
                                    if (listResponse.Data != null)
                                    {
                                        //result.chargeResponse = listResponse.Data;
                                    }
                                }
                            }

                            {
                                var listRequest = new RestRequest(string.Format("/company/{0}/uk-establishments", searchResult.company_number), Method.GET);
                                listRequest.AddHeader("Authorization", "Basic " + Base64Encode("_Y-9-pOnf-c0o4_bIZpjGASw8FYrNP-nVK2DvEbn"));
                                var listResponse = client.ExecuteTaskAsync<UkEstablishmentResponse>(listRequest).Result;
                                if (listResponse.StatusCode == HttpStatusCode.OK)
                                {
                                    if (listResponse.Data != null)
                                    {
                                        //result.ukEstablishmentResponse = listResponse.Data;
                                    }
                                }
                            }


                            {
                                var listRequest = new RestRequest(string.Format("/company/{0}/persons-with-significant-control", searchResult.company_number), Method.GET);
                                listRequest.AddHeader("Authorization", "Basic " + Base64Encode("_Y-9-pOnf-c0o4_bIZpjGASw8FYrNP-nVK2DvEbn"));
                                listRequest.AddParameter("items_per_page", 100);
                                listRequest.AddParameter("start_index", 0);
                                var listResponse = client.ExecuteTaskAsync<ControlPersonResponse>(listRequest).Result;
                                if (listResponse.StatusCode == HttpStatusCode.OK)
                                {
                                    if (listResponse.Data != null)
                                    {
                                        // result.controlPersonResponse = listResponse.Data;
                                    }
                                }
                            }

                            {
                                var listRequest = new RestRequest(string.Format("/company/{0}/persons-with-significant-control-statements", searchResult.company_number), Method.GET);
                                listRequest.AddHeader("Authorization", "Basic " + Base64Encode("_Y-9-pOnf-c0o4_bIZpjGASw8FYrNP-nVK2DvEbn"));
                                listRequest.AddParameter("items_per_page", 100);
                                listRequest.AddParameter("start_index", 0);
                                var listResponse = client.ExecuteTaskAsync<PersonControlStatements>(listRequest).Result;
                                if (listResponse.StatusCode == HttpStatusCode.OK)
                                {
                                    if (listResponse.Data != null)
                                    {
                                        // result.personControlStatements = listResponse.Data;
                                    }
                                }
                            }

                            {
                                var listRequest = new RestRequest(string.Format("/company/{0}/registers", searchResult.company_number), Method.GET);
                                listRequest.AddHeader("Authorization", "Basic " + Base64Encode("_Y-9-pOnf-c0o4_bIZpjGASw8FYrNP-nVK2DvEbn"));
                                var listResponse = client.ExecuteTaskAsync<CompanyRegisterResponse>(listRequest).Result;
                                if (listResponse.StatusCode == HttpStatusCode.OK)
                                {
                                    if (listResponse.Data != null)
                                    {
                                        // result.companyRegisterResponse = listResponse.Data;
                                    }
                                }
                            }

                            {
                                var listRequest = new RestRequest(string.Format("/company/{0}/exemptions", searchResult.company_number), Method.GET);
                                listRequest.AddHeader("Authorization", "Basic " + Base64Encode("_Y-9-pOnf-c0o4_bIZpjGASw8FYrNP-nVK2DvEbn"));
                                var listResponse = client.ExecuteTaskAsync<ExceptionResponse>(listRequest).Result;
                                if (listResponse.StatusCode == HttpStatusCode.OK)
                                {
                                    if (listResponse.Data != null)
                                    {
                                        //result.exceptionResponse = listResponse.Data;
                                    }
                                }
                            }

                        }
                }
                else
                {
                    yield break;
                }
            }
            else
            {
                var client = new RestClient("https://api.companieshouse.gov.uk");
                var request = new RestRequest(string.Format("company/{0}", id), Method.GET);
                request.AddHeader("Authorization", "Basic " + Base64Encode("_Y-9-pOnf-c0o4_bIZpjGASw8FYrNP-nVK2DvEbn"));
                var response = client.ExecuteTaskAsync<CompanyResult>(request).Result;

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    foreach (var result in response.Data.items)
                    {
                        var companyAndContact = new CompanyAndContacts();
                        {
                            var listRequest = new RestRequest(string.Format("/company/{0}/officers", result.company_number), Method.GET);
                            listRequest.AddHeader("Authorization", "Basic " + Base64Encode("_Y-9-pOnf-c0o4_bIZpjGASw8FYrNP-nVK2DvEbn"));
                            listRequest.AddParameter("items_per_page", 100);
                            listRequest.AddParameter("start_index", 0);
                            var listResponse = client.ExecuteTaskAsync<ContactList>(listRequest).Result;
                            if (listResponse.StatusCode == HttpStatusCode.OK)
                            {
                                if (listResponse.Data != null)
                                {
                                    var officers = new List<Contact>();
                                    if (listResponse.Data.items != null)
                                        foreach (var officer in listResponse.Data.items)
                                        {
                                            {
                                                var officerRequest = new RestRequest(string.Format("officers/{0}/appointments", officer.identification.registration_number), Method.GET);
                                                officerRequest.AddHeader("Authorization", "Basic " + Base64Encode("_Y-9-pOnf-c0o4_bIZpjGASw8FYrNP-nVK2DvEbn"));
                                                officerRequest.AddParameter("items_per_page", 100);
                                                officerRequest.AddParameter("start_index", 0);
                                                var officerResponse = client.ExecuteTaskAsync<AppointmentResponse>(officerRequest).Result;
                                                if (officerResponse.StatusCode == HttpStatusCode.OK)
                                                {
                                                    if (officerResponse.Data != null)
                                                    {
                                                        //Clues
                                                        officer.appointmentResponse = officerResponse.Data;
                                                    }
                                                }
                                            }

                                            {
                                                var officerRequest = new RestRequest(string.Format("disqualified-officers/natural/{0}", officer.identification.registration_number), Method.GET);
                                                officerRequest.AddHeader("Authorization", "Basic " + Base64Encode("_Y-9-pOnf-c0o4_bIZpjGASw8FYrNP-nVK2DvEbn"));
                                                var officerResponse = client.ExecuteTaskAsync<OfficerResponse>(officerRequest).Result;
                                                if (officerResponse.StatusCode == HttpStatusCode.OK)
                                                {
                                                    if (officerResponse.Data != null)
                                                    {
                                                        //Clues
                                                        officer.disqualifiedNaturalResponse = officerResponse.Data;
                                                    }
                                                }
                                            }

                                            {
                                                var officerRequest = new RestRequest(string.Format("disqualified-officers/corporate/{0}", officer.identification.registration_number), Method.GET);
                                                officerRequest.AddHeader("Authorization", "Basic " + Base64Encode("_Y-9-pOnf-c0o4_bIZpjGASw8FYrNP-nVK2DvEbn"));
                                                var officerResponse = client.ExecuteTaskAsync<OfficerResponse>(officerRequest).Result;
                                                if (officerResponse.StatusCode == HttpStatusCode.OK)
                                                {
                                                    if (officerResponse.Data != null)
                                                    {
                                                        //Clues
                                                        officer.disqualifiedCorporateResponse = officerResponse.Data;
                                                    }
                                                }
                                            }

                                            officers.Add(officer);
                                        }

                                    companyAndContact.contact = officers;
                                }
                            }
                        }

                        {
                            var listRequest = new RestRequest(string.Format("/company/{0}/registered-office-address", result.company_number), Method.GET);
                            listRequest.AddHeader("Authorization", "Basic " + Base64Encode("_Y-9-pOnf-c0o4_bIZpjGASw8FYrNP-nVK2DvEbn"));
                            var listResponse = client.ExecuteTaskAsync<CompanyAddress>(listRequest).Result;
                            if (listResponse.StatusCode == HttpStatusCode.OK)
                            {
                                if (listResponse.Data != null)
                                {
                                   // result.mainCompanyAddress = listResponse.Data;
                                }
                            }
                        }

                        {
                            var listRequest = new RestRequest(string.Format("/company/{0}/filing-history", result.company_number), Method.GET);
                            listRequest.AddHeader("Authorization", "Basic " + Base64Encode("_Y-9-pOnf-c0o4_bIZpjGASw8FYrNP-nVK2DvEbn"));
                            listRequest.AddParameter("items_per_page", 100);
                            listRequest.AddParameter("start_index", 0);
                            var listResponse = client.ExecuteTaskAsync<FilingHistory>(listRequest).Result;

                            if (listResponse.StatusCode == HttpStatusCode.OK)
                            {
                                if (listResponse.Data != null)
                                {
                                    //result.filingHistory = listResponse.Data;
                                }
                            }
                        }

                        {
                            var listRequest = new RestRequest(string.Format("/company/{0}/insolvency", result.company_number), Method.GET);
                            listRequest.AddHeader("Authorization", "Basic " + Base64Encode("_Y-9-pOnf-c0o4_bIZpjGASw8FYrNP-nVK2DvEbn"));
                            var listResponse = client.ExecuteTaskAsync<InsolvencyResponse>(listRequest).Result;
                            if (listResponse.StatusCode == HttpStatusCode.OK)
                            {
                                if (listResponse.Data != null)
                                {
                                    //result.insolvencyResponse = listResponse.Data;
                                }
                            }
                        }

                        {
                            var listRequest = new RestRequest(string.Format("/company/{0}/charges", result.company_number), Method.GET);
                            listRequest.AddHeader("Authorization", "Basic " + Base64Encode("_Y-9-pOnf-c0o4_bIZpjGASw8FYrNP-nVK2DvEbn"));
                            listRequest.AddParameter("items_per_page", 100);
                            listRequest.AddParameter("start_index", 0);
                            var listResponse = client.ExecuteTaskAsync<ChargeResponse>(listRequest).Result;
                            if (listResponse.StatusCode == HttpStatusCode.OK)
                            {
                                if (listResponse.Data != null)
                                {
                                    //result.chargeResponse = listResponse.Data;
                                }
                            }
                        }

                        {
                            var listRequest = new RestRequest(string.Format("/company/{0}/uk-establishments", result.company_number), Method.GET);
                            listRequest.AddHeader("Authorization", "Basic " + Base64Encode("_Y-9-pOnf-c0o4_bIZpjGASw8FYrNP-nVK2DvEbn"));
                            var listResponse = client.ExecuteTaskAsync<UkEstablishmentResponse>(listRequest).Result;
                            if (listResponse.StatusCode == HttpStatusCode.OK)
                            {
                                if (listResponse.Data != null)
                                {
                                    //result.ukEstablishmentResponse = listResponse.Data;
                                }
                            }
                        }


                        {
                            var listRequest = new RestRequest(string.Format("/company/{0}/persons-with-significant-control", result.company_number), Method.GET);
                            listRequest.AddHeader("Authorization", "Basic " + Base64Encode("_Y-9-pOnf-c0o4_bIZpjGASw8FYrNP-nVK2DvEbn"));
                            listRequest.AddParameter("items_per_page", 100);
                            listRequest.AddParameter("start_index", 0);
                            var listResponse = client.ExecuteTaskAsync<ControlPersonResponse>(listRequest).Result;
                            if (listResponse.StatusCode == HttpStatusCode.OK)
                            {
                                if (listResponse.Data != null)
                                {
                                   // result.controlPersonResponse = listResponse.Data;
                                }
                            }
                        }

                        {
                            var listRequest = new RestRequest(string.Format("/company/{0}/persons-with-significant-control-statements", result.company_number), Method.GET);
                            listRequest.AddHeader("Authorization", "Basic " + Base64Encode("_Y-9-pOnf-c0o4_bIZpjGASw8FYrNP-nVK2DvEbn"));
                            listRequest.AddParameter("items_per_page", 100);
                            listRequest.AddParameter("start_index", 0);
                            var listResponse = client.ExecuteTaskAsync<PersonControlStatements>(listRequest).Result;
                            if (listResponse.StatusCode == HttpStatusCode.OK)
                            {
                                if (listResponse.Data != null)
                                {
                                   // result.personControlStatements = listResponse.Data;
                                }
                            }
                        }

                        {
                            var listRequest = new RestRequest(string.Format("/company/{0}/registers", result.company_number), Method.GET);
                            listRequest.AddHeader("Authorization", "Basic " + Base64Encode("_Y-9-pOnf-c0o4_bIZpjGASw8FYrNP-nVK2DvEbn"));
                            var listResponse = client.ExecuteTaskAsync<CompanyRegisterResponse>(listRequest).Result;
                            if (listResponse.StatusCode == HttpStatusCode.OK)
                            {
                                if (listResponse.Data != null)
                                {
                                   // result.companyRegisterResponse = listResponse.Data;
                                }
                            }
                        }

                        {
                            var listRequest = new RestRequest(string.Format("/company/{0}/exemptions", result.company_number), Method.GET);
                            listRequest.AddHeader("Authorization", "Basic " + Base64Encode("_Y-9-pOnf-c0o4_bIZpjGASw8FYrNP-nVK2DvEbn"));
                            var listResponse = client.ExecuteTaskAsync<ExceptionResponse>(listRequest).Result;
                            if (listResponse.StatusCode == HttpStatusCode.OK)
                            {
                                if (listResponse.Data != null)
                                {
                                    //result.exceptionResponse = listResponse.Data;
                                }
                            }
                        }

                        companyAndContact.Company = result;

                        yield return new ExternalSearchQueryResult<CompanyAndContacts>(query, companyAndContact);

                    }
                }
                else if (response.StatusCode == HttpStatusCode.NoContent || response.StatusCode == HttpStatusCode.NotFound)
                    yield break;
                else if (response.ErrorException != null)
                    throw new AggregateException(response.ErrorException.Message, response.ErrorException);
                else
                    throw new ApplicationException("Could not execute external search query - StatusCode:" + response.StatusCode);
            }
        }

        /// <summary>Builds the clues.</summary>
        /// <param name="context">The context.</param>
        /// <param name="query">The query.</param>
        /// <param name="result">The result.</param>
        /// <param name="request">The request.</param>
        /// <returns>The clues.</returns>
        public override IEnumerable<Clue> BuildClues(ExecutionContext context, IExternalSearchQuery query, IExternalSearchQueryResult result, IExternalSearchRequest request)
        {
            var listOfClues = new List<Clue>();
            var resultItem = result.As<CompanyAndContacts>();

            var code = new EntityCode(EntityType.Organization, this.GetCodeOrigin(), resultItem.Data.Company.company_number);

            var clue = new Clue(code, context.Organization);
            clue.Data.OriginProviderDefinitionId = this.Id;

            this.PopulateMetadata(clue.Data.EntityData, resultItem.Data.Company);
            listOfClues.Add(clue);

            if (resultItem.Data.contact != null)
                foreach (var contact in resultItem.Data.contact)
                {
                    if (contact.identification != null)
                    {
                        if (contact.identification.registration_number != null)
                        {
                            var codeContact = new EntityCode(EntityType.Person, this.GetCodeOrigin(), contact.identification.registration_number);
                            var contactClue = new Clue(codeContact, context.Organization);

                            contactClue.Data.OriginProviderDefinitionId = this.Id;

                            this.PopulateContactMetadata(clue.Data.EntityData, contact);

                            listOfClues.Add(contactClue);
                        }
                    }
                }

            return listOfClues;
        }

        /// <summary>Gets the primary entity metadata.</summary>
        /// <param name="context">The context.</param>
        /// <param name="result">The result.</param>
        /// <param name="request">The request.</param>
        /// <returns>The primary entity metadata.</returns>
        public override IEntityMetadata GetPrimaryEntityMetadata(ExecutionContext context, IExternalSearchQueryResult result, IExternalSearchRequest request)
        {
            var resultItem = result.As<CompanyAndContacts>();
            return this.CreateMetadata(resultItem);
        }

        /// <summary>Gets the preview image.</summary>
        /// <param name="context">The context.</param>
        /// <param name="result">The result.</param>
        /// <param name="request">The request.</param>
        /// <returns>The preview image.</returns>
        public override IPreviewImage GetPrimaryEntityPreviewImage(ExecutionContext context, IExternalSearchQueryResult result, IExternalSearchRequest request)
        {
            return null;
        }

        /// <summary>Creates the metadata.</summary>
        /// <param name="resultItem">The result item.</param>
        /// <returns>The metadata.</returns>
        private IEntityMetadata CreateMetadata(IExternalSearchQueryResult<CompanyAndContacts> resultItem)
        {
            var metadata = new EntityMetadataPart();

            this.PopulateMetadata(metadata, resultItem.Data.Company);

            return metadata;
        }

        /// <summary>Gets the code origin.</summary>
        /// <returns>The code origin</returns>
        private CodeOrigin GetCodeOrigin()
        {
            return CodeOrigin.CluedIn.CreateSpecific("companyHouse");
        }

        /// <summary>Populates the metadata.</summary>
        /// <param name="metadata">The metadata.</param>
        /// <param name="resultCompany">The result item.</param>
        private void PopulateMetadata(IEntityMetadata metadata, CompanyNew resultCompany)
        {
            var code = new EntityCode(EntityType.Organization, this.GetCodeOrigin(), resultCompany.company_number);

            metadata.EntityType = EntityType.Organization;

            //if (resultCompany.title != null) metadata.Name = resultCompany.title;
            //if (resultCompany.description != null) metadata.Description = resultCompany.description;

            metadata.OriginEntityCode = code;

            if (resultCompany.company_number != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = resultCompany.company_number;

            //if (resultCompany.address != null)
            //{
            //    if (resultCompany.address.address_line_1 != null) metadata.Properties[CompanyHouseVocabulary.Organization.AddressLine1] = resultCompany.address.address_line_1;
            //    if (resultCompany.address.address_line_2 != null) metadata.Properties[CompanyHouseVocabulary.Organization.AddressLine2] = resultCompany.address.address_line_2;
            //    if (resultCompany.address.country != null) metadata.Properties[CompanyHouseVocabulary.Organization.Country] = resultCompany.address.country;
            //    if (resultCompany.address.etag != null) metadata.Properties[CompanyHouseVocabulary.Organization.Etag] = resultCompany.address.etag;
            //    if (resultCompany.address.locality != null) metadata.Properties[CompanyHouseVocabulary.Organization.Locality] = resultCompany.address.locality;
            //    if (resultCompany.address.postal_code != null) metadata.Properties[CompanyHouseVocabulary.Organization.PostalCode] = resultCompany.address.postal_code;
            //    if (resultCompany.address.po_box != null) metadata.Properties[CompanyHouseVocabulary.Organization.PoBox] = resultCompany.address.po_box;
            //    if (resultCompany.address.premises != null) metadata.Properties[CompanyHouseVocabulary.Organization.Premises] = resultCompany.address.premises;
            //    if (resultCompany.address.region != null) metadata.Properties[CompanyHouseVocabulary.Organization.Region] = resultCompany.address.region;
            //    if (resultCompany.address_snippet != null) metadata.Properties[CompanyHouseVocabulary.Organization.AddressSnippet] = resultCompany.address_snippet;
            //}

            //if (resultCompany.chargeResponse != null)
            //    if (resultCompany.chargeResponse.items != null)
            //        foreach (var charge in resultCompany.chargeResponse.items)
            //        {
            //            if (charge.acquired_on != null) metadata.Properties[CompanyHouseVocabulary.Organization.AcquiredOn] = charge.acquired_on;
            //        }


            //if (resultCompany.companyRegisterResponse != null)
            //{
            //    if (resultCompany.companyRegisterResponse.registers != null)
            //    {
            //        if (resultCompany.companyRegisterResponse.registers.directors != null)
            //            if (resultCompany.companyRegisterResponse.registers.directors.items != null)
            //                foreach (var director in resultCompany.companyRegisterResponse.registers.directors.items)
            //                {
            //                    if (director.linked_psc_name != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = director.linked_psc_name;
            //                    if (director.ceased_on != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = director.ceased_on;
            //                    if (director.notified_on != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = director.notified_on;
            //                    if (director.links.person_with_significant_control != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = director.links.person_with_significant_control;
            //                    if (director.links.self != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = director.links.self;
            //                    if (director.restrictions_notice_withdrawal_reason != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = director.restrictions_notice_withdrawal_reason;
            //                    if (director.statement != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = director.statement;
            //                }

            //        if (resultCompany.companyRegisterResponse.registers.llp_members != null)
            //            if (resultCompany.companyRegisterResponse.registers.llp_members.items != null)
            //                foreach (var director in resultCompany.companyRegisterResponse.registers.llp_members.items)
            //                {
            //                    if (director.moved_on != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = director.moved_on;
            //                    if (director.register_moved_to != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = director.register_moved_to;

            //                    if (director.links != null)
            //                        if (director.links.filing != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = director.links.filing;
            //                }

            //        if (resultCompany.companyRegisterResponse.registers.llp_usual_residential_address != null)
            //            if (resultCompany.companyRegisterResponse.registers.llp_usual_residential_address.items != null)
            //                foreach (var director in resultCompany.companyRegisterResponse.registers.llp_usual_residential_address.items)
            //                {
            //                    if (director.moved_on != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = director.moved_on;
            //                    if (director.register_moved_to != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = director.register_moved_to;

            //                    if (director.links != null)
            //                        if (director.links.filing != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = director.links.filing;
            //                }

            //        if (resultCompany.companyRegisterResponse.registers.members != null)
            //            if (resultCompany.companyRegisterResponse.registers.members.items != null)
            //                foreach (var director in resultCompany.companyRegisterResponse.registers.members.items)
            //                {
            //                    if (director.moved_on != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = director.moved_on;
            //                    if (director.register_moved_to != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = director.register_moved_to;

            //                    if (director.links != null)
            //                        if (director.links.filing != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = director.links.filing;
            //                }

            //        if (resultCompany.companyRegisterResponse.registers.persons_with_significant_control != null)
            //            if (resultCompany.companyRegisterResponse.registers.persons_with_significant_control.items != null)
            //                foreach (var director in resultCompany.companyRegisterResponse.registers.persons_with_significant_control.items)
            //                {
            //                    if (director.moved_on != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = director.moved_on;
            //                    if (director.register_moved_to != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = director.register_moved_to;

            //                    if (director.links != null)
            //                        if (director.links.filing != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = director.links.filing;
            //                }

            //        if (resultCompany.companyRegisterResponse.registers.secretaries != null)
            //            if (resultCompany.companyRegisterResponse.registers.secretaries.items != null)
            //                foreach (var director in resultCompany.companyRegisterResponse.registers.secretaries.items)
            //                {
            //                    if (director.moved_on != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = director.moved_on;
            //                    if (director.register_moved_to != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = director.register_moved_to;
            //                    if (director.links != null)
            //                        if (director.links.filing != null)
            //                            metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = director.links.filing;
            //                }

            //        if (resultCompany.companyRegisterResponse.registers.usual_residential_address != null)
            //            if (resultCompany.companyRegisterResponse.registers.usual_residential_address.items != null)
            //                foreach (var director in resultCompany.companyRegisterResponse.registers.usual_residential_address.items)
            //                {
            //                    if (director.moved_on != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = director.moved_on;
            //                    if (director.register_moved_to != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = director.register_moved_to;

            //                    if (director.links != null)
            //                        if (director.links.filing != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = director.links.filing;
            //                }
            //    }

            //    if (resultCompany.companyRegisterResponse.company_number != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = resultCompany.companyRegisterResponse.company_number;

            //}

            if (resultCompany.company_number != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = resultCompany.company_number;
            if (resultCompany.company_status != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyStatus] = resultCompany.company_status;
            //if (resultCompany.company_type != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyType] = resultCompany.company_type;

            //if (resultCompany.controlPersonResponse != null)
            //{
            //    if (resultCompany.controlPersonResponse.active_count != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = resultCompany.controlPersonResponse.active_count;
            //    if (resultCompany.controlPersonResponse.ceased_count != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = resultCompany.controlPersonResponse.ceased_count;
            //}

            //if (resultCompany.controlPersonResponse.items != null)
            //    foreach (var person in resultCompany.controlPersonResponse.items)
            //    {
            //        this.PopulateControlPersonItemMetadata(metadata, person);
            //    }

            if (resultCompany.date_of_creation != null)
            {
                DateTimeOffset createdDate;

                if (DateTimeOffset.TryParse(resultCompany.date_of_creation, out createdDate))
                {
                    metadata.CreatedDate = createdDate;
                }

                metadata.Properties[CompanyHouseVocabulary.Organization.DateOfCreation] = resultCompany.date_of_creation;
            }

            //if (resultCompany.description_identifier != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = string.Format(",", resultCompany.description_identifier);

            //if (resultCompany.exceptionResponse.exemptions.disclosure_transparency_rules_chapter_five_applies.items != null)
            //    foreach (var identifier in resultCompany.exceptionResponse.exemptions.disclosure_transparency_rules_chapter_five_applies.items)
            //    {
            //        if (identifier.exempt_from != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = identifier.exempt_from;
            //        if (identifier.exempt_to != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = identifier.exempt_to;
            //    }

            //if (resultCompany.filingHistory != null)
            //    if (resultCompany.filingHistory.filing_history_status != null)
            //        metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = resultCompany.filingHistory.filing_history_status;

            //if (resultCompany.filingHistory != null)
            //    if (resultCompany.filingHistory.items != null)
            //        foreach (var identifier in resultCompany.filingHistory.items)
            //        {
            //            if (identifier.ceased_on != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = identifier.ceased_on;
            //            if (identifier.linked_psc_name != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = identifier.linked_psc_name;
            //            if (identifier.links.person_with_significant_control != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = identifier.links.person_with_significant_control;
            //            if (identifier.links.self != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = identifier.links.self;

            //            if (identifier.notified_on != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = identifier.notified_on;
            //            if (identifier.restrictions_notice_withdrawal_reason != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = identifier.restrictions_notice_withdrawal_reason;
            //            if (identifier.statement != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = identifier.statement;
            //        }

            //if (resultCompany.insolvencyResponse != null)
            //    if (resultCompany.insolvencyResponse.cases != null)
            //        foreach (var identifier in resultCompany.insolvencyResponse.cases)
            //        {
            //            //if (identifier.dates != null)
            //            //    foreach (var date in identifier.dates)
            //            //    {
            //            //        if (resultCompany.company_number != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = date.date;
            //            //        if (resultCompany.company_number != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = date.type;
            //            //    }

            //            if (identifier.notes != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = string.Format(",", identifier.notes);

            //            if (identifier.practitioners != null)
            //                foreach (var practitioner in identifier.practitioners)
            //                {
            //                    this.PopulatePractionerMetadata(metadata, practitioner);
            //                }

            //            if (identifier.number != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = identifier.number;
            //            if (identifier.type != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = identifier.type;
            //        }

            //if (resultCompany.kind != null) metadata.Properties[CompanyHouseVocabulary.Organization.Kind] = resultCompany.kind;
            if (resultCompany.links.self != null) metadata.Properties[CompanyHouseVocabulary.Organization.LinksSelf] = resultCompany.links.self;

            //if (resultCompany.mainCompanyAddress != null)
            //{
            //    if (resultCompany.mainCompanyAddress.address_line_1 != null) metadata.Properties[CompanyHouseVocabulary.Organization.MainCompanyAddress1] = resultCompany.mainCompanyAddress.address_line_1;
            //    if (resultCompany.mainCompanyAddress.address_line_2 != null) metadata.Properties[CompanyHouseVocabulary.Organization.MainCompanyAddress2] = resultCompany.mainCompanyAddress.address_line_2;
            //    if (resultCompany.mainCompanyAddress.country != null) metadata.Properties[CompanyHouseVocabulary.Organization.MainCompanyCountry] = resultCompany.mainCompanyAddress.country;
            //    if (resultCompany.mainCompanyAddress.locality != null) metadata.Properties[CompanyHouseVocabulary.Organization.MainCompanyLocality] = resultCompany.mainCompanyAddress.locality;
            //    if (resultCompany.mainCompanyAddress.postal_code != null) metadata.Properties[CompanyHouseVocabulary.Organization.MainCompanyPostalCode] = resultCompany.mainCompanyAddress.postal_code;
            //    if (resultCompany.mainCompanyAddress.po_box != null) metadata.Properties[CompanyHouseVocabulary.Organization.MainCompanyPoBox] = resultCompany.mainCompanyAddress.po_box;
            //    if (resultCompany.mainCompanyAddress.premises != null) metadata.Properties[CompanyHouseVocabulary.Organization.MainCompanyPremises] = resultCompany.mainCompanyAddress.premises;
            //    if (resultCompany.mainCompanyAddress.region != null) metadata.Properties[CompanyHouseVocabulary.Organization.MainCompanyRegion] = resultCompany.mainCompanyAddress.region;
            //}

            //if (resultCompany.matches != null)
            //{
            //    foreach (var addressSnippet in resultCompany.matches.address_snippet)
            //    {
            //        if (resultCompany.company_number != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = JsonUtility.Serialize(resultCompany.matches.snippet);
            //        if (resultCompany.company_number != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = JsonUtility.Serialize(resultCompany.matches.title);
            //    }

            //    foreach (var snippet in resultCompany.matches.snippet)
            //    {
            //        if (resultCompany.company_number != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = JsonUtility.Serialize(resultCompany.matches.snippet);
            //        if (resultCompany.company_number != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = JsonUtility.Serialize(resultCompany.matches.title);
            //    }

            //    foreach (var title in resultCompany.matches.title)
            //    {
            //        if (resultCompany.company_number != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = JsonUtility.Serialize(resultCompany.matches.snippet);
            //        if (resultCompany.company_number != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = JsonUtility.Serialize(resultCompany.matches.title);
            //    }
            //}

            //if (resultCompany.personControlStatements.items != null)
            //    foreach (var identifier in resultCompany.personControlStatements.items)
            //    {
            //        if (identifier.ceased_on != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = identifier.ceased_on;
            //        if (identifier.linked_psc_name != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = identifier.linked_psc_name;
            //        if (identifier.links.person_with_significant_control != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = identifier.links.person_with_significant_control;
            //        if (identifier.links.self != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = identifier.links.self;
            //        if (identifier.notified_on != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = identifier.notified_on;
            //        if (identifier.restrictions_notice_withdrawal_reason != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = identifier.restrictions_notice_withdrawal_reason;
            //        if (identifier.statement != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = identifier.statement;
            //    }

            //if (resultCompany.snippet != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = resultCompany.snippet;
            //if (resultCompany.title != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = resultCompany.title;

            //if (resultCompany.ukEstablishmentResponse.items != null)
            //    foreach (var identifier in resultCompany.ukEstablishmentResponse.items)
            //    {
            //        if (identifier.company_name != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = identifier.company_name;
            //        if (identifier.company_number != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = identifier.company_number;
            //        if (identifier.company_status != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = identifier.company_status;
            //        if (identifier.links != null) if (identifier.links.company != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = identifier.links.company;
            //        if (identifier.locality != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = identifier.locality;
            //    }

        }


        private void PopulatePractionerMetadata(IEntityMetadata metadata, Practitioner resultItem)
        {
            var code = new EntityCode(EntityType.Person, this.GetCodeOrigin(), resultItem.name);

            metadata.EntityType = EntityType.Person;
            if (resultItem.name != null) metadata.Name = resultItem.name;
            metadata.OriginEntityCode = code;

            if (resultItem.address != null)
            {
                if (resultItem.address.address_line_1 != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = resultItem.address.address_line_1;
                if (resultItem.address.address_line_2 != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = resultItem.address.address_line_2;
                if (resultItem.address.country != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = resultItem.address.country;
                if (resultItem.address.locality != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = resultItem.address.locality;
                if (resultItem.address.postal_code != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = resultItem.address.postal_code;
                if (resultItem.address.region != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = resultItem.address.region;
            }

            if (resultItem.appointed_on != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = resultItem.appointed_on;
            if (resultItem.ceased_to_act_on != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = resultItem.ceased_to_act_on;
            if (resultItem.role != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = resultItem.role;
        }

        private void PopulateControlPersonItemMetadata(IEntityMetadata metadata, ControlPersonItem person)
        {
            var code = new EntityCode(EntityType.Person, this.GetCodeOrigin(), person.name);

            metadata.EntityType = EntityType.Person;
            if (person.name != null) metadata.Name = person.name;
            metadata.OriginEntityCode = code;

            if (person.ceased_on != null) metadata.Properties[CompanyHouseVocabulary.Person.CeasedOn] = person.ceased_on;
            if (person.country_of_residence != null) metadata.Properties[CompanyHouseVocabulary.Person.CountryOfResidence] = person.country_of_residence;
            if (person.date_of_birth.day != null) metadata.Properties[CompanyHouseVocabulary.Person.DateOfBirthDay] = person.date_of_birth.day;
            if (person.date_of_birth.month != null) metadata.Properties[CompanyHouseVocabulary.Person.DateOfBirthMonth] = person.date_of_birth.month;
            if (person.date_of_birth.year != null) metadata.Properties[CompanyHouseVocabulary.Person.DateOfBirthYear] = person.date_of_birth.year;
            if (person.name_elements.forename != null) metadata.Properties[CompanyHouseVocabulary.Person.Forename] = person.name_elements.forename;
            if (person.name_elements.other_forenames != null) metadata.Properties[CompanyHouseVocabulary.Person.OtherForeNames] = person.name_elements.other_forenames;
            if (person.name_elements.surname != null) metadata.Properties[CompanyHouseVocabulary.Person.Surname] = person.name_elements.surname;
            if (person.name_elements.title != null) metadata.Properties[CompanyHouseVocabulary.Person.Title] = person.name_elements.title;
            if (person.nationality != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = person.nationality;

            if (person.natures_of_control != null)
                foreach (var nature in person.natures_of_control)
                {
                    if (nature != null) metadata.Properties[CompanyHouseVocabulary.Organization.CompanyNumber] = nature;
                }

            if (person.notified_on != null) metadata.Properties[CompanyHouseVocabulary.Person.NotifiedOn] = person.notified_on;
            if (person.address.address_line_1 != null) metadata.Properties[CompanyHouseVocabulary.Person.AddressLine1] = person.address.address_line_1;
            if (person.address.address_line_2 != null) metadata.Properties[CompanyHouseVocabulary.Person.AddressLine2] = person.address.address_line_2;
            if (person.address.country != null) metadata.Properties[CompanyHouseVocabulary.Person.Country] = person.address.country;
            if (person.address.locality != null) metadata.Properties[CompanyHouseVocabulary.Person.Locality] = person.address.locality;
            if (person.address.postal_code != null) metadata.Properties[CompanyHouseVocabulary.Person.PostalCode] = person.address.postal_code;
            if (person.address.premises != null) metadata.Properties[CompanyHouseVocabulary.Person.Premises] = person.address.premises;
            if (person.address.region != null) metadata.Properties[CompanyHouseVocabulary.Person.Region] = person.address.region;
        }

        private void PopulateContactMetadata(IEntityMetadata metadata, Contact resultItem)
        {
            if (resultItem.identification != null)
            {
                if (resultItem.identification.registration_number != null)
                {
                    var code = new EntityCode(EntityType.Person, this.GetCodeOrigin(), resultItem.identification.registration_number);

                    metadata.EntityType = EntityType.Person;
                    if (resultItem.name != null) metadata.Name = resultItem.name;
                    metadata.OriginEntityCode = code;

                    if (resultItem.address != null)
                    {
                        if (resultItem.address.address_line_1 != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = resultItem.address.address_line_1;
                        if (resultItem.address.address_line_2 != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = resultItem.address.address_line_2;
                        if (resultItem.address.country != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = resultItem.address.country;
                        if (resultItem.address.locality != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = resultItem.address.locality;
                        if (resultItem.address.postal_code != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = resultItem.address.postal_code;
                        if (resultItem.address.premises != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = resultItem.address.premises;
                        if (resultItem.address.region != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = resultItem.address.region;
                    }

                    if (resultItem.appointed_on != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = resultItem.appointed_on;

                    if (resultItem.appointmentResponse != null)
                    {
                        if (resultItem.appointmentResponse.date_of_birth != null)
                        {
                            if (resultItem.appointmentResponse.date_of_birth.day != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = resultItem.appointmentResponse.date_of_birth.day;
                            if (resultItem.appointmentResponse.date_of_birth.month != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = resultItem.appointmentResponse.date_of_birth.month;
                            if (resultItem.appointmentResponse.date_of_birth.year != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = resultItem.appointmentResponse.date_of_birth.year;
                        }

                        if (resultItem.appointmentResponse.is_corporate_officer != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = resultItem.appointmentResponse.is_corporate_officer;
                        if (resultItem.appointmentResponse.name != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = resultItem.appointmentResponse.name;
                    }

                    if (resultItem.appointmentResponse.items != null)
                        foreach (var appointment in resultItem.appointmentResponse.items)
                        {
                            if (appointment.address != null)
                            {
                                if (appointment.address.address_line_1 != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = appointment.address.address_line_1;
                                if (appointment.address.address_line_2 != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = appointment.address.address_line_2;
                                if (appointment.address.care_of != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = appointment.address.care_of;
                                if (appointment.address.country != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = appointment.address.country;
                                if (appointment.address.locality != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = appointment.address.locality;
                                if (appointment.address.postal_code != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = appointment.address.postal_code;
                                if (appointment.address.po_box != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = appointment.address.po_box;
                                if (appointment.address.premises != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = appointment.address.premises;
                                if (appointment.address.region != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = appointment.address.region;
                            }

                            if (appointment.appointed_before != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = appointment.appointed_before;
                            if (appointment.appointed_on != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = appointment.appointed_on;

                            if (appointment.appointed_to != null)
                            {
                                if (appointment.appointed_to.company_name != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = appointment.appointed_to.company_name;
                                if (appointment.appointed_to.company_number != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = appointment.appointed_to.company_number;
                                if (appointment.appointed_to.company_status != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = appointment.appointed_to.company_status;
                            }

                            if (appointment.country_of_residence != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = appointment.country_of_residence;

                            if (appointment.former_names != null)
                                foreach (var name in appointment.former_names)
                                {
                                    if (name.forenames != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = name.forenames;
                                    if (name.surname != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = name.surname;
                                }

                            if (appointment.identification != null)
                            {
                                if (appointment.identification.identification_type != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = appointment.identification.identification_type;
                                if (appointment.identification.legal_authority != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = appointment.identification.legal_authority;
                                if (appointment.identification.legal_form != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = appointment.identification.legal_form;
                                if (appointment.identification.place_registered != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = appointment.identification.place_registered;
                                if (appointment.identification.registration_number != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = appointment.identification.registration_number;
                            }

                            if (appointment.is_pre_1992_appointment != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = appointment.is_pre_1992_appointment;

                            if (appointment.links != null)
                                if (appointment.links.company != null)
                                    metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = appointment.links.company;

                            if (appointment.name != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = appointment.name;

                            if (appointment.name_elements != null)
                            {
                                if (appointment.name_elements.forename != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = appointment.name_elements.forename;
                                if (appointment.name_elements.honours != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = appointment.name_elements.honours;
                                if (appointment.name_elements.other_forenames != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = appointment.name_elements.other_forenames;
                                if (appointment.name_elements.surname != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = appointment.name_elements.surname;
                                if (appointment.name_elements.title != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = appointment.name_elements.title;
                            }

                            if (appointment.nationality != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = appointment.nationality;
                            if (appointment.occupation != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = appointment.occupation;
                            if (appointment.officer_role != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = appointment.officer_role;
                            if (appointment.resigned_on != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = appointment.resigned_on;
                        }

                    if (resultItem.country_of_residence != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = resultItem.country_of_residence;
                    if (resultItem.date_of_birth.day != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = resultItem.date_of_birth.day;
                    if (resultItem.date_of_birth.month != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = resultItem.date_of_birth.month;
                    if (resultItem.date_of_birth.year != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = resultItem.date_of_birth.year;

                    if (resultItem.disqualifiedCorporateResponse != null)
                    {
                        if (resultItem.disqualifiedCorporateResponse.date_of_birth != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = resultItem.disqualifiedCorporateResponse.date_of_birth;

                        if (resultItem.disqualifiedCorporateResponse.disqualifications != null)
                            foreach (var disqualification in resultItem.disqualifiedCorporateResponse.disqualifications)
                            {
                                if (disqualification.undertaken_on != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = disqualification.undertaken_on;

                                if (disqualification.address != null)
                                {
                                    if (disqualification.address.address_line_1 != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = disqualification.address.address_line_1;
                                    if (disqualification.address.address_line_2 != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = disqualification.address.address_line_2;
                                    if (disqualification.address.country != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = disqualification.address.country;
                                    if (disqualification.address.locality != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = disqualification.address.locality;
                                    if (disqualification.address.postal_code != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = disqualification.address.postal_code;
                                    if (disqualification.address.premises != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = disqualification.address.premises;
                                    if (disqualification.address.region != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = disqualification.address.region;
                                }

                                if (disqualification.case_identifier != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = disqualification.case_identifier;

                                if (disqualification.company_names != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = string.Format(",", disqualification.company_names);

                                if (disqualification.court_name != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = disqualification.court_name;
                                if (disqualification.disqualification_type != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = disqualification.disqualification_type;
                                if (disqualification.disqualified_from != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = disqualification.disqualified_from;
                                if (disqualification.disqualified_until != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = disqualification.disqualified_until;
                                if (disqualification.heard_on != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = disqualification.heard_on;

                                if (disqualification.last_variation != null)
                                {
                                    if (disqualification.last_variation.case_identifier != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = disqualification.last_variation.case_identifier;
                                    if (disqualification.last_variation.court_name != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = disqualification.last_variation.court_name;
                                    if (disqualification.last_variation.varied_on != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = disqualification.last_variation.varied_on;
                                }

                                if (disqualification.reason != null)
                                {
                                    if (disqualification.reason.act != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = disqualification.reason.act;
                                    if (disqualification.reason.article != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = disqualification.reason.article;
                                    if (disqualification.reason.description_identifier != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = disqualification.reason.description_identifier;
                                    if (disqualification.reason.section != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = disqualification.reason.section;
                                }

                                if (disqualification.undertaken_on != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = disqualification.undertaken_on;
                            }

                        if (resultItem.disqualifiedCorporateResponse != null)
                        {
                            if (resultItem.disqualifiedCorporateResponse.forename != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = resultItem.disqualifiedCorporateResponse.forename;
                            if (resultItem.disqualifiedCorporateResponse.honours != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = resultItem.disqualifiedCorporateResponse.honours;
                            if (resultItem.disqualifiedCorporateResponse.kind != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = resultItem.disqualifiedCorporateResponse.kind;

                            if (resultItem.disqualifiedCorporateResponse.links != null)
                                if (resultItem.disqualifiedCorporateResponse.links.self != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = resultItem.disqualifiedCorporateResponse.links.self;

                            if (resultItem.disqualifiedCorporateResponse.nationality != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = resultItem.disqualifiedCorporateResponse.nationality;
                            if (resultItem.disqualifiedCorporateResponse.other_forenames != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = resultItem.disqualifiedCorporateResponse.other_forenames;


                            if (resultItem.disqualifiedCorporateResponse.permissions_to_act != null)
                                foreach (var permissions in resultItem.disqualifiedCorporateResponse.permissions_to_act)
                                {
                                    if (permissions.court_name != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = permissions.court_name;
                                    if (permissions.expires_on != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = permissions.expires_on;
                                    if (permissions.granted_on != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = permissions.granted_on;
                                    if (permissions.company_names != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = string.Join(",", permissions.company_names);
                                }

                            if (resultItem.disqualifiedCorporateResponse.surname != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = resultItem.disqualifiedCorporateResponse.surname;
                            if (resultItem.disqualifiedCorporateResponse.title != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = resultItem.disqualifiedCorporateResponse.title;
                        }
                    }

                    if (resultItem.disqualifiedNaturalResponse != null)
                    {
                        if (resultItem.disqualifiedNaturalResponse.date_of_birth != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = resultItem.disqualifiedNaturalResponse.date_of_birth;

                        if (resultItem.disqualifiedNaturalResponse.disqualifications != null)
                            foreach (var disqualification in resultItem.disqualifiedNaturalResponse.disqualifications)
                            {
                                if (disqualification.undertaken_on != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = disqualification.undertaken_on;

                                if (disqualification.address != null)
                                {
                                    if (disqualification.address.address_line_1 != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = disqualification.address.address_line_1;
                                    if (disqualification.address.address_line_2 != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = disqualification.address.address_line_2;
                                    if (disqualification.address.country != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = disqualification.address.country;
                                    if (disqualification.address.locality != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = disqualification.address.locality;
                                    if (disqualification.address.postal_code != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = disqualification.address.postal_code;
                                    if (disqualification.address.premises != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = disqualification.address.premises;
                                    if (disqualification.address.region != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = disqualification.address.region;
                                }

                                if (disqualification.case_identifier != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = disqualification.case_identifier;
                                if (disqualification.company_names != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = string.Format(",", disqualification.company_names);


                                if (disqualification.court_name != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = disqualification.court_name;
                                if (disqualification.disqualification_type != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = disqualification.disqualification_type;
                                if (disqualification.disqualified_from != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = disqualification.disqualified_from;
                                if (disqualification.disqualified_until != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = disqualification.disqualified_until;
                                if (disqualification.heard_on != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = disqualification.heard_on;

                                if (disqualification.last_variation != null)
                                {
                                    if (disqualification.last_variation.case_identifier != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = disqualification.last_variation.case_identifier;
                                    if (disqualification.last_variation.court_name != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = disqualification.last_variation.court_name;
                                    if (disqualification.last_variation.varied_on != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = disqualification.last_variation.varied_on;
                                }

                                if (disqualification.reason != null)
                                {
                                    if (disqualification.reason.act != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = disqualification.reason.act;
                                    if (disqualification.reason.article != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = disqualification.reason.article;
                                    if (disqualification.reason.description_identifier != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = disqualification.reason.description_identifier;
                                    if (disqualification.reason.section != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = disqualification.reason.section;
                                }

                                if (disqualification.undertaken_on != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = disqualification.undertaken_on;
                            }

                        if (resultItem.disqualifiedNaturalResponse.forename != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = resultItem.disqualifiedNaturalResponse.forename;
                        if (resultItem.disqualifiedNaturalResponse.honours != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = resultItem.disqualifiedNaturalResponse.honours;
                        if (resultItem.disqualifiedNaturalResponse.kind != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = resultItem.disqualifiedNaturalResponse.kind;

                        if (resultItem.disqualifiedNaturalResponse.links != null)
                            if (resultItem.disqualifiedNaturalResponse.links.self != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = resultItem.disqualifiedNaturalResponse.links.self;

                        if (resultItem.disqualifiedNaturalResponse.nationality != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = resultItem.disqualifiedNaturalResponse.nationality;
                        if (resultItem.disqualifiedNaturalResponse.other_forenames != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = resultItem.disqualifiedNaturalResponse.other_forenames;

                        if (resultItem.disqualifiedNaturalResponse.permissions_to_act != null)
                            foreach (var permissions in resultItem.disqualifiedNaturalResponse.permissions_to_act)
                            {
                                if (permissions.court_name != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = permissions.court_name;
                                if (permissions.expires_on != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = permissions.expires_on;
                                if (permissions.granted_on != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = permissions.granted_on;
                                if (permissions.company_names != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = string.Join(",", permissions.company_names);
                            }

                        if (resultItem.disqualifiedNaturalResponse.surname != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = resultItem.disqualifiedNaturalResponse.surname;
                        if (resultItem.disqualifiedNaturalResponse.title != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = resultItem.disqualifiedNaturalResponse.title;
                    }

                    if (resultItem.identification != null)
                    {
                        if (resultItem.identification.identification_type != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = resultItem.identification.identification_type;
                        if (resultItem.identification.place_registered != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = resultItem.identification.place_registered;
                        if (resultItem.identification.registration_number != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = resultItem.identification.registration_number;
                    }

                    if (resultItem.links != null)
                    {
                        if (resultItem.links.officer != null)
                            if (resultItem.links.officer.appointments != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = resultItem.links.officer.appointments;
                    }

                    if (resultItem.name != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = resultItem.name;
                    if (resultItem.nationality != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = resultItem.nationality;
                    if (resultItem.occupation != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = resultItem.occupation;
                    if (resultItem.officer_role != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = resultItem.officer_role;
                    if (resultItem.resigned_on != null) metadata.Properties[CompanyHouseVocabulary.Person.Nationality] = resultItem.resigned_on;
                }
            }
        }
    }
}
