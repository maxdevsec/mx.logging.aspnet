using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Mx.Logging.AspNet
{
    public static class WebHelper
    {
        public static Dictionary<string, object> GetRequestData(HttpContext context)
        {

            var request = context.Request;

            var requestData = new Dictionary<string, object>();

            if (request != null)
            {
                requestData.Add("UserAgent", request.Headers["User-Agent"]);

                var queryString =
                    Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(request.QueryString.ToString());

                foreach (var queryStringKey in queryString.Keys)
                {
                    requestData.Add($"QueryString-{queryStringKey}", queryString[queryStringKey]);
                }
            }


            return requestData;
        }


        public static UserData GetUserData(HttpContext context)
        {
            var userData = new UserData();

            var user = context.User;  // ClaimsPrincipal.Current is not sufficient
            if (user != null)
            {
                foreach (var claim in user.Claims)
                {
                    if (claim.Type == ClaimTypes.NameIdentifier)
                        userData.UserId = claim.Value;
                    else if (claim.Type == "name")
                        userData.UserName = claim.Value;
                    else
                        userData.AddClaim(claim.Type, claim.Value);
                }
            }

            return userData;
        }
    }
}
