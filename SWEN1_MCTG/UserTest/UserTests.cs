using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWEN1_MCTG;
using SWEN1_MCTG.Classes;

namespace UserTest;


[TestClass]
public class UserTests
{
    private static User createdUser;
    private static string playerUsername;
    private static string playerPassword;

    [ClassInitialize]
    public static void Setup(TestContext context)
    {
        // Initialize the fields
        playerUsername = "createdUserUsername";
        playerPassword = "createdUserPassword";

        // Create the user
        User newUser = new User(playerUsername, playerPassword);

        createdUser = newUser;
    }

    [TestMethod]
    public void Constructor_CreateUser_UsernameAndPasswordEqualToStringParameters()
    {
        Assert.AreEqual(playerUsername, createdUser.Username);
        Assert.AreEqual(PasswordHelper.HashPassword(playerPassword), createdUser.Password);
    }

    [TestMethod]
    public void Constructor_CreateUser_UserStacksAreEmptyCardLists()
    {
        bool expectedEmpty = false;

        Assert.AreEqual(expectedEmpty, createdUser.UserCards.Cards.Any());
        Assert.AreEqual(expectedEmpty, createdUser.UserDeck.Cards.Any());
    }
}

