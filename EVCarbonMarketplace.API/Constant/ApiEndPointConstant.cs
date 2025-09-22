namespace EVCarbonMarketplace.API.Constant
{
    public class ApiEndPointConstant
    {
        static ApiEndPointConstant()
        {

        }
        public const string RootEndPoint = "/api";
        public const string ApiVersion = "/v1";
        public const string ApiEndpoint = RootEndPoint + ApiVersion;

        public static class Account
        {
           public const string AccountEndPoint = ApiEndpoint + "/account";
           public const string Otp = AccountEndPoint + "/otp";
           public const string Register = AccountEndPoint + "/register";
            public const string ChangePassword = AccountEndPoint + "/change-password";
            public const string ForgotPassword = AccountEndPoint + "/forgot-password";
            public const string VerifyOtp = AccountEndPoint + "/verify-otp";
            public const string ResetPassword = AccountEndPoint + "/reset-password";
            public const string ChangeAvatar = AccountEndPoint + "/change-avatar";
        }
        public static class Authentication
        {
            public const string AuthenticationEndPoint = ApiEndpoint + "/auth";
            public const string Authenticate = AuthenticationEndPoint;

        }
        public static class GoogleAuthentication
        {
            public const string GoogleAuthEndPoint = ApiEndpoint + "/google-auth";
            public const string GoogleAuthLogin = GoogleAuthEndPoint + "/login";
            public const string GoogleAuthSignIn = GoogleAuthEndPoint + "/sign-in";
        }
        public static class User
        {
            public const string UserEndPoint = ApiEndpoint + "/user";
            public const string GetProfile = UserEndPoint + "/profile";
            public const string GetAllUsers = UserEndPoint;
            public const string GetUser = UserEndPoint + "/{id}";
            public const string DeleteUser = UserEndPoint + "/{id}";
            public const string UpdateUser = UserEndPoint;
        }
        public static class Owner
        {
            public const string OwnerEndPoint = ApiEndpoint + "/owner";
            public const string Register = OwnerEndPoint + "/register";
        }
        public static class Cva
        {
            public const string CvaEndPoint = ApiEndpoint + "/cva";
            public const string Register = CvaEndPoint + "/register";
        }

        public static class EVehicle
        {
            public const string ElectricVehicle = ApiEndpoint + "/electric-vehicle";
            public const string Create = ElectricVehicle;
            public const string Update = ElectricVehicle + "/{id}";
            public const string GetAll = ElectricVehicle;
            public const string GetById = ElectricVehicle + "/{id}";
            public const string Delete = ElectricVehicle + "/{id}";
            public const string ChangeImage = Update + "/change-image";
            public const string GetMyEVehicles = ElectricVehicle + "/my-vehicles";
        }
        public static class CarbonEmissions
        {
            public const string CarbonEmission = ApiEndpoint + "/carbon-emission";
            public const string GetById = CarbonEmission + "/{id}";
            public const string GetAll = CarbonEmission;
            public const string GetByEVehicle = CarbonEmission + "/by-EVehicle/{Id}";
            public const string Create = CarbonEmission + "/by-EVehicle/{Id}";
            public const string ApproveEmission = CarbonEmission + "/{id}/approve";
        }
        public static class CarbonCredits
        {
            public const string CarbonCredit = ApiEndpoint + "/carbon-credit";
            public const string GetMyCredits = CarbonCredit + "/my-credits";
            public const string GetAll = CarbonCredit;
            public const string GetDetail = CarbonCredit + "/{id}";
        }

        public static class Payment
        {
            public const string PaymentEndPoint = ApiEndpoint + "/payment";
            public const string Create = PaymentEndPoint;
            public const string Webhook = PaymentEndPoint + "/webhook";
        }

        public static class VehicleType
        {
            public const string VehicleTypeEndPoint = ApiEndpoint + "/vehicle-type";
            public const string GetAll = VehicleTypeEndPoint;
        }

        public static class Wallet
        {
            public const string WalletEndPoint = ApiEndpoint + "/wallet";
            public const string GetMyWallet = WalletEndPoint + "/my-wallet";
        }

        public static class CarbonListing
        {
            public const string CarbonListingEndPoint = ApiEndpoint + "/carbon-listing";
            public const string CreateSellListing = CarbonListingEndPoint + "/sell";
            public const string CreateBuyListing = CarbonListingEndPoint + "/buy";
            public const string GetAll = CarbonListingEndPoint;
            public const string GetById = CarbonListingEndPoint + "/{id}";
            public const string GetMyListings = CarbonListingEndPoint + "/my-listings";
            public const string Delete = CarbonListingEndPoint + "/{id}";
            public const string Update = CarbonListingEndPoint + "/{id}";

        }
        public static class Transaction
        {
            public const string TransactionEndPoint = ApiEndpoint + "/transaction";
            public const string Purchase = TransactionEndPoint + "/purchase/{listingId}";
            public const string GetMyTransactions = TransactionEndPoint + "/my-transactions";
            public const string GetAll = TransactionEndPoint;
        }

        public static class Bid
        {
            public const string BidEndPoint = ApiEndpoint + "/bid";
            public const string PlaceBid = BidEndPoint + "/placeBid";
            public const string FinalizeAuction = BidEndPoint + "/finalize-auction";
            public const string GetCurrentBid = BidEndPoint + "/current-bid";
        }
        public static class VehicleTelemetry
        {
            public const string VehicleTelemetryEndPoint = ApiEndpoint + "/vehicle-telemetry";
            public const string LogTelemetry = VehicleTelemetryEndPoint;
            public const string GetByEVehicle = VehicleTelemetryEndPoint + "/by-EVehicle/{id}";
        }

    }
}
