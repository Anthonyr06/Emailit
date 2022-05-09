using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emailit.Models
{
    /// <summary>
    /// Static class where the parameters for user data validation are configured.
    /// </summary>
    public static class UserDataValidation
    {
        /// <summary>
        /// Maximum number of characters the user name can have
        /// </summary>
        public const int MaxNameLenght = 80;

        /// <summary>
        /// Maximum number of characters the user lastname can have
        /// </summary>
        public const int MaxLastNameLenght = 80;

        /// <summary>
        /// Minimun number of characters the user email can have
        /// </summary>
        public const int MinEmailLenght = 6;
        /// <summary>
        /// Maximun number of characters the user email can have
        /// </summary>
        public const int MaxEmailLenght = 254;

        /// <summary>
        /// Minimun number of characters the user password can have
        /// </summary>
        public const int MinPasswordLenght = 8;
        /// <summary>
        /// Maximun number of characters the user password can have
        /// </summary>
        public const int MaxPasswordLenght = 32;

    }

    /// <summary>
    /// Static class where the parameters for role data validation are configured.
    /// </summary>
    public static class RoleDataValidation
    {
        /// <summary>
        /// Maximum number of characters the role name can have
        /// </summary>
        public const int MaxNameLenght = 80;

        /// <summary>
        /// Maximum number of characters the role description can have
        /// </summary>
        public const int MaxDescriptionLenght = 150;
    }

    /// <summary>
    /// Static class where the parameters for job data validation are configured.
    /// </summary>
    public static class JobDataValidation
    {
        /// <summary>
        /// Maximum number of characters the job name can have
        /// </summary>
        public const int MaxNameLenght = 80;

        /// <summary>
        /// Maximum number of characters the job description can have
        /// </summary>
        public const int MaxDescriptionLenght = 150;
    }

    /// <summary>
    /// Static class where the parameters for branch office data validation are configured.
    /// </summary>
    public static class BranchOfficeDataValidation
    {
        /// <summary>
        /// Maximum number of characters the BranchOffice name can have
        /// </summary>
        public const int MaxNameLenght = 80;

    }

    /// <summary>
    /// Static class where the parameters for department data validation are configured.
    /// </summary>
    public static class DepartmentDataValidation
    {
        /// <summary>
        /// Maximum number of characters the department name can have
        /// </summary>
        public const int MaxNameLenght = 80;

    }

    /// <summary>
    /// Static class where the parameters for message data validation are configured.
    /// </summary>
    public static class MessageDataValidation
    {
        /// <summary>
        /// Minimum number of characters needed to search for a user's email
        /// </summary>
        public const int MinLenghtSearchFieldOfUSersEmails = 3; //If this is modified, change in: sendMsg.js (magicSuggestParams:minChars)
        /// <summary>
        /// Maximum size allowed per file in MB
        /// </summary>
        public const int maxSizePerFileInMB = 20;  //If this is modified, change in: sendMsg.js
        /// <summary>
        /// Maximum number of attachments allowed in a message
        /// </summary>
        public const int maxFiles = 15;  //If this is modified, change in: sendMsg.js

    }
}
