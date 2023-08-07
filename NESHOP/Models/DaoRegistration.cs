using System;

namespace NESHOP.Models
{
    public class DaoRegistration
    {
        public string EmpId { get; set; }

        public string UserId { get; set; }

        public string UserPassword { get; set; }

        public DateTime EntryDate { get; set; }

        public bool IsActive { get; set; }

        public string OldUserPassword { get; set; }

        public string NewUserPassword { get; set; }

        public string ConfirmNewUserPassword { get; set; }
    }
}
