using System.Web.Mvc;
using ApplicationConfigurationWeb;
using Westwind.Utilities;
using Westwind.Utilities.Configuration;

namespace ApplicationConfigurationMvc.Controllers
{
    public class SamplesController : Controller
    {
        private const string STR_SUPERSECRET = "SUPERSECRET";

        /// <summary>
        /// Local instance of the configuration class that we'll end up binding to
        /// </summary>
        public ApplicationConfiguration AppConfig = null;

        
        /// <summary>
        /// Just display Configuration data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index(SamplesViewModel model)
        {
            if (model == null)
                model = new SamplesViewModel();

            this.LoadAppConfiguration(model);

            model.Configuration = this.AppConfig;
            //model.ErrorMessage = "Hello World";
            return View("Index",model);
        }

        [HttpPost]
        public ActionResult Save(SamplesViewModel model)
        {
            bool isSave = !string.IsNullOrEmpty(Request.Form["btnSubmit"]);
            if (!isSave)                
                return Index(model); 

            this.LoadAppConfiguration(model);

            if (this.AppConfig == null)
            {
                // Load the raw Config object without loading anything
                AppConfig = new ApplicationConfiguration();                
            }

            // Read all Formvars directly into the AppConfig
            WebUtils.FormVarsToObject(this.AppConfig, "Configuration.");

            if (model.ConfigTypeSelection == "Default Web.Config")
            {
                // This is simple
                if (this.AppConfig.Write())
                    model.ShowMessage("Keys have been written to Web.Config in the AppConfiguration Section.<hr>" +
                        "For best observation open your web.config and view as values get changed. Note " +
                        "that the Connection String and	MailServerPassword fields are encrypted. Based " +
                        "on the constructor which calls SetEncryption with the field names.");
                else
                    model.ShowError("Writing the keys to config failed... Make sure your permissions are set. " +
                                   AppConfig.ErrorMessage);
            }

            // Writing to WebStoreConfig section in Web.Config
            else if (model.ConfigTypeSelection == "Different Web.Config Section")
            {
                if (this.AppConfig.Write())
                    model.ShowMessage("Keys have been written to Web.Config in the WebStoreConfig Section.<hr>" +
                        "For best observation open your web.config and view as values get changed. Note " +
                        "that the Connection String and	MailServerPassword fields are encrypted. Based " +
                        "on the constructor which calls SetEncryption with the field names.");
                else
                    model.ShowError("Writing the keys to config failed... Make sure your permissions are set. " +
                                   AppConfig.ErrorMessage);
            }

            // Separate Configuration File with .config structure
            else if (model.ConfigTypeSelection == "Different .Config File")
            {
                if (this.AppConfig.Write())
                    model.ShowMessage("Keys have been written to a separate WebStore.Config in a WebStoreConfig Section.<hr>" +
                        "For best observation open WebStore.Config and view as values get changed. Note that changes in " +
                        "an external file will not automatically cause ASP.NET to recycle the Web application. However if you use " +
                        "a static property for the config object (unlike this example) the changes will be visible to all instances anyway");
                else
                    model.ShowError("Writing the keys to WebStore.Config failed... Make sure your permissions are set. " +
                                   AppConfig.ErrorMessage);
            }

            else if (model.ConfigTypeSelection == "Simple Xml File")
            {
                if (this.AppConfig.Write())
                    model.ShowMessage("Keys have been written to a separate XML file WebStoreConfig.Xml.<hr>" +
                        "For best observation open this file and view as values get changed. Note that changes in " +
                        "an external file will not automatically cause ASP.NET to recycle the Web application. However if you use " +
                        "a static property for the config object (unlike this example) the changes will be visible to all instances anyway");
                else
                    model.ShowError("Writing the keys to WebStoreConfig.Xml failed. For this mode to work the Web account need to have write access in the virtual folder." +
                                   AppConfig.ErrorMessage);
            }

            else if (model.ConfigTypeSelection ==  "String")
            {
                string XmlContent = this.AppConfig.WriteAsString();                

                if (!string.IsNullOrEmpty(XmlContent))
                {       
                        model.ShowMessage("Keys have been written to a string .<hr>" +
                        "The output looks like this:<p><pre>" +
                        Server.HtmlEncode(XmlContent) +
                        "</pre>" +
                        "This string is now written into View state and then retrieved from the Config object when reloading this page." +
                        "To see the persisted value, make a change to the settings, save, then move to a different mode, then come back - the change should be persisted. " +
                        "Note that with string and file output you can persist complex objects as long as objects are serializable.");
                        
                }
                else
                    model.ShowError("Writing the keys to string failed... Make sure your permissions are set. " + 
                                   AppConfig.ErrorMessage);
                
                
                HttpContext.Application["XmlString"] = XmlContent;
            }

            else if (model.ConfigTypeSelection == "Database")
            {
                if (this.AppConfig.Write())
                    model.ShowMessage("Keys have been written to the database .<hr>");
                else
                    model.ShowError("Writing the keys to the database failed <hr>" +
                                   "The connecton string or table namea and PK are incorrect. Use the database settings in AppConfiguration " +
                                   "to set up a connection string, then create a table called Config and add a text field called ConfigData and a PK field.");
            }
           


            model.Configuration = this.AppConfig;
            return View("Index",model);
        }

