using System;
using System.Collections.Generic;
using System.Text;
using MyApp.ServiceModel;
using ServiceStack;

namespace MyApp
{
    public class MyService : Service
    {
        public object Any(Hello request) => new HelloResponse
        {
            Result = $"Hello, {request.Name}!"
        };
    }
}
