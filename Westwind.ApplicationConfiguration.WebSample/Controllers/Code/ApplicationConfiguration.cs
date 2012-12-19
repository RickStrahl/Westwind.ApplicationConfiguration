using System.ComponentModel;
using System.Web.Security;
using Westwind.Utilities.Configuration;

namespace ApplicationConfigurationWeb
{

    /// <summary>
    /// Application specific config class that holds global configuration values
    /// that are read and optionally persisted into a configuration store.
    /// </summary>    
    public class ApplicationConfiguration : AppConfiguration
    {

        /// <summary>
        /// Override default provider creation by using custom options
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="configData"></param>
        /// <returns></returns>
        protected override IConfigurationProvider OnCreateDefaultProvider(string sectionName, object configData)
        {
            
            var provider = new ConfigurationFileConfigurationProvider<ApplicationConfiguration>()
            {
                // use the machine key to seed encoding key string or provide any other string
                EncryptionKey = MachineKey.Encode(new byte[] { 3, 233, 8, 11, 32, 44 }, 
                                                    MachineKeyProtection.Encryption),                    
                PropertiesToEncrypt = "MailServerPassword,ConnectionString",
                // Custom section - if not specified goes to AppSettings
                ConfigurationSection = "ApplicationConfiguration"
            };

            return provider;

            // Example of Sql configuration
            //var provider = new SqlServerConfigurationProvider<ApplicationConfiguration>()
            //{
            //    FieldsToEncrypt = "MailServerPassword,ConnectionString",
            //    EncryptKey = "secret",
            //    ConnectionString = "DevSampleConnectionString",
            //    Tablename = "Configuration",
            //    Key = 1
            //};
            // Example of external XML configuration - advantage: Supports complex object hierarchies
            //var provider = new XmlFileConfigurationProvider<ApplicationConfiguration>()
            //{
            //    FieldsToEncrypt = "MailServerPassword,ConnectionString",
            //    EncryptKey = "secret",
            //    XmlConfigurationFile = HttpContext.Current.Server.MapPath("~/Configuration.xml")
            //};
        }

        // Define properties and defaults

        public ApplicationConfiguration()
        {
            ApplicationTitle = "Configuration Sample";
            ApplicationSubTitle = "Making ASP.NET Easier to use";
            ApplicationCookieName = "_ApplicationId";
        }

        /// <summary>
        /// The main title of the Weblog
        /// </summary>
        public string ApplicationTitle  {get; set; }
                

        /// <summary>
        /// The subtitle for the Web Log displayed on the banner
        /// </summary>
        public string ApplicationSubTitle   {get; set; }

        
        /// <summary>
        /// Application Cookie name used for user tracking
        /// </summary>
        public string ApplicationCookieName  {get; set; }
        
        
        #region System Settings
        /// <summary>
        /// The database connection string for this WebLog instance
        /// </summary>
        public string ConnectionString {get; set; }

        
        /// <summary>
        /// Determines how errors are displayed
        /// Default - ASP.NET Default behavior
        /// ApplicationErrorMessage - Application level error message
        /// DeveloperErrorMessage - StackTrace and full error info
        /// </summary>
        public DebugModes DebugMode {get; set; }
        
        /// <summary>
        /// Determines how many items are displayed per page in typical list displays
        /// </summary>
        public int MaxPageItems   {get; set; }
        
        #endregion

        #region Email Settings

        /// <summary>
        /// The Email Address used to send out emails
        /// </summary>
        public string SenderEmailAddress   {get; set; }
        

        /// <summary>
        /// The Name of the senders Email 
        /// </summary>
        public string SenderEmailName   {get; set; }
        
        /// <summary>
        /// Optional CC List for email confirmations to customers.
        /// </summary>
        public string MailCc  {get; set; }


        /// <summary>
        /// Email address use for admin emails
        /// </summary>
        public string AdminEmailAddress   {get; set; }

        /// <summary>
        /// Email name used for Admin emails
        /// </summary>
        public string AdminEmailName {get; set; }

        /// <summary>
        /// Determines whether administrative emails are sent if
        /// the email addresses are set
        /// </summary>
        public bool AdminSendEmails {get; set; }


        /// <summary>
        /// The IP address or domain name of the mail server
        /// used to send email notifications and admin 
        /// alerts through
        /// </summary>
        public string MailServer {get; set; }


        /// <summary>
        /// Mail Server username if required
        /// </summary>
        public string MailServerUsername {get; set; }


        /// <summary>
        /// Mail Server password if required
        /// </summary>        
        public string MailServerPassword { get; set; }

        #endregion
    }

    /// <summary>
    /// Different modes that errors are displayed in the application
    /// </summary>
    public enum DebugModes
    {
        Default,
        ApplicationErrorMessage,
        DeveloperErrorMessage
    }

}
