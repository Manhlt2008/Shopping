using System.Diagnostics.CodeAnalysis;

namespace WebApplication.Lib.Bll.Payments
{
    public static partial class A123PayBll
    {
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public static class BankCodeEnum
        {
            public const string _123PayLocalDebit = "123PAY";
            public const string _123PCCCreditCard = "123PCC";

            public const string Vietcombank = "123PVCB";
            public const string Dab = "123PDAB";
            public const string Vietinbank = "123PVTB";
            public const string Agribank = "123PAGB";
            public const string Techcombank = "123PTCB";
            public const string Eximbank = "123PEIB";
            public const string Sacombank = "123PSCB";
            public const string Vib = "123PVIB";
            public const string Bidv = "123PBIDV";
            public const string Mb = "123PMB";
            public const string Acb = "123PACB";
            public const string MaritimeBank = "123PMRTB";
            public const string GPBank = "123PGPB";
            public const string HDBank = "123PHDB";
            public const string NaviBank = "123PNVB";
            public const string VietA = "123PVAB";
            public const string VPBank = "123PVPB";
            public const string BacABank = "123PBAB";
            public const string OceanBank = "123POCEB";
            public const string ABB = "123PABB";
            public const string NamABank = "123PNAB";
            public const string SaigonBank = "123PSGB";
            public const string PGBank = "123PPGB";
            public const string OCB = "123POCB";
            public const string DaiABank = "123PDAIAB";
            public const string TienPhongBank = "123PTPB";
            public const string MasterCardVisaCardJCB = "123PCC";
        }
    }
}