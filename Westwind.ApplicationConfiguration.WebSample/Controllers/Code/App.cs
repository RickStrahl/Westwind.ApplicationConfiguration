using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Westwind.Utilities.Configuration;

namespace ApplicationConfigurationWeb
{
    /// <summary>
    /// Summary description for Class1
    /// </summary>
    public class App
    {
        public static ApplicationConfiguration Configuration { get; set; }

	     static App()
	    {
            // Create an instance of the class with default provider
            Configuration = new ApplicationConfiguration();
            Configuration.Initialize();
	    }
    }
}