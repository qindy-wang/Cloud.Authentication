using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cloud.Authentication.Services
{
    public interface IGraphClientService
    {
         Task<GraphServiceClient> GetGraphServiceClientByIdentity(string username, string password);

         GraphServiceClient GetGraphServiceClientByApp(string tenantId, string clientId, string clientSecret);
    }

    public enum ClientType
    { 
        Delegated = 0,
        Application = 1
    }
}
