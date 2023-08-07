using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace NeSHOP.Models
{

    #region change password

    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Current password")]
        public string OldPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("New password")]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Confirm new password")]
        public string ConfirmPassword { get; set; }

        public string Btn { get; set; }
    }

    #endregion
    //-----------------------////------------------------------------
    #region Logon Model
    public class LogOnModel
    {
        [Required]
        [DisplayName("User name")]
        public string UserId { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Password")]
        public string Password { get; set; }
    }

    #endregion
    //--------------------------/////-----------------------------
    #region Registration

    public class RegisterModel
    {
        [DisplayName("User ID")]
        public string UserId { get; set; }

        [DisplayName("Password")]
        public string UserPassword { get; set; }

        [DisplayName("Confirm password")]
        public string ConfirmPassword { get; set; }

        [DisplayName("Emp ID")]
        public string EmpId { get; set; }

        [DisplayName("Is User Active")]
        public bool IsActive { get; set; }

        public SelectList EmpIdList { get; set; }

        public string Btn { get; set; }
    }

    #endregion
    //------------------------////-----------------------------------------------

    #region User Info  info
    public class UserInfoModel
    {
        [Required(ErrorMessage = "EmpId Required")]
        [StringLength(10, ErrorMessage = "Must be less than 10 characters")]
        [DisplayName("Emp. ID ")]
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
        [DisplayName("Contact #")]
        [StringLength(50, ErrorMessage = "Must be less than 200 characters")]
        public string EmpPhone { get; set; }

        [Required(ErrorMessage = "Email Required")]
        [RegularExpression("^([0-9a-zA-Z]([-.\\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\\w]*[0-9a-zA-Z]\\.)+[a-zA-Z]{2,9})$", ErrorMessage = "Not a valid email")]
        [DisplayName("E-mail")]
        public string EmpEmail { get; set; }

        [DisplayName("Ras. Address ")]
        [StringLength(400, ErrorMessage = "Must be less than 400 characters")]
        public string EmpAddress { get; set; }

        [DisplayName("Remarks ")]
        [StringLength(400, ErrorMessage = "Must be less than 400 characters")]
        public string EmpComments { get; set; }

        public string UserId { get; set; }

        public DateTime EntryDate { get; set; }

        public bool IsActive { get; set; }


        public string Btn { get; set; }

        public string InstCode { get; set; }

        public SelectList EmpList { get; set; }
    }
    #endregion

    #region Department
    //------------------------//---------------------------------
    public class DepartmentsModel
    {
        [DisplayName("ID")]
        public String Deptid { get; set; }

        [DisplayName("Name")]
        public string Deptname { get; set; }
        public SelectList DeptList { get; set; }

        [DisplayName("Division")]
        public String Supdeptid { get; set; }

        public List<DepartmentsModel> DepartmentsModelList { set; get; }
        public DepartmentsModel()
        {
            DepartmentsModelList = new List<DepartmentsModel>();
        }

        public string DeptSearchKey { get; set; }
        public SelectList DivList { get; set; }

        public string Btn { get; set; }
    }
    #endregion
    //---------------------------------/////-------------------------------

    #region Division info
    //------------------------//---------------------------------
    public class DivisionModel
    {
        [DisplayName("ID")]
        public String Divid { get; set; }

        [DisplayName("Name")]
        public string Divname { get; set; }

        public string DivSearchKey { get; set; }
        public SelectList DivDataList { get; set; }

        public string Btn { get; set; }
    }
    #endregion

    #region Designation Model
    public class TblDesigModel
    {
        [DisplayName("Code")]
        public string Desigcode { get; set; }

        [DisplayName("Name")]
        public string Desigdesc { get; set; }

        [DisplayName("Type")]
        public string Desigtype { get; set; }

        public SelectList DesigtypeList { get; set; }
        public SelectList DesigDataList { get; set; }

        public string DesigSearchKey { get; set; }
        public string Btn { get; set; }




    }
    #endregion

    #region Role Model
    public class TblRoleModel
    {
        [DisplayName("Code")]
        public string Rolecode { get; set; }

        [DisplayName("Desc")]
        public string Roledesc { get; set; }

        [DisplayName("Is Active")]
        public bool IsActive { get; set; }

        public SelectList RolecodeList { get; set; }

        public string Btn { get; set; }
    }
    #endregion

    #region RoleConfig Model
    public class TblRoleconfigModel
    {
        [DisplayName("Role")]
        public string Rolecode { get; set; }
        public SelectList RolecodeList { get; set; }

        [DisplayName("Form")]
        public string Formcode { get; set; }

        [DisplayName("Isactive")]
        public bool? Isactive { get; set; }

        public Collection<TblFormsModel> FormsModelList { get; set; }

        public string Btn { get; set; }
    }
    #endregion

    #region Form Model
    public class TblFormsModel
    {
        [DisplayName("Formcode")]
        public string Formcode { get; set; }

        [DisplayName("Formname")]
        public string Formname { get; set; }

        [DisplayName("Actionlink")]
        public string Actionlink { get; set; }

        [DisplayName("Formloc")]
        public string Formloc { get; set; }


        public bool IsActive { get; set; }
    }
    #endregion

    #region App Inst Info
    public class TblAppinfoModel
    {

        [DisplayName("Institute")]
        public string Instcode { get; set; }

        [DisplayName("Name")]
        public string Instname { get; set; }

        [DisplayName("Address")]
        public string Instaddress { get; set; }

        [DisplayName("E-mail")]
        public string Instemail { get; set; }

        [DisplayName("Fax")]
        public string Instfax { get; set; }

        [DisplayName("Phone")]
        public string Intphone { get; set; }

        [DisplayName("App. Version")]
        public string Version { get; set; }

        [DisplayName("Web Address")]
        public string Website { get; set; }

        [DisplayName("Inst. Catagory")]
        public string Instcatagorycode { get; set; }

        [DisplayName("Ref. Code")]
        public string RefCode { get; set; }

        public string ShiftNo { get; set; }

        //--------------------------------------



        public SelectList InstcatagorycodeList { get; set; }

        public SelectList ShiftNoList { get; set; }

        public SelectList InstCodeList { get; set; }

        public string Btn { get; set; }

        public string DateFrom { get; set; }

        public string DateTo { get; set; }

        
    }
    #endregion

    #region User Prev
    public class TblUserprevModel
    {
        public Collection<TblRoleModel> RoleModelList { get; set; }

        [DisplayName("Role")]
        public string Rolecode { get; set; }

        [DisplayName("User ID")]
        public string Userid { get; set; }

        [DisplayName("Isactive")]
        public bool? Isactive { get; set; }


        public string Btn { get; set; }

        public SelectList UserList { get; set; }
    }

    #endregion

    #region Day End Model
    public class DayEndModel
    {
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Current Business Date")]
        public DateTime CurrentBusinessDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("New Business Date")]
        public DateTime NewBusinessDate { get; set; }

        public string InstCode { get; set; }
        public string SysStatus { get; set; }
        public DateTime EntryDate { get; set; }
        public string EntryBy { get; set; }

        public string Btn { get; set; }
    }

    #endregion
    

    #region Database Backup
    public class DatabaseBackModel
    {
        
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Business Date")]
        public DateTime BackupDate { get; set; }

        public string Btn { get; set; }

        public object MsgCode { get; set; }
    }
    #endregion

    #region Customer info
    public class CustomerModels
    {

        [DisplayName("Customer Code")]
        public string CustomerCode { get; set; }

        [DisplayName("Comapny Name")]
        public string CompanyName { get; set; }

        [DisplayName("Contact Person")]
        public string ContactPerson { get; set; }

        [DisplayName("Company Address")]
        public string CompanyAddress { get; set; }

        [DisplayName("Tele Phone #")]
        public string ContactTPhone { get; set; }

        [DisplayName("Contact E-mail")]
        public string ContactCPhone { get; set; }

        [DisplayName("Cell phone #")]
        public string ContactEmail { get; set; }

        //---------------Button----------------------
        public string Btn { get; set; }

        public String MsgCode { get; set; }

        public String SearchKey { get; set; }

        public SelectList CustomerList { get; set; }

    }
    #endregion

    #region Add Licance  Model
    public class LicanceModel
    {
        [DisplayName("Entry Licance Code")]
        public string Licance { get; set; }

        public string Btn { get; set; }
    }

    #endregion


    #region Supplier info
    public class SupplierModels
    {

        [DisplayName("Supplier Code")]
        public string SupplierCode { get; set; }

        [DisplayName("Comapny Name")]
        public string CompanyName { get; set; }

        [DisplayName("Contact Person")]
        public string ContactPerson { get; set; }

        [DisplayName("Company Address")]
        public string CompanyAddress { get; set; }

        [DisplayName("Tele Phone #")]
        public string ContactTPhone { get; set; }

        [DisplayName("Contact E-mail")]
        public string ContactCPhone { get; set; }

        [DisplayName("Cell phone #")]
        public string ContactEmail { get; set; }

        //---------------Button----------------------
        public string Btn { get; set; }

        public String MsgCode { get; set; }

        public String SearchKey { get; set; }

        public SelectList CustomerList { get; set; }

    }
    #endregion
}
