using Microsoft.AspNetCore.Mvc.Rendering;
using NeSHOP.Models;
using NESHOP.Models;
using System.Collections.ObjectModel;

namespace NESHOP.Contacts
{
    public interface IBllAdmin
    {
        SelectList FuncDeptList(string DeptId);
        DepartmentsModel FuncDeptSearchData(string DeptId);
        TblDesigModel FuncDesigSearchData(string DesigCode);
        DivisionModel FuncDivSearchData(string DivId);
        SelectList FuncDivsionList(string DivId);
        bool FuncEntryEmpInfo(UserInfoModel objDaoUserInfo);
        SelectList FuncReturnCatagoryList(string Code);
        List<DepartmentsModel> FuncReturnDeptList(string DeptId);
        SelectList FuncReturnDesigDataList(string Code);
        SelectList FuncReturnDesigTypeList(string Code);
        SelectList FuncReturnEmpList(string EmpId);
        Collection<TblFormsModel> FuncReturnFormList(string roleCode);
        TblAppinfoModel FuncReturnInstInformation(string Instcode);
        string FuncReturnLastDeptId();
        string FuncReturnLastDivId();
        string FuncReturnMaxEmpId();
        SelectList FuncReturnUserList(string userid);
        bool FuncReturnValidDept(string DeptId);
        bool FuncReturnValidDesigCode(string DesigCode);
        bool FuncReturnValidDiv(string DivId);
        bool FuncReturnValidLicance(string Licance);
        bool FuncReturnValidPermission(string Userid, string Rolecode);
        bool FuncReturnValidRoleConfig(string Rolecode, string FormCode);
        List<TblRoleModel> FuncRoleModel();
        Collection<TblRoleModel> FuncRoleModelArray(string UserId);
        SelectList FuncRoleModelList(string RoleCode);
        bool FuncSaveDayEndInfo(DayEndModel viewModel, DaoUserInfo objDaoUserInfo);
        bool FuncSaveDeptInfo(DepartmentsModel viewModel);
        bool FuncSaveDesigInfo(TblDesigModel viewModel);
        bool FuncSaveDivInfo(DivisionModel viewModel);
        bool FuncSaveInstInformation(TblAppinfoModel viewModel, DaoUserInfo objDaoUserInfo);
        bool FuncSaveLicanceInfo(LicanceModel viewModel);
        bool FuncSaveRoleConfig(TblFormsModel tblFormModel, string Rolecode);
        bool FuncSaveRoleConfig(TblRoleconfigModel viewModel);
        bool FuncSaveUserPermission(TblRoleModel tblRoleModel, string Userid);
        bool FuncSaveUserPermission(TblUserprevModel viewModel);
        TblAppinfoModel FuncSearchInstInformation(string Instcode, DaoUserInfo objDaoUserInfo);
        bool FuncUpdateInstInformation(TblAppinfoModel viewModel, DaoUserInfo objDaoUserInfo);
        int FuncValidLicanceAuthentication();
        UserInfoModel FunSearchEmpInfo(string EmpCode);
        bool FunUpdateEmpInfo(UserInfoModel viewModel);
    }
}