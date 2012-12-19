#West Wind Application Configuration
###Strongly typed configuration classes for .NET Applications###
.NET library that provides for code-first creation of configuration settings
using strongly typed .NET classes. Configuration data can be mapped to various
configuration stores and can auto-sync from store to class and vice versa.
You can use standard .NET config files, sections and external files, or 
other custom stores including plain XML files, strings or a database.
Additional custom stores can also be created easily.

Unlike the built-in .NET Configuration Manager, the classes you
create are strongly typed and automatically convert config store values
to strong types. You can also write configuration data from the class to
the configuration store and if a store or store value doesn't exist it's
automatically created (provided permissions allow it).

This library provides:

* Strongly typed classes for configuration values
* Automatic type conversion from configuration store to class properties
* Default values for configuration values (never worry about read failures)
* Optional encryption of individual keys
* Automatic creation of configuration store if it doesn't exist
* Automatic addition of values that don't exist in configuration store
  (your store stays in sync with your class automatically)
* Support for multiple configuration objects simultaneously
* Works in any kind of .NET application: Web, Desktop, Console, Service...

Default Configuration Storage formats:

* Standard .NET .config files
	* Custom Configuration Sections
	* External Configuration Files
    * AppSettings
* Standalone, plain XML files
* Strings
* Sql Server Tables
* Customizable to create your own Configuration Providers

