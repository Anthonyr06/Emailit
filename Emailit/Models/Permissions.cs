using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Emailit.Models
{
    [Flags]
    public enum Permissions : ulong //max 64 ermissions
    {
        [Display(Name = "Can view and list roles")]
        ReadRoles = 1 << 0,
        [Display(Name = "Can create and modify roles")]
        WriteRoles = 1 << 1,
        [Display(Name = "Can deactivate and reactivate roles")]
        DeactivateRoles = 1 << 2,

        [Display(Name = "Can view and list users")]
        ReadUsers = 1 << 3,
        [Display(Name = "Can create users")]
        CreateUsers = 1 << 4,
        [Display(Name = "Can modify users")]
        UpdateUsers = 1 << 5,
        [Display(Name = "Can deactivate and reactivate users")]
        DeactivateUsers = 1 << 6,

        [Display(Name = "Can add and modify users roles")]
        WriteUsersRoles = 1 << 7,
        [Display(Name = "Can add and remove users permissions")]
        WriteUsersPermissions = 1 << 8,

        [Display(Name = "Can view and list jobs")]
        ReadJobs = 1 << 9,
        [Display(Name = "Can create and modify jobs")]
        WriteJobs = 1 << 10,
        [Display(Name = "Can deactivate and reactivate roles")]
        DeactivateJobs = 1 << 11,

        [Display(Name = "Can view and list offices")]
        ReadBranchOffices = 1 << 12,
        [Display(Name = "Can create and modify offices")]
        WriteBranchOffices = 1 << 13,
        [Display(Name = "Can deactivate and reactivate offices")]
        DeactivateBranchOffices = 1 << 14,

        [Display(Name = "Can view and list departments")]
        ReadDepartments = 1 << 15,
        [Display(Name = "Can create and modify departments")]
        WriteDepartments = 1 << 16,
        [Display(Name = "Can deactivate and reactivate departments")]
        DeactivateDepartments = 1 << 17,
    }
}
