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
           public const string Register = AccountEndPoint + "/register/CcBuyer";
        }
    }
}
