using Assignment4ChatApplication.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Assignment4ChatTests
// I, Sami Nachwati, student number 000879289, certify that this material is my original work. No other person's
// work has been used without due acknowledgment and I have not made my work available to anyone else.
{
    [TestClass]
    public class UserTests
    {
        /// <summary>
        /// Method used to obtain the DB context
        /// </summary>
        /// <returns></returns>
        private ChatDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ChatDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ChatDbContext(options);
        }


        /// <summary>
        /// Method used to test if a user exists
        /// </summary>
        [TestMethod]
        public void HasUser_ShouldReturnTrue_WhenUserExists()
        {
            var db = GetInMemoryDbContext();
            db.Users.Add(new User { Username = "Aisha" });
            db.SaveChanges();

            var result = db.HasUser("Aisha");

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Method used to test if system will return null if user doesnt exist
        /// </summary>
        [TestMethod]
        public void GetUser_ShouldReturnNull_WhenUserDoesNotExist()
        {
            var db = GetInMemoryDbContext();

            var result = db.GetUser("Unknown");

            Assert.IsNull(result);
        }


        /// <summary>
        /// Method used to test if user message content is properly updated
        /// </summary>
        [TestMethod]
        public void UpdateUserMessage_ShouldUpdateCorrectly()
        {
            var db = GetInMemoryDbContext();
            db.Users.Add(new User { Username = "Bilal", Message = "Old message" });
            db.SaveChanges();

            db.UpdateUserMessage("Bilal", "New message");

            var updatedUser = db.GetUser("Bilal");
            Assert.AreEqual("New message", updatedUser.Message);
        }


        /// <summary>
        /// Method used to test that the user disconnected time is of proper type
        /// </summary>
        [TestMethod]
        public void UpdateUserDisconnectedTime_ShouldSetTimestamp()
        {
            var db = GetInMemoryDbContext();
            db.Users.Add(new User { Username = "Khadija" });
            db.SaveChanges();

            db.UpdateUserDisconnectedTime("Khadija");

            var user = db.GetUser("Khadija");
            Assert.IsNotNull(user.DisconnectedAt);
            Assert.IsInstanceOfType(user.DisconnectedAt, typeof(DateTimeOffset));
        }
    }
}
