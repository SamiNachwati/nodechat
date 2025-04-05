using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Assignment4ChatApplication.Models;

// I, Sami Nachwati, student number 000879289, certify that this material is my original work. No other person's
// work has been used without due acknowledgment and I have not made my work available to anyone else.
public class ChatDbContext : DbContext
{
    /// <summary>
    /// Constructor for Db Context
    /// </summary>
    /// <param name="options"></param>
    public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options) { }

    /* Database set of users in the DB */
    public DbSet<User> Users { get; set; }

    /// <summary>
    /// Method to determine if user is in the database
    /// </summary>
    /// <param name="username">username entered by user</param>
    /// <returns>true if user exists, false if not</returns>
    public bool HasUser(string username)
    {
        if (string.IsNullOrWhiteSpace(username)) return false;

        return Users.Any(u => u.Username.ToLower() == username.ToLower());
    }


    /// <summary>
    /// Method used to return a user within the database by username
    /// </summary>
    /// <param name="username">username from user</param>
    /// <returns>User if user exists otherwise null</returns>
    public User? GetUser(string username)
    {
        if (string.IsNullOrWhiteSpace(username)) return null;

        return Users.FirstOrDefault(u => u.Username.ToLower() == username.ToLower());
    }


    /// <summary>
    /// Method used to update the user message
    /// </summary>
    /// <param name="username">username of user</param>
    /// <param name="message">message of user</param>
    /// <returns>User if user exists</returns>
    public User? UpdateUserMessage(string username, string message)
    {
        if (string.IsNullOrWhiteSpace(username)) return null;

        var user = GetUser(username);
        if (user == null) return null;

        user.Message = message;
        SaveChanges();

        return user;
    }


    /// <summary>
    /// Method used to update the user message time
    /// </summary>
    /// <param name="username">username of user</param>
    public void UpdateUserMessageTime(string username)
    {
        var user = GetUser(username);
        if (user != null)
        {
            user.MessageAt = DateTimeOffset.Now;
            SaveChanges();
        }
    }

    /// <summary>
    /// Method used to update the user connection time
    /// </summary>
    /// <param name="username">username of user</param>
    public void UpdateUserConnectionTime(string username)
    {
        var user = GetUser(username);
        if (user != null)
        {
            user.ConnectedAt = DateTimeOffset.Now;
            SaveChanges();
        }
    }

    /// <summary>
    /// Method used to update the user disconnection time
    /// </summary>
    /// <param name="username">username of user</param>
    public void UpdateUserDisconnectedTime(string username)
    {
        var user = GetUser(username);
        if (user != null)
        {
            user.DisconnectedAt = DateTimeOffset.Now;
            SaveChanges();
        }
    }
}
