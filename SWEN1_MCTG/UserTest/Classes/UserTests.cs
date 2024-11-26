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
    private User createdUser;

    private string player1Username;
    private string player1Password;
    private string player2Username;
    private string player2Password;

    [TestInitialize]
    public void Setup()
    {
        // Initialize the fields
        player1Username = "createdUserUsername";
        player1Password = "createdUserPassword";

        // Create the users
        
        User.Create(player1Username, player1Password);

        // Retrieve the users
        createdUser = User.Get(player1Username);
    }

    [TestMethod]
    public void Constructor_CreateUser_UsernameAndPasswordEqualToStringParameters()
    {
        Assert.AreEqual(player1Username, createdUser.Username);
        Assert.AreEqual(player1Password, createdUser.Password);
    }

    [TestMethod]
    public void Constructor_CreateUser_UserStacksAreEmptyCardLists()
    {
        bool expectedEmpty = false;


        Assert.AreEqual(expectedEmpty, createdUser.UserCards.Cards.Any());
        Assert.AreEqual(expectedEmpty, createdUser.UserDeck.Cards.Any());
        Assert.AreEqual(expectedEmpty, createdUser.UserHand.Cards.Any());
        Assert.AreEqual(expectedEmpty, createdUser.UserDiscard.Cards.Any());
    }

    [TestMethod]
    public void Constructor_CreateUser_UserHasTwentyCoins()
    {
        int expectedCoins = 20;

        Assert.AreEqual(expectedCoins, createdUser.UserCoinPurse.GetCoinsValue());
    }

}

