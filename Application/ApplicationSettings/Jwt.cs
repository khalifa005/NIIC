using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NIIC.Application.ApplicationSettings
{
    public class Jwt
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public double DurationInMinutes { get; set; }

    }
}
