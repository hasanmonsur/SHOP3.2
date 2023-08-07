using Microsoft.AspNetCore.Mvc.Rendering;
using NeSHOP.Models;
using NESHOP.Models;

namespace NESHOP.Contacts
{
    public interface IBllCommon
    {
        List<DaoRoleInfo> FuncReturnRoleList(DaoUserInfo objDaoUserInfo);
        List<DaoModuleInfo> FuncReturnModuleList(List<DaoRoleInfo> objDaoRoleList);
        string FunctionReturnFileLoc(string varAction, String ModuleCode);
        List<DaoModuleInfo> FuncReturnMenuList(List<DaoRoleInfo> objDaoRoleList, List<DaoModuleInfo> objDaoModuleList);
        string funcAmountToWord(decimal amount);
        SelectList FuncCounterCodeList(string CounterCode);
        SelectList FuncDelverMediaCodeList(string DelverMediaCode);
        SelectList FuncDesigcodeList(string Desigcode);
        SelectList FuncEmptypeList(string EmpType);
        SelectList FuncInvTranTypeList(string TranType);
        List<Product> FuncIteamOrdList(string OrdType);
        SelectList FuncIteamOrdStatusList(string OrdStatus);
        SelectList FuncIteamOrdTypeList(string OrdType);
        SelectList FuncPaymentTypeList(string PaymentType);
        TblAppinfoModel FuncReturnAppInformation(string Instcode);
        SelectList FuncReturnDeptList(string Deptcode);
        List<IteamModels> FuncReturnFaultIteamList(string Code);
        SelectList FuncReturnGenderList(string Gender);
        string FuncReturnInstCode(DaoUserInfo objDaoUserInfo);
        SelectList FuncReturnInstList(string code);
        SelectList FuncReturnIteamCatagoryList(string CatagoryCode);
        List<IteamModels> FuncReturnIteamList(string Code);
        SelectList FuncReturnIteamTypeList(string IteamType);
        SelectList FuncReturnIteamVendorList(string VenCode);
        SelectList FuncReturnProductList(string TaxCode);
        SelectList FuncReturnPTypeList(string Code);
        SelectList FuncReturnReligiouscodeList(string Code);
        SelectList FuncReturnReportTypeList(string Code);
        SelectList FuncReturnReportViewList(string Code);
        SelectList FuncReturnSelectVenList(string BrandCode);
        SelectList FuncReturnShistNoList(string Code);
        SelectList FuncReturnTaxCodeList(string TaxCode);
        SelectList FuncReturnVenList(string Code);
        SelectList FuncReturnVenTypeList(string VenType);
        SelectList FuncTranTypeList(string TranType);
        SelectList FuncVendorList(string VenCode);
        List<SelectListItem> GetCompanyList();
        SelectList GetCountryList(string CountryCode);
        SelectList GetDistrictList(string DistCode);
    }
}