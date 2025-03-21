namespace WebParfum.API.Services
{
    public interface IAuthenticationService
    {
        /// <summary>
        /// Autentica a un usuario con su correo y contraseña.
        /// Retorna el token JWT si la autenticación es exitosa, o null en caso contrario.
        /// </summary>
        Task<string> AuthenticateAsync(string email, string password);
    }
}
