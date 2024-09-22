namespace Domain.Configuration
{
    /// <summary>
    /// Used by the Web.Api to create JWTokens.
    /// </summary>
    public class EncryptionSettings
    {
        public static string EncryptionKey { get; set; }

        public static string Issuer { get; set; }

        public static string Audience { get; set; }
    }
}
