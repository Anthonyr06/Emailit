using Emailit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emailit.Services.Data
{
    public interface IUserRoleRepository
    {
        Task DeleteAsync(UserRole userRole);
    }
}
