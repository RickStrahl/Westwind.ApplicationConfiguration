using System.IO;
using System.Reflection;
using System.Text;
using System.Web.UI;
using ApplicationConfigurationWeb;
using Westwind.Utilities;

namespace ApplicationConfigurationMvc.Controllers
{
    public class SamplesViewModel
    {
        public ApplicationConfiguration Configuration { get; set; }
        public string ConfigTypeSelection { get; set; }
        public SamplesController Controller { get; set; }
        public string ErrorMessage { get; set; }
        public string InfoMessage { get; set; }

        public SamplesViewModel()
        {
            Configuration = App.Configuration;
            ConfigTypeSelection = "Default Web.Config";
        }

        public string ShowPropertyGrid()
        {
            return ShowPropertyGrid(Configuration);
        }

        /// <summary>
        /// Creates a simplistic Property Grid to display of an object
        /// </summary>
        /// <returns></returns>
        public static string ShowPropertyGrid(object sourceObject)
        {
            if (sourceObject == null)
                return "<hr/>No object passed.<hr/>";

            StringBuilder sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            {
                using (HtmlTextWriter hWriter = new HtmlTextWriter(sw))
                {
                    hWriter.WriteBeginTag("table");
                    hWriter.WriteAttribute("border", "1");
                    hWriter.WriteAttribute("cellpadding", "5");
                    hWriter.WriteAttribute("class", "table table-bordered table-striped");
                    hWriter.Write(" style='border-collapse:collapse;'");
                    hWriter.Write(HtmlTextWriter.TagRightChar);
                    MemberInfo[] miT = sourceObject.GetType().FindMembers(MemberTypes.Field | MemberTypes.Property, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, null, null);
                    foreach (MemberInfo Field in miT)
                    {
                        string Name = Field.Name;
                        object Value = null;
                        if (Field.MemberType == MemberTypes.Field)
                        {
                            Value = ((FieldInfo)Field).GetValue(sourceObject);
                            continue;
                        }
                        else
                            if (Field.MemberType == MemberTypes.Property)
                                Value = ((PropertyInfo)Field).GetValue(sourceObject, null);
                        hWriter.WriteFullBeginTag("tr");
                        hWriter.WriteFullBeginTag("td");
                        hWriter.Write(Name);
                        hWriter.WriteEndTag("td");
                        hWriter.WriteLine();
                        hWriter.WriteFullBeginTag("td");
                        hWriter.WriteBeginTag("input");
                        hWriter.WriteAttribute("name", "Configuration." + Name);
                        hWriter.WriteAttribute("value", ReflectionUtils.TypedValueToString(Value));
                        hWriter.Write(" style='Width:400px' ");
                        hWriter.Write(HtmlTextWriter.TagRightChar);
                        hWriter.WriteEndTag("td");
                        hWriter.WriteLine();
                        hWriter.WriteEndTag("tr");
                        hWriter.WriteLine();
                    }
                    hWriter.WriteEndTag("table");
                    //string TableResult = sb.ToString();
                    hWriter.Close();
                }
                sw.Close();
            }

            return sb.ToString();
        }

        public void ShowError(string errorMessage)
        {
            this.ErrorMessage = errorMessage;
        }

        public void ShowMessage(string message)
        {
            this.InfoMessage = message;
        }

    }
}
