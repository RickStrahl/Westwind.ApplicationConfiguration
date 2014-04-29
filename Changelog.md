#West Wind Application Configuration Change Log

###Version 2.20
*not released yet*

* **ConfigurationFile Configuration Provider support for Complex Types**
Added another option for serialization of flat complex objects, by 
implementing additional checks for a **static** FromString() method that
if found can be used to deserialize object. [more info](http://west-wind.com/westwindtoolkit/docs/?page=_1cx0ymket.htm)

* **ConfigurationFile Configuration Provider support for IList**
You can now also serialize IList objects into the config file. The list
is serialize a ItemList1,ItemList2,ItemList3 where ItemList is the name
of the property. Complex objects are supported with the new complex
type parsing support or TypeConverters. [more info](http://west-wind.com/westwindtoolkit/docs/?page=_1cx0ymket.htm)


###Version 2.15
*Nov. 14th, 2013*

* **JSON File Provider added**<br/>
You can now store configuration optionally using JSON. The new JsonFileConfigurationProvider
uses JSON.NET to provide JSON configuration output. JSON.NET is dynamically linked so
there's no hard dependency on it. If you use this provider make sure JSON.NET is added
to your project.

* **Updated Documentation**
Added this changelog as well as updating the [detailed help file documentation](http://west-wind.com/westwind.applicationconfiguration/docs) step by step instructions.


###Version 2.11
*Nov. 4, 2013*

* **Make XML Config File Reader read-only**<br/>
Changed the config file reader to open files in read-only mode to minimze multi-user/thread access issues while writing configuration in case other threads want to access the data. Thanks to  Patrick Wyatt.

* **Fix XmlConfig Encryption Error**<br/>
Fixed small bug that failed to decrypt encoded properties after writing out XmlFile properties to disk. Thanks to Patrick Wyatt.

* **Several minor bug fixes**<br/>
Fix issue configuration file issue in one of the AutoConfig file tests. Fix a small, config section reader issue.

* **Fixed documentation**<br/>
Accidentally left outdated V1 documentation on the detailed developer documentation site. Fixed.


###Version 2.10
*August 28th, 2013*

* **License Change - drop commercial License Requirement**<br/>
We've dropped the requirement for a commercial license for this tool and are using a pure MIT license, 
with an *optional* commercial license available for those that want official support, require a 
commercial license in their organization or simply want to support this project.

* **Documentation Updates**<br/>
The documentation both on GitHub and in the online/chm help files have been updated significantly to provide a host more examples.

* **Minor Bug fixes**<br/>
Fixed a number of small issues with the configuration file and Sql providers. See GitHub commit history for more info.


###Version 2.00
*January 14th, 2013*

* **Simplified Instantiation Management**<br/>
Changed the way the class initializes by explicitly requiring a call to Initialize() after instantiation. This removes any requirements to implement specific subclass methods on the configuration class making it much simpler to get started.

* **Easier Customization of default Configuration Provider**<br/>
If you need to customize the default configuration store for your configuration class you can now simply override the new <<i>>OnCreateDefaultProvider()<</i>> method. This method can create an instance of a provider and customize it as needed and simply return it. There's also a new OnInitialize() method which can take over the entire initial provider load and read process under your full control.

* **Improved Auto-Creation Logic**<br/>
Configuration stores are now automatically created if they don't exist yet - assuming the calling application has rights to write the file/store. On first read AppConfiguration checks if the store exists and if it doesn't tries to create it. If the store does exist, but keys are missing the keys are added and written to the store.

* **Added additional Unit Tests**<br/>
Added a host of new unit tests to demonstrate Configuration class implementations for each configuration provider type and usage examples. Updated existing examples to the new initialization syntax.
