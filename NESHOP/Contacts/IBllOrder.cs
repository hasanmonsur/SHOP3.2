using Microsoft.AspNetCore.Mvc.Rendering;
using NeSHOP.Models;
using NESHOP.Models;

namespace NESHOP.Contacts
{
    public interface IBllOrder
    {
        bool FucUpdateChalanStatusForInvoice(string TranStatus, string ChallanNo, string ModelCode);
        bool FucUpdateInventoryIteam(string ChalanNo);
        bool FuncChallanEntry(ChallanModels viewModel, DaoUserInfo objDaoUserInfo);
        bool FuncChallanEntryFromInvoice(InvoiceModels viewModel, DaoUserInfo objDaoUserInfo);
        bool FuncChallanIteamEntry(ChallanModels viewModel, DaoUserInfo objDaoUserInfo);
        bool FuncDeleteChallanInfo(string ChalanNo);
        bool FuncDeleteChallanItemInfo(ChallanModels viewModel);
        bool FuncDeleteInvoiceItem(IteamModels viewModel);
        bool FuncDeleteInvoiceItemInfo(InvoiceModels viewModel);
        void FuncInvoiceChieldEntry(InvoiceModels viewModel, DaoUserInfo objDaoUserInfo);
        bool FuncInvoiceChieldItemEntry(InvoiceModels viewModel, DaoUserInfo objDaoUserInfo);
        bool FuncInvoiceMasterDelete(InvoiceModels viewModel, DaoUserInfo objDaoUserInfo);
        bool FuncInvoiceMasterEntry(InvoiceModels viewModel, DaoUserInfo objDaoUserInfo);
        bool FuncInvoiceMasterUpdate(InvoiceModels viewModel, DaoUserInfo objDaoUserInfo);
        SelectList FuncIteamCodeList(string IteamCode);
        bool FuncOrderEntry(OrderModels objOrder, DaoUserInfo objDaoUserInfo);
        void FuncOrderTermsEntry(OrderModels viewModel, DaoUserInfo objDaoUserInfo);
        string FuncRetunUnitPrice(string qString);
        List<IteamModels> FuncReturnAllInvoic(string ids);
        string FuncReturnCatagoryName(string Code);
        SelectList FuncReturnChallanItemList(string ChallanNo);
        List<ChallanModel> FuncReturnChallanList(ChallanModels viewModel);
        List<string> FuncReturnChallanQuantity(string Sel_StateName);
        List<ChallanModel> FuncReturnChallanReport(ChallanModels viewModel);
        string FuncReturnChallanTranNo(DaoUserInfo objDaoUserInfo);
        string FuncReturnCustomerCode(string ids);
        List<IteamModels> FuncReturnDeleteChalaData(List<IteamModels> objIteamlList, string ids);
        List<IteamModels> FuncReturnDeleteInvoiceIteam(List<IteamModels> objIteamlList, string ids);
        List<IteamModels> FuncReturnDeleteOrderData(List<IteamModels> iteamList, string ids);
        SelectList FuncReturnInvoiceItemList(string InvoiceNo);
        List<InvoiceModel> FuncReturnInvoiceList(InvoiceModels viewModel);
        List<InvoiceModel> FuncReturnInvoiceListReport(InvoiceModels objOrder);
        List<InvoiceModel> FuncReturnInvoiceReport(InvoiceModels viewModel);
        InvoiceModels FuncReturnInvoiceReportMaster(string InvoiceNo);
        string FuncReturnInvoiceStatusUpdate(string InvoiceNo, string TranStatus);
        string FuncReturnInvoiceTranNo(DaoUserInfo objDaoUserInfo);
        string[] FuncReturnItemInformation(string Sel_StateName);
        string[] FuncReturnModelDesc(string ids);
        string FuncReturnModelName(string Code);
        string FuncReturnModelSlNo(string Sel_StateName);
        string FuncReturnOrdRefCode(string InstCode);
        string FuncReturnOrdTranNo(DaoUserInfo objDaoUserInfo);
        IteamModels FuncReturnParseChallanData(string ids);
        IteamModels FuncReturnParseChallanInvoiceData(string ids);
        IteamModels FuncReturnParseOrderData(string ids);
        string FuncReturnProductName(string Code);
        string[] FuncReturnProductQuantity(string Sel_StateName);
        List<OrderModel> FuncReturnQutationDetailsList(OrderModels viewModel);
        List<OrderModel> FuncReturnQutationList(OrderModels viewModel);
        List<ChallanModel> FuncReturnSalesReturnChallanData(ChallanModels viewModel);
        SelectList FuncReturnSelectChallanList(string CustomerCode);
        SelectList FuncReturnSelectChallanList(string CustomerCode, string ChallanNo);
        SelectList FuncReturnSelectChallanModelList(string Sel_StateName);
        SelectList FuncReturnSelectChallanModelList(string Sel_StateName, string ModelCode, string PairCode);
        SelectList FuncReturnSelectChallanModelSearchList(string Sel_StateName, string ModelCode, string Quantity);
        SelectList FuncReturnSelectChallanModelSerialList(string Sel_StateName);
        SelectList FuncReturnSelectChallanSearchList(string CustomerCode, string ChallanNo);
        SelectList FuncReturnSelectInvoiceModelList(string Sel_StateName);
        string[] FuncReturnSumData(List<IteamModels> IteamModelList);
        List<TermsModel> FuncReturnTermsList(string QutationNo);
        string FuncReturnVenName(string Code);
        ChallanModels FuncSearchChallanItemInfo(ChallanModels viewModel);
        InvoiceModels FuncSearchInvoiceItemInfo(InvoiceModels viewModel);
        InvoiceModels FuncSearchInvoiceMasterInfo(InvoiceModels viewModel);
        void FuncUpdateInventoryIteam(IteamModels objOrd);
        bool FuncWarrantyNewInfoEntry(InvoiceReturnModels viewModel, DaoUserInfo objDaoUserInfo);
        bool FuncWarrentyInfoEntry(InvoiceReturnModels viewModel, DaoUserInfo objDaoUserInfo);
    }
}