using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using System.Collections.Specialized;

namespace Westwind.Utilities.Configuration.Tests
{
    [TestClass]
    public class MiscellaneousConfigurationTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var settings = ConfigurationManager.GetSection("CustomConfiguration") as NameValueCollection;            
            Console.WriteLine(settings["ApplicationName"]);

            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //object customconfig = config.GetSection("CustomConfiguration") as 
            //Console.WriteLine(customconfig.ToString());
            //customconfig.Add("NewKey", "Value");
            //config.Save();
        }

        [TestMethod]
        public void WriteAppSettingsTest()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var section = config.GetSection("appSettings") as AppSettingsSection;            
            section.Settings.Add("NewKey", "Value");
            config.Save();
        }

        [TestMethod]
        public void ReadStronglyTypedSectionTest()
        {
            var section = ConfigurationManager.GetSection("MyCustomConfiguration") as MyCustomConfigurationSection;

            Console.WriteLine(section.ApplicationName);
            Console.WriteLine(section.MaxDisplayItems);
            Console.WriteLine(section.DebugMode);

        }
    }
}
