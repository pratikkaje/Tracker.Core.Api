﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using RESTFulSense.Clients;

namespace Tracker.Core.Api.Tests.Acceptance.Brokers
{
    public partial class TrackerCoreApiBroker
    {
        private readonly WebApplicationFactory<Program> webApplicationFactory;
        private readonly HttpClient httpClient;
        private readonly IRESTFulApiFactoryClient apiFactoryClient;

        public TrackerCoreApiBroker()
        {
            this.webApplicationFactory = 
                new WebApplicationFactory<Program>();

            this.httpClient = 
                this.webApplicationFactory.CreateClient();

            this.apiFactoryClient =
                new RESTFulApiFactoryClient(httpClient);
        }
    }
}
