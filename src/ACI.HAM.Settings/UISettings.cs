namespace ACI.HAM.Settings
{
    public class UISettings
    {
        public string BaseUrl { get; set; } = "http://localhost:4200";

        public string ChangeEmailRelativeUrl { get; set; } = "/auth/change-email";

        public string ConfirmEmailAndSetPasswordRelativeUrl { get; set; } = "/auth/confirm-email-and-set-password";

        public string ConfirmEmailRelativeUrl { get; set; } = "/auth/confirm-email";

        public string ResetPasswordRelativeUrl { get; set; } = "/auth/reset-password";
    }
}
