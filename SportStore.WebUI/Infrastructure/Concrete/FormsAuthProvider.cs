using SportStore.WebUI.Infrastructure.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;


namespace SportStore.WebUI.Infrastructure.Concrete
{
    public class FormsAuthProvider : IAuthProvider
    {
        public bool Authenticate(string user, string password)
        {
            bool result = FormsAuthentication.Authenticate(user, password);
            if(result)
            {
                FormsAuthentication.SetAuthCookie(user, false);
            }
            return result;
        }
    }
}