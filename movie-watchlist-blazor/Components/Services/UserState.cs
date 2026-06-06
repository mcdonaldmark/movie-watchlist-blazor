using System.Collections.Generic;
using System.Linq;

namespace movie_watchlist_blazor.Services;

public static class UserState
{
    public static List<(string Name, string Email, string Password)> Users { get; set; } = new();

    public static bool Register(string name, string email, string password)
    {
        // prevent duplicate email
        bool exists = Users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

        if (exists)
        {
            return false;
        }

        Users.Add((name, email, password));
        return true;
    }

    public static bool Login(string email, string password)
    {
        return Users.Any(u =>
            u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) &&
            u.Password == password);
    }
}