using Microsoft.AspNetCore.Mvc.Rendering;
using NeSHOP.Models;
using NESHOP.Models;

namespace NESHOP.Contacts
{
    public interface IBllSettings
    {
        bool FuncCatagoryDelete(CatagoryModels viewModel);
        bool FuncCatagoryEntry(CatagoryModels viewModel, DaoUserInfo objDaoUserInfo);
        bool FuncCatagoryUpdate(CatagoryModels viewModel, DaoUserInfo objDaoUserInfo);
        bool FuncCustomerDelete(CustomerModels viewModel);
        bool FuncCustomerEntry(CustomerModels viewModel, DaoUserInfo objDaoUserInfo);
        bool FuncCustomerUpdate(CustomerModels viewModel, DaoUserInfo objDaoUserInfo);
        bool FuncModelDelete(IteamModelModels viewModel);
        bool FuncModelEntry(IteamModelModels viewModel, DaoUserInfo objDaoUserInfo);
        bool FuncModelUpdate(IteamModelModels viewModel, DaoUserInfo objDaoUserInfo);
        bool FuncProductDelete(ProductModels viewModel);
        bool FuncProductEntry(ProductModels viewModel, DaoUserInfo objDaoUserInfo);
        bool FuncProductUpdate(ProductModels viewModel, DaoUserInfo objDaoUserInfo);
        SelectList FuncReturnBankList(string Code);
        string FuncReturnCatagoryCode();
        CatagoryModels FuncReturnCatagoryInfo(string Code);
        SelectList FuncReturnCatagoryList(string Code);
        string FuncReturnCustomerCode();
        CustomerModels FuncReturnCustomerInfo(string Code);
        SelectList FuncReturnCustomerList(string Code);
        string FuncReturnModelCode(string BrandCode);
        IteamModelModels FuncReturnModelInfo(string Code);
        SelectList FuncReturnModelList(string Code);
        SelectList FuncReturnPartyList(string PType);
        string FuncReturnProductCode(string catagoryCode);
        ProductModels FuncReturnProductInfo(string Code);
        SelectList FuncReturnProductList(string Code);
        List<IteamModels> FuncReturnReplaceIteamList();
        List<IteamModels> FuncReturnSearchFultStockReport(IteamModels viewModel);
        List<IteamModels> FuncReturnSearchIteamList(IteamModels viewModel);
        List<IteamModels> FuncReturnSearchStockList(IteamModels viewModel);
        List<IteamModels> FuncReturnSearchStockReport(IteamModels viewModel);
        SelectList FuncReturnSelectModelList(string Code);
        SelectList FuncReturnSelectProductList(string Code);
        string FuncReturnSupplierCode();
        SupplierModels FuncReturnSupplierInfo(string Code);
        SelectList FuncReturnSupplierList(string Code);
        string FuncReturnTarmsCode();
        TarmsModels FuncReturnTarmsInfo(string Code);
        SelectList FuncReturnTarmsList(string Code);
        bool FuncSupplierDelete(SupplierModels viewModel);
        bool FuncSupplierEntry(SupplierModels viewModel, DaoUserInfo objDaoUserInfo);
        bool FuncSupplierUpdate(SupplierModels viewModel, DaoUserInfo objDaoUserInfo);
        bool FuncTarmsDelete(TarmsModels viewModel);
        bool FuncTarmsEntry(TarmsModels viewModel, DaoUserInfo objDaoUserInfo);
        bool FuncTarmsUpdate(TarmsModels viewModel, DaoUserInfo objDaoUserInfo);
    }
}