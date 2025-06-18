using BCrypt.Net;

namespace Kervan.Web.Helpers.aboutHash
{
    public class HasherByBcrypt
    {

        public static string HashBcrypt(string password)
        {
            if (string.IsNullOrEmpty(password))
           {
                throw new ArgumentException("Parola boş olamaz!", nameof(password));
            }
             
            int saltRounds = 12;
             
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, saltRounds);

            return hashedPassword;
        }
        public static bool Verify(string plainPassword, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword);
        }
    }
}

