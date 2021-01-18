using System;
using System.Collections.Generic;
using System.Text;
using Domains.Identity;
using Microsoft.Extensions.Options;
using NIIC.Application.ApplicationSettings;


namespace Application.Interfaces
{
    //anything related to security shouldn't included in api project or application project so we use infrastructure
    //and use interface to access it
    
    public interface IJwtGenerator
    {
        string CreateToken(AppUser user);
    }
}
