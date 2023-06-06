namespace Web_App.Data
{
    public sealed class Credentials
    {
        public int CredentialId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
