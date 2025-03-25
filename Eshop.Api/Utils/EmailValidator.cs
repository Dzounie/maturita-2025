namespace Eshop.Api.Utils;

public static class EmailValidator
{
    public static readonly string[] AllowedDomains = {"com", "cz", "net", "org" }; // Povolené přípony
    public static readonly string[] AllowedWebs = {"gmail", "seznam", "outlook", "email", "post", "gymspit", "email" }; // Povolené přípony

    public static bool IsValidEmail(string email)
    {
        try
        {
            // Extrakce přípony z e-mailu
            string domain = email.Split('.').Last().ToLower();
            string web = email.Split('@').Last().Split('.').First().ToLower();
            return AllowedDomains.Contains(domain) && AllowedWebs.Contains(web);
        }
        catch (Exception)
        {
            return false;
        }
    }
}
