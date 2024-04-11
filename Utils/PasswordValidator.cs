using System.Text.RegularExpressions;

namespace AdminGateway.Utils
{
    public class PasswordValidator
    {
        public static bool ValidatePassword(string password)
        {
            if (password.Length < 8)
            {
                throw new ArgumentException("Password length must be min 8");
            }
            else if (!Regex.IsMatch(password, @"[a-z]"))
            {
                throw new ArgumentException("Password must contains at leat 1 lowercase");
            }
            else if (!Regex.IsMatch(password, @"[A-Z]"))
            {
                throw new ArgumentException("Password must contains at leat 1 uppercase");
            }
            else if (!Regex.IsMatch(password, @"[`!@#$%^&*()_\-+=,.?/|\\]"))
            {
                throw new ArgumentException("Password must contains at leat 1 symbol");
            }

            return true;
        }
    }
}
