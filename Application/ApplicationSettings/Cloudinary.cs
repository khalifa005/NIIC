using System;
using System.Collections.Generic;
using System.Text;

namespace Application.ApplicationSettings
{
    public class CloudinarySetting
    {
        //convention based need to match the json settings
        //cloud name 
        public string Name { get; set; }
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
    }
}
