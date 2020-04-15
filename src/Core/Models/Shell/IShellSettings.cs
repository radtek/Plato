﻿using System;
using System.Collections.Generic;

namespace PlatoCore.Models.Shell
{
    public interface IShellSettings
    {

        string Name { get; set; }

        string Location { get; set; }

        string ConnectionString { get; set; }

        /// <summary>
        /// For example https://site1.url.com/, https://site2.url.com/, https://site3.url.com/ etc
        /// </summary>
        string RequestedUrlHost { get; set; }

        /// <summary>
        /// For example https://url.com/site1, https://url.com/site2, https://url.com/site3, etc
        /// </summary>
        string RequestedUrlPrefix { get; set; }

        /// <summary>
        /// Unique database table prefix for the shell.
        /// </summary>
        string TablePrefix { get; set; }
        
        string DatabaseProvider { get; set; }

        /// <summary>
        /// The default theme for the shell.
        /// </summary>
        string Theme { get; set; }

        string AuthCookieName { get; }

        /// <summary>
        /// Indicates the current shell is the host shell capable of managing tenants. 
        /// This should be true upon initial set-up but false for all subsequent tenants
        /// </summary>
        bool IsHost { get; set; }

        /// <summary>
        /// The shell state. 
        /// </summary>
        TenantState State { get; set; }

        string this[string key] { get; }

        IEnumerable<string> Keys { get; }

        IDictionary<string, string> Configuration { get; }
    }

}
