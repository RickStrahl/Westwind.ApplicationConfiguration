using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Westwind.Utilities.Configuration.Tests
{
    public class TestHelpers
    {
        public static string GetTestConfigFilePath()
        {
            return (typeof(TestHelpers).Assembly.CodeBase + ".config")
                   .Replace("file:///", "");
        }

        public static void xDeleteTestConfigFile()
        {
            string configFile = GetTestConfigFilePath();
            File.Delete(configFile);                   
        }

    }
}
