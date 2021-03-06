namespace BeautyAtHome.Utils
{
    public class Constants
    {

        public static readonly int PAGE_SIZE = 50;

        public static readonly int MAXIMUM_PAGE_SIZE = 250;

        public static readonly string EXPIRES_IN_DAY = "86400";

        public static class Role
        {
            public const string ADMIN = "CUSTOMER";
        }

        public static class PrefixPolicy
        {
            public const string REQUIRED_ROLE = "RequiredRole";
        }

        public static class TokenClaims
        {
            public const string ROLE = "role";
            public const string UID = "uid";
            public const string EMAIL = "email";
        }

        public static class HeaderClaims
        {
            public const string FIREBASE_AUTH = "FirebaseAuth";
        }

        public static class AccountStatus
        {
            public const string ACTIVE = "ACTIVE";
        }

        public static class Status
        {
            public const string ACTIVE = "Active";

            public const string DISABLED = "Disabled";
        }

        public static class AppSetting
        {
            public const string FirebaseBucket = "Firebase:Bucket";
            public const string FirebaseApiKey = "Firebase:ApiKey";
            public const string FirebaseAuthEmail = "Firebase:Email";
            public const string FirebaseAuthPassword = "Firebase:Password";
            
        }
    }
}
