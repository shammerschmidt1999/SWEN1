using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWEN1_MCTG;
using SWEN1_MCTG.Classes;

namespace BattleTest;

[TestClass]
public class BattleTests
{

    private static User player1;
    private static User player2;

    private static string player1Username;
    private static string player1Password;
    private static string player2Username;
    private static string player2Password;

    [ClassInitialize]
    public static void Setup(TestContext context)
    {
        player1Username = "Player1Name";
        player1Password = "Player1Password";
        player2Username = "Player2Name";
        player2Password = "Player2Password";

        // Create the users
        User.Create(player1Username, player1Password);
        User.Create(player2Username, player2Password);

        // Retrieve the users
        player1 = User.Get(player1Username);
        player2 = User.Get(player2Username);
    }

    [TestMethod]
    public void Constructor_CreateBattle_PropertiesAreSetCorrectly()
    {
        int roundCount = 0;
        int maxRounds = 100;

        // Create the battle
        Battle testBattle = new Battle(player1, player2);

        // Assert the properties
        Assert.AreEqual(player1Username, testBattle.Player1.Username);
        Assert.AreEqual(player1Password, testBattle.Player1.Password);
        Assert.AreEqual(player2Username, testBattle.Player2.Username);
        Assert.AreEqual(player2Password, testBattle.Player2.Password);
        Assert.AreEqual(roundCount, testBattle.RoundCount);
        Assert.AreEqual(maxRounds, testBattle.MaxRounds);
    }

    [TestMethod]
    public void CalculateDamage_SpellVsSpellDamage_SpellAttackAgainstEffectiveElementDoublesCardDamage()
    {
        string player1CardName = "Player1SpellCard";
        string player2CardName = "Player2SpellCard";
        double player1CardDamage = 20;
        double player2CardDamage = 20;
        GlobalEnums.ElementType player1CardElementType = GlobalEnums.ElementType.Water;
        GlobalEnums.ElementType player2CardElementType = GlobalEnums.ElementType.Fire;

        SpellCard player1SpellCard = new SpellCard(player1CardName, player1CardDamage, player1CardElementType);
        SpellCard player2SpellCard = new SpellCard(player2CardName, player2CardDamage, player2CardElementType);

        Battle testBattle = new Battle(player1, player2);

        double calculatedDamage = testBattle.CalculateDamage(player1SpellCard, player2SpellCard, player1CardDamage);

        Assert.AreEqual(player1CardDamage * 2, calculatedDamage);
    }

    [TestMethod]
    public void CalculateDamage_SpellVsSpellDamage_SpellAttackAgainstNotEffectiveElementHalvesCardDamage()
    {
        string player1CardName = "Player1SpellCard";
        string player2CardName = "Player2SpellCard";
        double player1CardDamage = 20;
        double player2CardDamage = 20;
        GlobalEnums.ElementType player1CardElementType = GlobalEnums.ElementType.Fire;
        GlobalEnums.ElementType player2CardElementType = GlobalEnums.ElementType.Water;

        SpellCard player1SpellCard = new SpellCard(player1CardName, player1CardDamage, player1CardElementType);
        SpellCard player2SpellCard = new SpellCard(player2CardName, player2CardDamage, player2CardElementType);

        Battle testBattle = new Battle(player1, player2);

        double calculatedDamage = testBattle.CalculateDamage(player1SpellCard, player2SpellCard, player1CardDamage);

        Assert.AreEqual(player1CardDamage / 2, calculatedDamage);
    }

    [TestMethod]
    public void CalculateDamage_SpellVsSpellDamage_SpellAttackAgainstSameElementDoesNotChangeDamage()
    {
        string player1CardName = "Player1SpellCard";
        string player2CardName = "Player2SpellCard";
        double player1CardDamage = 20;
        double player2CardDamage = 20;
        GlobalEnums.ElementType player1CardElementType = GlobalEnums.ElementType.Fire;
        GlobalEnums.ElementType player2CardElementType = GlobalEnums.ElementType.Fire;

        SpellCard player1SpellCard = new SpellCard(player1CardName, player1CardDamage, player1CardElementType);
        SpellCard player2SpellCard = new SpellCard(player2CardName, player2CardDamage, player2CardElementType);

        Battle testBattle = new Battle(player1, player2);

        double calculatedDamage = testBattle.CalculateDamage(player1SpellCard, player2SpellCard, player1CardDamage);

        Assert.AreEqual(player1CardDamage, calculatedDamage);
    }

