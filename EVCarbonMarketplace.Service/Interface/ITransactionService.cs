using EVCarbonMarketplace.Model.Enum;
using EVCarbonMarketplace.Model.Paginate;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Model.Payload.Response.Transaction;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Service.Interface
{
    public interface ITransactionService
    {
        Task<BaseResponse<TransactionResponse>> Purchase(Guid listingId);

        Task<BaseResponse<IPaginate<TransactionUserResponse>>> GetAll(int page, int size, TransactionEnum? type , TransactionStatusEnum? status);
        Task<BaseResponse<IPaginate<TransactionUserResponse>>> GetMyTransaction(int page, int size, TransactionEnum? type, TransactionStatusEnum? status);


    }
}
