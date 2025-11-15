using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace NeSHOP.Models
{

    #region Models
    
    //public class ChangePasswordModel
    //{
    //    [Required]
    //    [DataType(DataType.Password)]
    //    [DisplayName("Current password")]
    //    public string OldPassword { get; set; }

    //    [Required]
    //    [DataType(DataType.Password)]
    //    [DisplayName("New password")]
    //    public string NewPassword { get; set; }

    //    [Required]
    //    [DataType(DataType.Password)]
    //    [DisplayName("Confirm new password")]
    //    public string ConfirmPassword { get; set; }
    //}

    public class LogOnModel
    {
        [Required]
        [DisplayName("User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Password")]
        public string Password { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [DisplayName("User ID")]
        public string UserId { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Password")]
        public string UserPassword { get; set; }

        [Required(ErrorMessage = "EmpId Required")]
        [DataType(DataType.Password)]
        [DisplayName("Confirm password")]
        public string ConfirmPassword { get; set; }

        [Required]
        [DisplayName("Emp ID")]
        public string EmpId { get; set; }

        [DisplayName("Is User Active")]
        public bool IsActive { get; set; }
      }

    //[PropertiesMustMatch("Password", "ConfirmPassword", ErrorMessage = "The password and confirmation password do not match.")]
    public class UserInfoModel
    {
        [Required(ErrorMessage = "EmpId Required")]
        [StringLength(10, ErrorMessage = "Must be less than 10 characters")]
        public string EmpId { get; set; }

        [Required(ErrorMessage = "NickName Required")]
        [DisplayName("Nick Name ")]
        [StringLength(50, ErrorMessage = "Must be less than 50 characters")]
        public string NickName { get; set; }
        
        [Required(ErrorMessage = "FullName Required")]
        [DisplayName("Full Name ")]
        [StringLength(50, ErrorMessage = "Must be less than 200 characters")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Phone Required")]
        [DisplayName("Phone Number ")]
        [StringLength(50, ErrorMessage = "Must be less than 200 characters")]
        public string EmpPhone { get; set; }

        [Required(ErrorMessage = "Email Required")]
        [RegularExpression("^([0-9a-zA-Z]([-.\\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\\w]*[0-9a-zA-Z]\\.)+[a-zA-Z]{2,9})$", ErrorMessage = "Not a valid email")]
        public string EmpEmail { get; set; }

        [DisplayName("Address ")]
        [StringLength(400, ErrorMessage = "Must be less than 400 characters")]
        public string EmpAddress { get; set; }

        [DisplayName("Comment ")]
        [StringLength(400, ErrorMessage = "Must be less than 400 characters")]
        public string EmpComments { get; set; }

        public string UserId { get; set; }

        public decimal EntryDate { get; set; }

        public bool IsActive { get; set; }
        
      }
    #endregion

    
}