    [TestMethod]
    public void CalculateDamage_SpellVsMonsterDamage_SpellAttackAgainstKrakenNullifiesDamage()
    {
        string player1CardName = "Player1SpellCard";
        string player2CardName = "Player2MonsterCard";
        double player1CardDamage = 20;
        double player2CardDamage = 20;
        GlobalEnums.ElementType player1CardElementType = GlobalEnums.ElementType.Fire;
        GlobalEnums.ElementType player2CardElementType = GlobalEnums.ElementType.Fire;
        GlobalEnums.MonsterType player2CardMonsterType = GlobalEnums.MonsterType.Kraken;
        double spellDamageAgainstKraken = 0;

        SpellCard player1SpellCard = new SpellCard(player1CardName, player1CardDamage, player1CardElementType);
        MonsterCard player2MonsterCard = new MonsterCard(player2CardName, player2CardMonsterType, player2CardDamage, player2CardElementType);

        Battle testBattle = new Battle(player1, player2);

        double calculatedDamage = testBattle.CalculateDamage(player1SpellCard, player2MonsterCard, player1CardDamage);

        Assert.AreEqual(spellDamageAgainstKraken, calculatedDamage);
    }

    [TestMethod]
    public void CalculateDamage_SpellVsMonsterDamage_SpellAttackAgainstMonsterWithEffectiveElementDoublesDamage()
    {
        string player1CardName = "Player1SpellCard";
        string player2CardName = "Player2MonsterCard";
        double player1CardDamage = 20;
        double player2CardDamage = 20;
        GlobalEnums.ElementType player1CardElementType = GlobalEnums.ElementType.Water;
        GlobalEnums.ElementType player2CardElementType = GlobalEnums.ElementType.Fire;
        GlobalEnums.MonsterType player2CardMonsterType = GlobalEnums.MonsterType.Goblin;

        SpellCard player1SpellCard = new SpellCard(player1CardName, player1CardDamage, player1CardElementType);
        MonsterCard player2MonsterCard = new MonsterCard(player2CardName, player2CardMonsterType, player2CardDamage, player2CardElementType);

        Battle testBattle = new Battle(player1, player2);

        double calculatedDamage = testBattle.CalculateDamage(player1SpellCard, player2MonsterCard, player1CardDamage);

        Assert.AreEqual(player1CardDamage * 2, calculatedDamage);
    }

    [TestMethod]
    public void CalculateDamage_SpellVsMonsterDamage_SpellAttackAgainstSameElementDoesNotChangeDamage()
    {
        string player1CardName = "Player1SpellCard";
        string player2CardName = "Player2MonsterCard";
        double player1CardDamage = 20;
        double player2CardDamage = 20;
        GlobalEnums.ElementType player1CardElementType = GlobalEnums.ElementType.Fire;
        GlobalEnums.ElementType player2CardElementType = GlobalEnums.ElementType.Fire;
        GlobalEnums.MonsterType player2CardMonsterType = GlobalEnums.MonsterType.Goblin;

        SpellCard player1SpellCard = new SpellCard(player1CardName, player1CardDamage, player1CardElementType);
        MonsterCard player2MonsterCard = new MonsterCard(player2CardName, player2CardMonsterType, player2CardDamage, player2CardElementType);

        Battle testBattle = new Battle(player1, player2);

        double calculatedDamage = testBattle.CalculateDamage(player1SpellCard, player2MonsterCard, player1CardDamage);

        Assert.AreEqual(player1CardDamage, calculatedDamage);
    }

    [TestMethod]
    public void CalculateDamage_MonsterlVsMonsterDamage_GoblinAttackAgainstDragonNullifiesDamage()
    {
        string player1CardName = "Player1SpellCard";
        string player2CardName = "Player2MonsterCard";
        double player1CardDamage = 20;
        double player2CardDamage = 20;
        GlobalEnums.ElementType player1CardElementType = GlobalEnums.ElementType.Fire;
        GlobalEnums.MonsterType player1CardMonsterType = GlobalEnums.MonsterType.Goblin;
        GlobalEnums.ElementType player2CardElementType = GlobalEnums.ElementType.Fire;
        GlobalEnums.MonsterType player2CardMonsterType = GlobalEnums.MonsterType.Dragon;
        double goblinDamageAgainstDragon = 0;

        MonsterCard player1MonsterCard = new MonsterCard(player1CardName, player1CardMonsterType, player1CardDamage, player1CardElementType);
        MonsterCard player2MonsterCard = new MonsterCard(player2CardName, player2CardMonsterType, player2CardDamage, player2CardElementType);

        Battle testBattle = new Battle(player1, player2);

        double calculatedDamage = testBattle.CalculateDamage(player1MonsterCard, player2MonsterCard, player1CardDamage);

        Assert.AreEqual(goblinDamageAgainstDragon, calculatedDamage);
    }

