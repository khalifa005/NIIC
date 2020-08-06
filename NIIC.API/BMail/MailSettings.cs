using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NIIC.API.BMail
{
    public class MailSettings
    {
        public string Mail { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }
}
