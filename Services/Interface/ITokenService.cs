using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cloud.Authentication.Services
{
    public interface ITokenService
    {
        Task<string> GetAccessToken(string userName, string password, TokenType tokenType, string clientId = "");

        Task<string> GetAppAccessToken(string clientId, string clientSecret);
    }

    public enum TokenType
    {
        AzureAD = 1,
        EndpointManager = 2,
        SharePoint = 3
    }
}