    [TestMethod]
    public void CalculateDamage_MonsterlVsMonsterDamage_OrkAttackAgainstWizardNullifiesDamage()
    {
        string player1CardName = "Player1SpellCard";
        string player2CardName = "Player2MonsterCard";
        double player1CardDamage = 20;
        double player2CardDamage = 20;
        GlobalEnums.ElementType player1CardElementType = GlobalEnums.ElementType.Fire;
        GlobalEnums.MonsterType player1CardMonsterType = GlobalEnums.MonsterType.Ork;
        GlobalEnums.ElementType player2CardElementType = GlobalEnums.ElementType.Fire;
        GlobalEnums.MonsterType player2CardMonsterType = GlobalEnums.MonsterType.Wizard;
        double orkDamageAgainstWizard = 0;

        MonsterCard player1MonsterCard = new MonsterCard(player1CardName, player1CardMonsterType, player1CardDamage, player1CardElementType);
        MonsterCard player2MonsterCard = new MonsterCard(player2CardName, player2CardMonsterType, player2CardDamage, player2CardElementType);

        Battle testBattle = new Battle(player1, player2);

        double calculatedDamage = testBattle.CalculateDamage(player1MonsterCard, player2MonsterCard, player1CardDamage);

        Assert.AreEqual(orkDamageAgainstWizard, calculatedDamage);
    }

    [TestMethod]
    public void CalculateDamage_MonsterlVsMonsterDamage_DragonAttackAgainstFireElvesNullifiesDamage()
    {
        string player1CardName = "Player1SpellCard";
        string player2CardName = "Player2MonsterCard";
        double player1CardDamage = 20;
        double player2CardDamage = 20;
        GlobalEnums.ElementType player1CardElementType = GlobalEnums.ElementType.Fire;
        GlobalEnums.MonsterType player1CardMonsterType = GlobalEnums.MonsterType.Dragon;
        GlobalEnums.ElementType player2CardElementType = GlobalEnums.ElementType.Fire;
        GlobalEnums.MonsterType player2CardMonsterType = GlobalEnums.MonsterType.FireElve;
        double dragonDamageAgainstFireElve = 0;

        MonsterCard player1MonsterCard = new MonsterCard(player1CardName, player1CardMonsterType, player1CardDamage, player1CardElementType);
        MonsterCard player2MonsterCard = new MonsterCard(player2CardName, player2CardMonsterType, player2CardDamage, player2CardElementType);

        Battle testBattle = new Battle(player1, player2);

        double calculatedDamage = testBattle.CalculateDamage(player1MonsterCard, player2MonsterCard, player1CardDamage);

        Assert.AreEqual(dragonDamageAgainstFireElve, calculatedDamage);
    }

    [TestMethod]
    public void CalculateDamage_MonsterlVsMonsterDamage_AttackWithoutSynergyDamageDoesNotChange()
    {

        string player1CardName = "Player1SpellCard";
        string player2CardName = "Player2MonsterCard";
        double player1CardDamage = 20;
        double player2CardDamage = 20;
        GlobalEnums.ElementType player1CardElementType = GlobalEnums.ElementType.Fire;
        GlobalEnums.MonsterType player1CardMonsterType = GlobalEnums.MonsterType.Knight;
        GlobalEnums.ElementType player2CardElementType = GlobalEnums.ElementType.Fire;
        GlobalEnums.MonsterType player2CardMonsterType = GlobalEnums.MonsterType.FireElve;

        MonsterCard player1MonsterCard = new MonsterCard(player1CardName, player1CardMonsterType, player1CardDamage, player1CardElementType);
        MonsterCard player2MonsterCard = new MonsterCard(player2CardName, player2CardMonsterType, player2CardDamage, player2CardElementType);

        Battle testBattle = new Battle(player1, player2);

        double calculatedDamage = testBattle.CalculateDamage(player1MonsterCard, player2MonsterCard, player1CardDamage);

        Assert.AreEqual(player1CardDamage, calculatedDamage);
    }

    [TestMethod]
    public void CompareDamage_Player1Wins_ReturnsVictory()
    {
        int player1Damage = 20;
        int player2Damage = 10;

        Battle testBattle = new Battle(player1, player2);

        GlobalEnums.RoundResults result = testBattle.CompareDamage(player1Damage, player2Damage);
        GlobalEnums.RoundResults victory = GlobalEnums.RoundResults.Victory;

        Assert.AreEqual(result, victory);
    }

    [TestMethod]
    public void CompareDamage_Player1Draws_ReturnsDraw()
    {
        int player1Damage = 10;
        int player2Damage = 10;

        Battle testBattle = new Battle(player1, player2);

        GlobalEnums.RoundResults result = testBattle.CompareDamage(player1Damage, player2Damage);
        GlobalEnums.RoundResults draw = GlobalEnums.RoundResults.Draw;

        Assert.AreEqual(result, draw);
    }

    [TestMethod]
    public void CompareDamage_Player1Looses_ReturnsDefeat()
    {
        int player1Damage = 10;
        int player2Damage = 20;

        Battle testBattle = new Battle(player1, player2);

        GlobalEnums.RoundResults result = testBattle.CompareDamage(player1Damage, player2Damage);
        GlobalEnums.RoundResults defeat = GlobalEnums.RoundResults.Defeat;

        Assert.AreEqual(result, defeat);
    }
}

