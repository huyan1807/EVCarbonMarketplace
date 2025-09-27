using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Model.Enum
{
     public enum DisputeTypeEnum
    {
        [Description("Gian lận")]
        Fraud,

        [Description("Lỗi thanh toán")]
        PaymentError,

        [Description("Lỗi bàn giao")]
        DeliveryError,

        [Description("Sai lệch dữ liệu")]
        DataMismatch,

        [Description("Giao dịch trái phép")]
        Unauthorized,

        [Description("Khác")]
        Other
    }
}
