using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApplicationConfigurationWeb
{
    /// <summary>
    /// Summary description for MyModel
    /// </summary>
    public class MyModel
    {
        public MyModel()
        {
            Name = "Rick";
            Age = 20;
            Entered = DateTime.Now.Date;

        }

        public string Name { get; set; }
        public int Age { get; set; }
        public DateTime Entered { get; set; }
    }
}