#region License
/*
 **************************************************************
 *  Author: Rick Strahl 
 *          © West Wind Technologies, 2008 - 2011
 *          http://www.west-wind.com/
 * 
 * Created: Date
 *
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 * 
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 **************************************************************  
*/
#endregion

using System;
using System.Web;
using System.Reflection;
using System.Web.UI;
using System.Collections.Generic;

namespace Westwind.Utilities
{
    /// <summary>
    /// Summary description for wwWebUtils.
    /// </summary>
    public static class WebUtils
    {

        /// <summary>
        /// Routine that can be used read Form Variables into an object if the 
        /// object name and form variable names match either exactly or with a specific
        /// prefix.
        /// 
        /// The method loops through all *public* members of the object and tries to 
        /// find a matching form variable by the same name in Request.Form.
        /// 
        /// The routine returns false if any value failed to parse (ie. invalid
        /// formatting etc.). However parsing is not aborted on errors so all
        /// other convertable values are set on the object.
        /// 
        /// You can pass in a Dictionary<string,string> for the Errors parameter
        /// to optionally retrieve unbinding errors. The dictionary key holds the
        /// simple form varname for the field (ie. txtName), the value the actual
        /// exception error message
        /// </summary>
        /// <remarks>
        /// This method can have unexpected side-effects if multiple naming
        /// containers share common variable names. This routine is not recommended
        /// for those types of pages.
        /// </remarks>
        /// <param name="Target"></param>
        /// <param name="FormVarPrefix">empty or one or more prefixes spearated by |</param>
        /// <param name="errors">Allows passing in a string dictionary that receives error messages. returns key as field name, value as error message</param>
        /// <returns>true or false if an unbinding error occurs</returns>
        public static bool FormVarsToObject(object target, string formvarPrefixes = null, Dictionary<string, string> errors = null)
        {
            bool isError = false;
            List<string> ErrorList = new List<string>();

            if (formvarPrefixes == null)
                formvarPrefixes = "";

            if (HttpContext.Current == null)
                throw new InvalidOperationException("FormVarsToObject can only be called from a Web Request");

            HttpRequest Request = HttpContext.Current.Request;

            // try to get a generic reference to a page for recursive find control
            // This value will be null if not dealing with a page (ie. in JSON Web Service)
            Page page = HttpContext.Current.CurrentHandler as Page;

            MemberInfo[] miT = target.GetType().FindMembers(
                MemberTypes.Field | MemberTypes.Property,
                BindingFlags.Public | BindingFlags.Instance,
                null, null);

            // Look through all prefixes separated by |
            string[] prefixes = formvarPrefixes.Split('|');

            foreach (string prefix in prefixes)
            {

                // Loop through all members of the Object
                foreach (MemberInfo Field in miT)
                {
                    string Name = Field.Name;

                    FieldInfo fi = null;
                    PropertyInfo pi = null;
                    Type FieldType = null;

                    if (Field.MemberType == MemberTypes.Field)
                    {
                        fi = (FieldInfo)Field;
                        FieldType = fi.FieldType;
                    }
                    else
                    {
                        pi = (PropertyInfo)Field;
                        FieldType = pi.PropertyType;
                    }

                    // Lookup key will be field plus the prefix
                    string formvarKey = prefix + Name;

                    // Try a simple lookup at the root first
                    var strValue = Request.Form[formvarKey];

                    // if not found try to find the control and then
                    // use its UniqueID for lookup instead
                    if (strValue == null && page != null)
                    {
                        Control ctl = WebUtils.FindControlRecursive(page, formvarKey);
                        if (ctl != null)
                            strValue = Request.Form[ctl.UniqueID];
                    }

                    // Bool values and checkboxes might require special handling
                    if (strValue == null)
                    {
                        // Must handle checkboxes/radios
                        if (FieldType == typeof(bool))
                            strValue = "false";
                        // other values that are null are not updated
                        else
                            continue;
                    }

                    try
                    {
                        // Convert the value to it target type
                        object Value = ReflectionUtils.StringToTypedValue(strValue, FieldType);

                        // Assign it to the object property/field
                        if (Field.MemberType == MemberTypes.Field)
                            fi.SetValue(target, Value);
                        else
                            pi.SetValue(target, Value, null);
                    }
                    catch (Exception ex)
                    {
                        isError = true;
                        if (errors != null)
                            errors.Add(Field.Name, ex.Message);
                    }
                }
            }

            return !isError;
        }

        /// <summary>
        /// Finds a Control recursively. Note finds the first match and exits
        /// </summary>
        /// <param name="ContainerCtl">The top level container to start searching from</param>
        /// <param name="IdToFind">The ID of the control to find</param>
        /// <param name="alwaysUseFindControl">If true uses FindControl to check for hte primary Id which is slower, but finds dynamically generated control ids inside of INamingContainers</param>
        /// <returns></returns>
        public static Control FindControlRecursive(Control Root, string id, bool alwaysUseFindControl = false)
        {
            if (alwaysUseFindControl)
            {
                Control ctl = Root.FindControl(id);
                if (ctl != null)
                    return ctl;
            }
            else
            {
                if (Root.ID == id)
                    return Root;
            }

            foreach (Control Ctl in Root.Controls)
            {
                Control foundCtl = FindControlRecursive(Ctl, id, alwaysUseFindControl);
                if (foundCtl != null)
                    return foundCtl;
            }

            return null;
        }
    }
}
