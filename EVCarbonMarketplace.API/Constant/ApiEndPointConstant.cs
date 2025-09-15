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

    }
}
