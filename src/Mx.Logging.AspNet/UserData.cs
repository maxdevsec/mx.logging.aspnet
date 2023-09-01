using System.Collections.Generic;

namespace Mx.Logging.AspNet
{
    public class UserData
    {
        public UserData()
        {
            Claims = new List<string>();
            UserId = string.Empty;
            UserName = string.Empty;
        }
        public string UserId { get; set; }

        public string UserName { get; set; }

        public ICollection<string> Claims { get; set; }


        public void AddClaim(string claimType, string value)
        {
            Claims.Add($"UserClaim - {claimType} Value: {value}");
        }
    }
}
