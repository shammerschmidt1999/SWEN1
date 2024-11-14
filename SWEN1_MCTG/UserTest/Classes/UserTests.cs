using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWEN1_MCTG.Classes;

namespace UserTest.Classes;


[TestClass]
public class UserTests
{
    [TestMethod]
    public void Constructor_CreateUser_UsernameAndPasswordEqualToStringParameters()
    {
        // Arrange (Prepare code for thest)
        string username = "TestUsername";
        string password = "TestPassword";

        // Act (Perform Method)
        var user = new User(username, password);

        // Assert (Check for expected outcome)
        Assert.AreEqual(username, user.Username);
        Assert.AreEqual(password, user.Password);
    }

    [TestMethod]
    public void Constructor_CreateUser_UserStacksAreEmptyCardLists()
    {
        string username = "TestUsername";
        string password = "TestPassword";

        var user = new User(username, password);
        bool expectedEmpty = false;

        Assert.AreEqual(expectedEmpty, user.UserCards.Cards.Any());
        Assert.AreEqual(expectedEmpty, user.UserDeck.Cards.Any());
        Assert.AreEqual(expectedEmpty, user.UserHand.Cards.Any());
        Assert.AreEqual(expectedEmpty, user.UserDiscard.Cards.Any());
    }

    [TestMethod]
    public void Constructor_CreateUser_UserHasTwentyCoins()
    {
        string username = "TestUsername";
        string password = "TestPassword";
        int expectedCoins = 20;

        var user = new User(username, password);

        Assert.AreEqual(expectedCoins, user.UserCoinPurse.GetCoinsValue());
    }

}

