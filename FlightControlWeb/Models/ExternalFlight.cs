﻿using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class ExternalFlight
    {
        private SqliteDB db = SqliteDB.Instance;

        //private string url;
        //private List<Flight> flight;



        public ExternalFlight()
        {
           
            //url = "api/Flights?relative_to="+time;
           
           
        }
        //"api/Flights?relative_to="+time
        public async Task<List<Flight>> GetRequestAsync(string url)
        {
            List<Flight> flight = new List<Flight>();
            List<ServerFlight> servers = db.GetServers();
            foreach (ServerFlight s in servers)
            {
                //HttpClient httpClient = new HttpClient();
                //httpClient.BaseAddress = new Uri(s.ServerUrl);
                ////add header fields for jyson
                //httpClient.DefaultRequestHeaders.Add("User-Agent", "C# console program");
                //httpClient.DefaultRequestHeaders.Accept.Add(
                //        new MediaTypeWithQualityHeaderValue("application/json"));
                HttpClient httpClient = BuildHttpClient(s);
                try
                { 
                    HttpResponseMessage response = await httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    var resp = await response.Content.ReadAsStringAsync();

                    List<Flight> newFlight = JsonConvert.DeserializeObject<List<Flight>>(resp);
                    flight.AddRange(newFlight);
                } catch(Exception) {
                    Console.WriteLine("failed in external filght get response");
                }
                
            }
            changeStatusFlight(flight);
            return flight;
            
        }

        public async Task<FlightPlan> GetExternalFlightPlanAsync(string id)
        {
            FlightPlan flightPlan = null;
            List<ServerFlight> servers = db.GetServers();
            string url = "api/FlightPlan/" + id;
            foreach (ServerFlight s in servers)
            {
                HttpClient httpClient = BuildHttpClient(s);
                try
                {

                    string resp = await ResponceAsync(httpClient, url);
                    flightPlan = JsonConvert.DeserializeObject<FlightPlan>(resp);
                }
                catch (Exception)
                {
                    Console.WriteLine("failed in external filghtPlan by id get response");
                }
            }
            return flightPlan;
        }

        public async Task<List<Flight>> GetExternalFlightAsync(string time)
        {
            List<Flight> flight = new List<Flight>();
            List<ServerFlight> servers = db.GetServers();
            string url = "api/Flights?relative_to=" + time;
            foreach (ServerFlight s in servers)
            {
                HttpClient httpClient = BuildHttpClient(s);
                try
                {
                   
                    string resp = await ResponceAsync(httpClient, url);
                    List<Flight> newFlight = JsonConvert.DeserializeObject<List<Flight>>(resp);
                    flight.AddRange(newFlight);
                }
                catch (Exception)
                {
                  Console.WriteLine("failed in external filght get response");
                }
            }
            changeStatusFlight(flight);
            return flight;
        }
        private async Task<string> ResponceAsync(HttpClient httpClient, string url)
        {
            HttpResponseMessage response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string resp = await response.Content.ReadAsStringAsync();
            return resp;
        }
        private void changeStatusFlight(List<Flight> flight)
        {
           foreach(Flight f in flight){
                f.IsExternal = true;
           }
        }
        //build new Http client for each request from extenal server
        private HttpClient BuildHttpClient(ServerFlight s)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(s.ServerUrl);
            //add header fields for jyson
            httpClient.DefaultRequestHeaders.Add("User-Agent", "C# console program");
            httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
            return httpClient;
        }
       
    }
}
