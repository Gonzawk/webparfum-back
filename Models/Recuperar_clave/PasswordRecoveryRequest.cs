namespace WebParfum.API.Models.Recuperar_clave
{
    public class PasswordRecoveryRequest
    {
        public string Email { get; set; }
    }

    public class PasswordResetRequest
    {
        public string Code { get; set; }
        public string NewPassword { get; set; }
    }
}
