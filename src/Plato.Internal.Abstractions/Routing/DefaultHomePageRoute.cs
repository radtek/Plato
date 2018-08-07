﻿using Microsoft.AspNetCore.Routing;

namespace Plato.Internal.Abstractions.Routing
{
    public class DefaultHomePageRoute : RouteValueDictionary
    {
        public DefaultHomePageRoute()
        {
            this["Area"] = "Plato.Users";
            this["Controller"] = "Account";
            this["Action"] = "Login";
        }
    }
}
