using Microsoft.AspNetCore.Mvc.Rendering;
using NeSHOP.Models;
using NESHOP.Models;

namespace NESHOP.Contacts
{
    public interface IBllTran
    {
        List<string> FuncBuyTranDataSum(List<ChallanModels> TranIteamList);
        InvoiceModels FuncInvoiceInfo(string Sel_StateName);
        List<AccountSumModel> FuncReturnAccountDataList(AcTranModel viewModel);
        List<AccountSumModel> FuncReturnAccountDetailsData(AcTranModel viewModel);
        List<AcTranModel> FuncReturnDailyTranList(AcTranModel viewModel);
        SelectList FuncReturnIteamCodeList(string IteamCode);
        List<AcTranModel> FuncReturnSearchTranList(AcTranModel viewModel);
        SelectList FuncReturnSelectInvoiceList(string Code);
        object FuncReturnSelectModifyInvoiceList(string Sel_StateName);
        bool FuncReturnTranDelete(string TranNo);
        List<AcTranModel> FuncReturnTranList(string TranNo);
        string FuncReturnTranNo(DateTime BusinessDate);
        List<AcTranPaymentModel> FuncReturnTranPaymentList(string TranNo);
        bool FuncTransactionEntry(AcTranModel objTran, DaoUserInfo objDaoUserInfo);
        AcTranModel FuncTransactionInfo(string Sel_StateName);
        bool FuncTransactionPaymentEntry(AcTranPaymentModel viewModel, DaoUserInfo objDaoUserInfo);
        AcTranPaymentModel FuncTransactionPaymentInfo(string Sel_StateName);
        bool FuncTransactionPaymentUpdate(AcTranPaymentModel viewModel);
        bool FuncTransactionUpdate(AcTranModel objTran);
    }
}