More detailed documentation is available as part of the Westwind.WebToolkit
here:
* [Main Product Page](http://west-wind.com/westwind.applicationconfiguration)
* [Westwind.ApplicationConfiguration Documentation](http://west-wind.com/westwind.applicationconfiguration/docs)
* [Westwind.ApplicationConfiguration Class Reference](hhttp://west-wind.com/westwind.ApplicationConfiguration/docs?page=_3lf07k2cg.htm)

West Wind Application Configuration is also part of: 
[West Wind Web and Ajax Toolkit's Westwind.Utilities library](http://west-wind.com/WestwindWebToolkit/)

## Getting Started
To create configurations, simply create a class that holds properties
for each configuration value. The class maps configuration values
stored in the configuration file. Values are stored as string in 
the configuration store, but are accessed as strongly typed values
in your class.

The library allows for reading and writing of configuration data
(assuming you have permissions) and assignment of default values
if values don't exist in the configuration store. Your class ALWAYS
has default values.

To use you simply create a class derived from AppConfiguration and add properties:

	public class ApplicationConfiguration : Westwind.Utilities.Configuration.AppConfiguration
	{    
	    public string ApplicationTitle { get; set; }
	    public string ConnectionString {get; set; }
	    public DebugModes DebugMode {get; set; }  // enum
	    public int MaxPageItems {get; set; }   // number

	    public ApplicationConfiguration()
	    {
	           ApplicationTitle = "West Wind Web Toolkit Sample";
	           DebugMode = DebugModes.ApplicationErrorMessage;
	           MaxPageItems = 20;
	    }
	}

Each property maps to a configuration store setting.

To use the class you simply create an instance and call Initialize() then
read configuration values that were read from the configuration store, or
default values if store values don't exist:

	// Create an instance - typically you'd use a static singleton instance
	var config = new ApplicationConfiguration();
	config.Initialize();  

	// Now read values retrieved from web.config/ApplicationConfiguration Section
    // If write access is available, the section is auto-created if it doesn't exist
	string title = config.ApplicationTitle;
	DebugModes modes = config.DebugMode;  
	
	// You can also update values
	config.MaxPageItems = 15;
	config.DebugMode = DebugModes.ApplicationErrorMessage;  
	
	// Save values to configuration store if permissions allow
	config.Write();

The above instantiation works, but typically in an application you'll want
to reuse the configuration object without having to reinstantiate it each time.

More effectively, create a static instance in application scope and initialize
it once, then re-use everywhere in your application or component:

	public class App
	{
	    // static property on any class in your app or component
	    public static ApplicationConfiguration Configuration { get; set; }
	
	    // static constructor ensures this code runs only once 
	    // the first time any static property is accessed
	    static App()
	    {
	        /// Load the properties from the Config store
	        Configuration = new ApplicationConfiguration();
	        Configuration.Initialize();
	    }
	}

You can then use the configuration class anywhere, globally without recreating:

	int maxItems = App.Configuration.MaxPageItems;
	DebugModes mode = App.Configuration.DebugMode;

Once instantiated you can also use Read() and Write() to re-read or
write values to the underlying configuration store.

Note that you can easily create multiple application configuration classes,
which is great for complex apps that need to categorize configuration,
or for self-contained components that need to handle their own internal
configuration settings.

## Configuration Providers
By default configuration information is stored in standard config files.
When calling the stock Initialize() method with no parameters, you get
configuration settings stored in an app/web.config file with
a section that matches the class name.

To customize the configuration provider you can create an instance
and pass in one of the providers with customizations applied:

    public static App 
    {
		App.Config = new AutoConfigFileConfiguration();

		// Create a customized provider to set provider options
		// Note: several different providers are available    
		var provider = new ConfigurationFileConfigurationProvider<AutoConfigFileConfiguration>()
		{
			ConfigurationSection = "CustomConfiguration",
			EncryptionKey = "seekrit123",
			PropertiesToEncrypt = "MailServer,MailServerPassword"                
		};

		App.Config.Initialize(provider);  
	}

Alternately you can abstract the above logic directly into your configuration
class by overriding the OnInitialize() method to provide your default 
initialization logic which keeps all configuration related logic in 
one place.

The following creates a new configuration using the Database provider to store
the configuration information:

    public class DatabaseConfiguration : Westwind.Utilities.Configuration.AppConfiguration
    {
        public DatabaseConfiguration()
        {
            ApplicationName = "Configuration Tests";
            DebugMode = DebugModes.Default;
            MaxDisplayListItems = 15;
            SendAdminEmailConfirmations = false;
            Password = "seekrit";
            AppConnectionString = "server=.;database=hosers;uid=bozo;pwd=seekrit;";
        }

        public string ApplicationName { get; set; }
        public DebugModes DebugMode { get; set; }
        public int MaxDisplayListItems { get; set; }
        public bool SendAdminEmailConfirmations { get; set; }
        public string Password { get; set; }
        public string AppConnectionString { get; set; }
        
        /// <summary>
        /// Override this method to create the custom default provider - in this case a database
        /// provider with a few options. Config data can be passed in for connectionstring and table
        /// </summary>
        protected override IConfigurationProvider OnCreateDefaultProvider(string sectionName, object configData)
        {
            // default connect values
            string connectionString =  "LocalDatabaseConnection";
            string tableName = "ConfigurationData";

            // ConfigData: new { ConnectionString = "...", Tablename = "..." }
            if (configData != null)
            {
                dynamic data = configData;
                connectionString = data.ConnectionString;
                tableName = data.Tablename;                       
            }

            var provider = new SqlServerConfigurationProvider<DatabaseConfiguration>()
            {
                ConnectionString = connectionString,
                Tablename = tableName,
                ProviderName = "System.Data.SqlServerCe.4.0",
                EncryptionKey = "ultra-seekrit",  // use a generated value here
                PropertiesToEncrypt = "Password,AppConnectionString"
                // UseBinarySerialization = true                     
            };

            return provider;
        }    

        /// <summary>
        /// Optional - simplify Initialize() for database provider default
        /// </summary>
        public void Initialize(string connectionString, string tableName = null)
        {
            base.Initialize(configData: new { ConnectionString = connectionString, Tablename = tableName });
        }
    }

You can override the OnCreateDefaultProfile() method and configure a provider, or the slightly
higher level OnInitialize() which creates a provider and then reads the content. Either one allows
customization of the default Initialization() when Initialize is called with no explicit Provider.

##Multiple Configuration Stores
To create multiple configuration stores simply create multiple classes and 
access each class individually. A single app can easily have multiple configuration
classes to separate distinct sections or tasks within an application.
Ideally you store the configuration objects on a global static instance like this:

	App.Configuration = new MyApplicationConfiguration(null);
	App.AdminConfiguration = new AdminConfiguration(null);

This allows for nice compartmentalization of configuration settings and
also for multiple components/assemblies to have their own private 
configuration settings.

## Class Structure
This library consists of the main AppConfiguration class plus provider
logic. Providers are based on a IConfigurationProvider interface with
a ConfigurationProviderBase class providing base functionality.

Here's the complete class layout:
![Classes](https://raw.github.com/RickStrahl/Westwind.ApplicationConfiguration/Version2_InitializationChanges/AppConfiguration.png)

##Many More Options
Many more configuration options are available. Please check the full documentation
for more information.

* [Westwind.ApplicationConfiguration Documentation](http://west-wind.com/westwindwebtoolkit/docs?page=_2le027umn.htm)
* [Westwind.ApplicationConfiguration Class Reference](http://west-wind.com/westwindwebtoolkit/docs?page=_3ff0psdpu.htm)