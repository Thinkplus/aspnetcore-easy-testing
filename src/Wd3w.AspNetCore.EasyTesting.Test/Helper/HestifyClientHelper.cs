using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;
using Hestify;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Wd3w.AspNetCore.EasyTesting.Test.Helper
{
    public static class HestifyClientHelper
    {
        public static HestifyClient WithBearerToken(this HestifyClient client, string token)
        {
            return client.WithHeader(HttpRequestHeader.Authorization, $"Bearer ${token}");
        }

        public static HestifyClient WithFakeBearerToken(this HestifyClient client)
        {
            IdentityModelEventSource.ShowPII = true;
            var token = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
                "http://localhost", 
                "http://localhost",
                expires: DateTime.Now.AddDays(1),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes("ABCDABCDABCDABCDABCDABCDABCDABCD")), 
                    SecurityAlgorithms.HmacSha256)));

            return client.WithBearerToken(token);
        }
    }
}