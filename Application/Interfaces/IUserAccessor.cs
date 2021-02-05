using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IUserAccessor
    {
        string GetCurrentLogedinUsername();
        string GetCurrentLogedinUserId();
    }
}
