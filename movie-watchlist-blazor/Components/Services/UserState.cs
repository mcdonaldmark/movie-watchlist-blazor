using System.Collections.Generic;
using System.Linq;

namespace movie_watchlist_blazor.Services;

public static class UserState
{
    // Different lists to hold the login info for the user
    // In memory data to store the user login info
    public static List<(string Name, string Email, string Password)> Users { get; set; } = new();

    public static bool Register(string name, string email, string password)
    {
        // Allows a new account to be created if it is unique
        // Prevents a duplicate email from being created if one already exists
        // Returns True or False based on what the user enters in
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
        // If the login credentials from the user are valid then the user login in returned
        // and the user is able to log into the application
        return Users.Any(u =>
            u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) &&
            u.Password == password);
    }
}