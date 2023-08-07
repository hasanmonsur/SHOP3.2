using Microsoft.AspNetCore.Mvc.Rendering;
using NeSHOP.Models;
using NESHOP.Models;

namespace NESHOP.Contacts
{
    public interface IBllUserInfo
    {
        string FuncReturnMaxRoleCode();
        bool FunLoginPasswordChange(ChangePasswordModel viewModel, DaoUserInfo objDaoUserInfo);
        SelectList FunReturnRoleList(string Code);
        bool FunSaveRoleInfo(TblRoleModel viewModel, DaoUserInfo objDaoUserInfo);
        bool FunSaveUserInfo(RegisterModel viewModel, DaoUserInfo objDaoUserInfo);
        TblRoleModel FunSearchRoleInfo(string RoleCode);
        RegisterModel FunSearchUserInfo(string UserId);
        bool FunUpdateRoleInfo(TblRoleModel viewModel);
        bool FunUpdateUserInfo(RegisterModel viewModel, DaoUserInfo objDaoUserInfo);
        DaoUserInfo FunValidateUser(DaoUserInfo objDaoUserInfo);
    }
}