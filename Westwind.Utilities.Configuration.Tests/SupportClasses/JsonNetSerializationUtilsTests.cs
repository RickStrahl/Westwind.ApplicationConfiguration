using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Westwind.Utilities.Configuration.Tests.SupportClasses
{
    [TestClass]
    public class JsonNetSerializationUtilsTests
    {
        [TestMethod]
        public void JsonStringSerializeTest()
        {                        
            var config = new AutoConfigFileConfiguration();
            
            string json = JsonSerializationUtils.Serialize(config,true,true);

            Console.WriteLine(json);
            Assert.IsNotNull(json);

        }

        [TestMethod]
        public void JsonSerializeToFile()
        {
            var config = new AutoConfigFileConfiguration();
            
            bool result = JsonSerializationUtils.SerializeToFile(config,"serialized.config",true,true);
            string filetext = File.ReadAllText("serialized.config");
            Console.WriteLine(filetext);                                    
        }


        [TestMethod]
        public void JsonDeserializeStringTest()
        {
            var config = new AutoConfigFileConfiguration();
            config.ApplicationName = "New App";
            config.DebugMode = DebugModes.DeveloperErrorMessage;
            string json = JsonSerializationUtils.Serialize(config, true, true);

            config = null;

            config = JsonSerializationUtils.Deserialize(json, typeof (AutoConfigFileConfiguration),true) as AutoConfigFileConfiguration;

            Assert.IsNotNull(config);
            Assert.IsTrue(config.ApplicationName == "New App");
            Assert.IsTrue(config.DebugMode == DebugModes.DeveloperErrorMessage);

        }

        [TestMethod]
        public void DeserializeFromFileTest()
        {
            string fname = "serialized.config";

            var config = new AutoConfigFileConfiguration();            
            config.ApplicationName = "New App";
            config.DebugMode = DebugModes.DeveloperErrorMessage;
            bool result = JsonSerializationUtils.SerializeToFile(config,fname,true,true);

            Assert.IsTrue(result);

            config = null;

            config = JsonSerializationUtils.DeserializeFromFile(fname, typeof (AutoConfigFileConfiguration)) as
                AutoConfigFileConfiguration;

            Assert.IsNotNull(config);
            Assert.IsTrue(config.ApplicationName == "New App");
            Assert.IsTrue(config.DebugMode == DebugModes.DeveloperErrorMessage);
        }

        [TestMethod]
        public void JsonNativeInstantiation()
        {
            // Try to create instance
            var ser = new JsonSerializer();
            
            //ser.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            //ser.ObjectCreationHandling = ObjectCreationHandling.Auto;
            //ser.MissingMemberHandling = MissingMemberHandling.Ignore;
            ser.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            ser.Converters.Add(new  StringEnumConverter());
            
            var config = new AutoConfigFileConfiguration();
            config.ApplicationName = "New App";
            config.DebugMode = DebugModes.DeveloperErrorMessage;

            var writer = new StringWriter();
            var jtw = new JsonTextWriter(writer);
            jtw.Formatting = Formatting.Indented;

            ser.Serialize(jtw, config);

            string result = writer.ToString();
            jtw.Close();            

            Console.WriteLine(result);


            dynamic json = ReflectionUtils.CreateInstanceFromString("Newtonsoft.Json.JsonSerializer");
            dynamic enumConverter = ReflectionUtils.CreateInstanceFromString("Newtonsoft.Json.Converters.StringEnumConverter");
            json.Converters.Add(enumConverter);

            writer = new StringWriter();
            jtw = new JsonTextWriter(writer);
            jtw.Formatting = Formatting.Indented;

            json.Serialize(jtw, config);

            result = writer.ToString();
            jtw.Close();

            Console.WriteLine(result);

        }

    }
}
