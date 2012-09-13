#West Wind Application Configuration

This library allows you to create strongly typed configuration classes
that map configuration data stored in a variety of configuration stores:

* Standard .NET .config files
* Specific Configuration Sections
* External Configuration Files
* Plain XML files
* Strings
* Sql Server Tables

In addition this library provides
* Encryption of individual keys
* Default values for configuration values
* Automatic creation of default values in configuration store
* Support for multiple configuration objects
* Works in any kind of .NET application: Web, Desktop, Console, Service...

More detailed documentation is available as part of the Westwind.WebToolkit
here:

* [Westwind.ApplicationConfiguration Documentation](http://west-wind.com/westwindwebtoolkit/docs/page=_2le027umn.htm)
* [Westwind.ApplicationConfiguration Class Reference](http://west-wind.com/westwindwebtoolkit/docs/?page=_3ff0psdpu.htm)

West Wind Application Configuration is also part of the full 
[West Wind Web and Ajax Toolkit's Westwind.Utilities library](http://west-wind.com/WestwindWebToolkit/)

## Getting Started
To create configurations, simply create a class that holds properties
for each configuration value. The class maps configuration values
stored in the configuration file. Values are stored as string in 
the configuration store, but accessed as strongly typed values
in your class.

The library allows for reading and writing of configuration data
(assuming you have permissions) and assignment of default values
if values don't exist in the configuration store. Your class ALWAYS
has default values.

To use you simply create a class and implement this minimal code:

	public class ApplicationConfiguration : Westwind.Utilities.Configuration.AppConfiguration
	{
	    public ApplicationConfiguration() : base(null,"MyConfiguration")
	    { }

		// default implementation - web.config or app.config and specify a section to write to	    
	    public ApplicationConfigurationIConfigurationProvider provider) 
	    		: base(null,"MyConfiguration")
	    { }
	
	    // put property initialization into this separate method
	    // Base constructors calls this to initialize instance before reading from store
	    public override void Initialize()
	    {
	           // set any default values or other init functionality
	           ApplicationTitle = "West Wind Web Toolkit Sample";
	           DebugMode = DebugModes.ApplicationErrorMessage;
	           MaxPageItems = 20;
	    }
	
	    // Create properties to read or persist to/from the config store
	    public string ApplicationTitle { get; set; }
	    public string ConnectionString {get; set; }
	    public DebugModes DebugMode {get; set; }  // enum
	    public int MaxPageItems {get; set; }   // number
	}

This example maps the class properties to a standard web.config 
file (or app.config in non-Web apps) and reads and (optionally) writes
data there. If write permissions are available the default settings
are written automatically to the configuration file as well. Even
if settings don't exist the classes default values will always be
available.

To use the class you simply create an instance:

	// Create an instance - typically you'd use a static singleton instance
	var config = new ApplicationConfiguration(null);
	
	// Read values - retrieved from web.config/MyApplicationConfiguration
	string title = config.ApplicationTitle;
	DebugModes modes = config.DebugMode;  
	
	// You can also update values
	config.MaxPageItems = 15;
	
	// And save changes to config store if permissions allow
	config.Write();

or, more effectively, create a static instance in application scope:

	public class App
	{
	    // static property on any class
	    public static ApplicationConfiguration Configuration { get; set; }
	
	    // static constructor ensures this code runs only once 
	    // the first time any static property is accessed
	    static App()
	    {
	        /// Load the properties from the Config store
	        Configuration = new ApplicationConfiguration(null);
	    }
	}

You can then use the configuration class globally without recreating:

	int maxItems = App.Configuration.MaxPageItems;
	DebugModes mode = App.Configuration.DebugMode;

You can create multiple application configuration classes, and store each
configuration settings in a different section, a different file or even an
entirely different configuration format.

## Configuration Providers
By default configuration information is stored in standard config files,
preferrably in custom sections. The example above demonstrates this
by simply calling the base constructors to provide default behavior
that stores data in the application .config file and a custom section.

To use a non-default provider or customize the provider with additional
information you can explicitly assign a provider by overriding the 
constructor and explicitly creating a provider object. Rather than 
the default constructor shown above you can use the following:

	public class MyApplicationConfiguration : Westwind.Utilities.Configuration.AppConfiguration
	{
	    /// <summary>
	    /// Always implement a default constructor so new instances
	    /// can be created by the various de-serialization config schemes.
	    /// </summary>
	    public ApplicationConfiguration() 
	    {
	         this.Initialize()
	    }


	    /// <summary>
	    /// By convention a second constructor that takes a Config Provider
	    /// or null as an optional parameter should be implemented. This
	    /// ctor should implement auto-load behavior and create a default
	    /// provider for the object. Typically called like this:
	    ///
	    /// App.Configuration = new ApplicationConfiguration(null);
	    /// 
	    /// Note: this constructor calls back to parameterless one to
	    /// ensure the default values are set
	    /// </summary>  
	    public ApplicationConfiguration(IConfigurationProvider provider)
	    {
	        this.Initialize();

	        if (provider == null)
	        {
	            // create the customized default configuration so you only have to configure this in one place
	            this.Provider = new ConfigurationFileConfigurationProvider<ApplicationConfiguration>()
	            {
	                PropertiesToEncrypt = "MailServerPassword,ConnectionString",
	                EncryptionKey = "secret",
	                ConfigurationSection = "MyConfiguration"
                        // ConfigurationFile = "MyConfigFile.config"
	            };
	        }
	        else
	            this.Provider = provider;

	        // now read the configuration data from the store
	        this.Provider.Read(this);
	    }

	    public string ApplicationTitle { get; set; }
	    public string ApplicationSubTitle { get; set; } 
	    public string ConnectionString {get; set; }
	    public DebugModes DebugMode {get; set; }
	    public int MaxPageItems {get; set; }

	    // centralized initialization so both constructors can use it
	    public override void Initialize()
	    {
	        // set any default values here
	        ApplicationTitle = "West Wind Web Toolkit Sample";
	        DebugMode = DebugModes.ApplicationErrorMessage;
	        MaxPageItems = 20;
	    }
	}

Here an ConfigurationFileProvider is explictly created when the provider is passed as null.
To fire this custom code:

	// Initialize global reference
	App.Configuration = new MyApplicationConfiguration(null);

and to use it in your application:

	var title = App.Configuration.ApplicationTitle;
	DebugModes mode = App.Configuration.DebugMode;


##Multiple Configuration Stores
To create multiple configuration stores simply create multiple classes and 
access each class. Ideally you store the configuration objects on a global
static instance like this:

	App.Configuration = new MyApplicationConfiguration(null);
	App.AdminConfiguration = new AdminConfiguration(null);

This allows for nice compartmentalization of configuration settings and
also allows multiple components/assemblies to use the same configuration
class, but store settings in separate locations.

##Many More Options
Many more configuration options are available. Please check the full documentation
for more information.

* [Westwind.ApplicationConfiguration Documentation](http://west-wind.com/westwindwebtoolkit/docs/page=_2le027umn.htm)
* [Westwind.ApplicationConfiguration Class Reference](http://west-wind.com/westwindwebtoolkit/docs/?page=_3ff0psdpu.htm)