using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Westwind.Utilities.Configuration
{
class MyCustomConfigurationSection : ConfigurationSection
{        
    [ConfigurationProperty("ApplicationName")]
    public string ApplicationName
    {
        get { return (string) this["ApplicationName"]; }
        set { this["ApplicationName"] = value; 
        
        
        }
    }

    [ConfigurationProperty("MaxDisplayItems",DefaultValue=15)]
    public int MaxDisplayItems
    {
        get { return (int) this["MaxDisplayItems"]; }
        set { this["MaxDisplayItems"] = value; }
    }

    [ConfigurationProperty("DebugMode")]
    public DebugModes DebugMode
    {
        get { return (DebugModes) this["DebugMode"]; }
        set { this["DebugMode"] = value; }
    }                
}
}