        private void LoadAppConfiguration(SamplesViewModel model)
        {            
           
            // Default Web.Config read with Constructor
            // at first access
            if (model.ConfigTypeSelection == "Default Web.Config")
            {
                // Simply assign the default config object - it gets loaded via 
                // the default constructor defined in AppConfig.cs
                this.AppConfig = App.Configuration;
                
                // force to re-read in case we updated previously
                AppConfig.Read();
            }
            

            // Explicit object creation for the remaining objects, 
            // but you can use the same static constructor approach
            // as with the above

            // Separate Web.Config Section
            else if (model.ConfigTypeSelection == "Different Web.Config Section")
            {
                var provider = new ConfigurationFileConfigurationProvider<ApplicationConfiguration>()
                {
                     ConfigurationSection = "WebStoreConfiguration",
                     PropertiesToEncrypt = "ConnectionString,MailServerPassword", 
                     EncryptionKey = STR_SUPERSECRET
                };
                AppConfig = new ApplicationConfiguration();
                AppConfig.Initialize(provider);
                //this.AppConfig.Read();
            }

                // Separate Web.AppConfig Section
            else if (model.ConfigTypeSelection == "Different .Config File")
            {
                
                var provider = new ConfigurationFileConfigurationProvider<ApplicationConfiguration>()
                {
                    ConfigurationFile= Server.MapPath("~/WebStore.config"),
                    ConfigurationSection = "WebStoreConfiguration",
                    PropertiesToEncrypt = "ConnectionString,MailServerPassword",
                    EncryptionKey = STR_SUPERSECRET
                };

                AppConfig = new ApplicationConfiguration();
                AppConfig.Initialize(provider);
                //this.AppConfig.Read();
            }

            else if (model.ConfigTypeSelection == "Simple Xml File")
            {
                var provider = new XmlFileConfigurationProvider<ApplicationConfiguration>()
                {
                    XmlConfigurationFile = Server.MapPath("WebStoreConfig.xml"),
                    PropertiesToEncrypt = "ConnectionString,MailServerPassword",
                    EncryptionKey = STR_SUPERSECRET
                };

                this.AppConfig = new ApplicationConfiguration();
                AppConfig.Initialize(provider);
                //this.AppConfig.Read();
            }
            else if (model.ConfigTypeSelection == "String")
            {

                string XmlString = HttpContext.Application["XmlString"] as string;
                AppConfig = new ApplicationConfiguration();

                if (XmlString != null)
                {
                    // You can always read from an XML Serialization string w/o
                    // any provider setup
                    AppConfig.Read(XmlString);
                }
            }

            // Not implemented since you will need a database
            // this example uses the connection string configured in the Web.Config
            else if (model.ConfigTypeSelection == "Database")
            {
                var provider = new SqlServerConfigurationProvider<ApplicationConfiguration>()
                {
                    ConnectionString = "DevSampleConnectionString",
                    Tablename = "ConfigData",
                    Key = 1,
                    PropertiesToEncrypt = "ConnectionString,MailServerPassword",
                    EncryptionKey = STR_SUPERSECRET
                };

                AppConfig = new ApplicationConfiguration();
                AppConfig.Initialize(provider);

                if (!this.AppConfig.Read())
                    model.ShowError(
                      "Unable to connect to the Database.<hr>" +
                      "This database samle uses the connection string in the configuration settings " +
                      "with a table named 'ConfigData' and a field named 'ConfigData' to hold the " +
                      "configuration settings. If you have a valid connection string you can click " +
                      "on Save Settings to force the table and a single record to be created.<br><br>" +
                      "Note: The table name is parameterized (and you can change it in the default.aspx.cs page), but the field name always defaults to ConfigData.<hr/>" +
                      AppConfig.ErrorMessage);
            }
        }
      

    }
}