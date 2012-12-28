using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using System.Collections.Specialized;
using System.IO;

namespace Westwind.Utilities.Configuration.Tests
{
    [TestClass]
    public class MiscellaneousConfigurationTests
    {
 

        [TestMethod]
        public void WriteAppSettingsTest()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var section = config.GetSection("appSettings") as AppSettingsSection;            
            section.Settings.Add("NewKey", "Value");
            config.Save();

            // Config File and custom section should exist
            string text = File.ReadAllText(TestHelpers.GetTestConfigFilePath());
            Assert.IsTrue(text.Contains("key=\"NewKey\""));
        }   
    }
}
