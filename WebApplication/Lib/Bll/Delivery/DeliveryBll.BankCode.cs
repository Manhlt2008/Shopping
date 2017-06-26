using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics.CodeAnalysis;

namespace WebApplication.Lib.Bll.Delivery
{
    public static partial class DeliveryBll
    {
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public static class DeliveryEnum
        {
            public const string PICKUP_DELAY_REASON = "Lý do delay lấy hàng";

            public const string PICKUP_FAIL_REASON = "Lý do không lấy được hàng";

            public const string DELIVERY_DELAY_REASON = "Lý do delay giao hàng";

            public const string DELIVERY_FAIL_REASON = "Lý do không giao được hàng";

            public const string RETURN_DELAY_REASON = "Lý do delay trả hàng";

            public enum StatusDeliveryOrder
            {
                cancel = -1,
                unknown = 0,
                notreceived = 1,
                received = 2,
                warehouseadded = 3,
                isdelivery = 4,
                delivered = 5,
                controlled = 6,
                notgetproduct = 7,
                delaygetproduct = 8,
                cantdelivery = 9,
                delaydelivery = 10,
                debtreconciliation = 11,
                isreturnorder = 12,
                returnorder = 20,
                returned = 21,

                cantcontacttosupplier = 101,
                supplieroutofstock = 102,
                supplierchangeaddress = 103,
                supplierchangedate = 104,
                deliverylate = 105,
                outwardnessreason = 106,

                outofarea = 110,
                productnotaccepttodelivery = 111,
                productcancelbysupplier = 112,
                supplierlate = 113,

                customerdealnextday = 121,
                cantcontactwithcustomer = 122,
                customerdealanotherday = 123,
                customerchangeaddress = 124,
                wrongaddress = 125,
                objectivenessreason = 126,
                ortherreason = 127,

                customerrejectorder = 130,
                threetimescontactsupplier = 131,
                customerdelaythreetimes = 132,
                suppliercancelorder = 133,
                deliveryorderreason = 134,

                supplierdealanothersessionreturn = 140,
                cantcontactsuppliertoreturn = 141,
                suppliernotathome = 142,
                supplierdealanotherdayreturn = 143,
                supplierorderreason = 144



            }

            public static StatusDeliveryOrder ParseDeliveryCode(string code)
            {
                if (!Enum.IsDefined(typeof(StatusDeliveryOrder), code))
                    return StatusDeliveryOrder.unknown;

                return (StatusDeliveryOrder)Enum.Parse(typeof(StatusDeliveryOrder), code);
            }
          

            public static string DeliveryCodeDetail(StatusDeliveryOrder code)
            {
                switch (code)
                {
                    case StatusDeliveryOrder.cancel:
                        return "Hủy đơn hàng";
                    case StatusDeliveryOrder.notreceived:
                        return "Chưa tiếp nhận";
                    case StatusDeliveryOrder.received:
                        return "Đã tiếp nhận";
                    case StatusDeliveryOrder.warehouseadded:
                        return "Đã lấy hàng/Đã nhập kho";
                    case StatusDeliveryOrder.isdelivery:
                        return "Đã điều phối giao hàng/Đang giao hàng";
                    case StatusDeliveryOrder.delivered:
                        return "Đã giao hàng/Chưa đối soát";
                    case StatusDeliveryOrder.controlled:
                        return "Đã đối soát";
                    case StatusDeliveryOrder.notgetproduct:
                        return "Không lấy được hàng";
                    case StatusDeliveryOrder.delaygetproduct:
                        return "Delay lấy hàng";
                    case StatusDeliveryOrder.cantdelivery:
                        return "Không giao được hàng";
                    case StatusDeliveryOrder.delaydelivery:
                        return "Delay giao hàng";
                    case StatusDeliveryOrder.debtreconciliation:
                        return "Đã đối soát công nợ trả hàng";
                    case StatusDeliveryOrder.isreturnorder:
                        return "Đã điều phối lấy hàng/Đang lấy hàng";
                    case StatusDeliveryOrder.returnorder:
                        return "Đang trả hàng (COD cầm hàng đi trả)";
                    case StatusDeliveryOrder.returned:
                        return "Đã trả hàng (COD đã trả xong hàng)";
                    case StatusDeliveryOrder.cantcontacttosupplier:
                        return PICKUP_DELAY_REASON + " : Không liên lạc được với nhà cung cấp";
                    case StatusDeliveryOrder.supplieroutofstock:
                        return PICKUP_DELAY_REASON + ": NCC chưa có hàng";
                    case StatusDeliveryOrder.supplierchangeaddress:
                        return PICKUP_DELAY_REASON + " : NCC đổi địa chỉ";
                    case StatusDeliveryOrder.supplierchangedate:
                        return PICKUP_DELAY_REASON + " : NCC hẹn ngày lấy";
                    case StatusDeliveryOrder.deliverylate:
                        return PICKUP_DELAY_REASON + ": GHTK không lấy kịp";
                    case StatusDeliveryOrder.outwardnessreason:
                        return PICKUP_DELAY_REASON + " : Do điều kiện thời tiết, khách quan";
                    case StatusDeliveryOrder.outofarea:
                        return PICKUP_FAIL_REASON + " : Địa chỉ ngoài vùng phục vụ";
                    case StatusDeliveryOrder.productnotaccepttodelivery:
                        return PICKUP_FAIL_REASON + ": Hàng không nhận vận chuyển";
                    case StatusDeliveryOrder.productcancelbysupplier:
                        return PICKUP_FAIL_REASON + " : NCC báo hủy";
                    case StatusDeliveryOrder.supplierlate:
                        return PICKUP_FAIL_REASON + " : NCC delay/không liên lạc được 3 lần";
                    case StatusDeliveryOrder.customerdealnextday:
                        return DELIVERY_DELAY_REASON + " : KH hẹn giao ca tiếp theo";
                    case StatusDeliveryOrder.cantcontactwithcustomer:
                        return DELIVERY_DELAY_REASON + " : Không gọi được cho KH";
                    case StatusDeliveryOrder.customerdealanotherday:
                        return DELIVERY_DELAY_REASON + " : KH hẹn ngày giao";
                    case StatusDeliveryOrder.customerchangeaddress:
                        return DELIVERY_DELAY_REASON + " : KH chuyển địa chỉ mới";
                    case StatusDeliveryOrder.wrongaddress:
                        return DELIVERY_DELAY_REASON + " : Địa chỉ/ SĐT KH sai, chờ NCC check lại";
                    case StatusDeliveryOrder.objectivenessreason:
                        return DELIVERY_DELAY_REASON + " : Do điều kiện thời tiết, khách quan";
                    case StatusDeliveryOrder.ortherreason:
                        return DELIVERY_DELAY_REASON + " : Lý do khác";
                    case StatusDeliveryOrder.customerrejectorder:
                        return DELIVERY_FAIL_REASON + " : KH không đồng ý nhận sản phẩm";
                    case StatusDeliveryOrder.threetimescontactsupplier:
                        return DELIVERY_FAIL_REASON + " : Không liên lạc được với KH 3 lần";
                    case StatusDeliveryOrder.customerdelaythreetimes:
                        return DELIVERY_FAIL_REASON + " : KH hẹn giao lại quá 3 lần";
                    case StatusDeliveryOrder.suppliercancelorder:
                        return DELIVERY_FAIL_REASON + " : Shop báo hủy đơn hàng";
                    case StatusDeliveryOrder.deliveryorderreason:
                        return DELIVERY_FAIL_REASON + " : Lý do khác";
                    case StatusDeliveryOrder.supplierdealanothersessionreturn:
                        return RETURN_DELAY_REASON + " : NCC hẹn trả ca sau";
                    case StatusDeliveryOrder.cantcontactsuppliertoreturn:
                        return RETURN_DELAY_REASON + " : Không liên lạc được với NCC";
                    case StatusDeliveryOrder.suppliernotathome:
                        return RETURN_DELAY_REASON + " : NCC không có nhà";
                    case StatusDeliveryOrder.supplierdealanotherdayreturn:
                        return RETURN_DELAY_REASON + " : NCC hẹn ngày trả";
                    case StatusDeliveryOrder.supplierorderreason:
                        return RETURN_DELAY_REASON + " : Lý do khác";
                    default:
                        return string.Empty;

                }
            }

        }
    }
}