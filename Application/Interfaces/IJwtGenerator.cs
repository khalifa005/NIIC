using System;
using System.Collections.Generic;
using System.Text;
using Domains.Identity;

namespace Application.Interfaces
{
    public interface IJwtGenerator
    {
        string CreateToken(AppUser user);
    }
}
