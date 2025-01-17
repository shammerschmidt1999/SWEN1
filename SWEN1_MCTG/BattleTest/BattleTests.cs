using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWEN1_MCTG;
using SWEN1_MCTG.Classes;
using static SWEN1_MCTG.GlobalEnums;

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
        User userPlayer1 = new User(player1Username, player1Password);
        User userPlayer2 = new User(player2Username, player2Password);

        player1 = userPlayer1;
        player2 = userPlayer2;
    }

    [TestMethod]
    public void CalculateDamage_SpellVsSpellDamage_SpellAttackAgainstEffectiveElementDoublesCardDamage()
    {
        string player1CardName = "Player1SpellCard";
        string player2CardName = "Player2SpellCard";
        double player1CardDamage = 20;
        double player2CardDamage = 20;
        ElementType player1CardElementType = ElementType.Water;
        ElementType player2CardElementType = ElementType.Fire;

        SpellCard player1SpellCard = new SpellCard(player1CardName, player1CardDamage, player1CardElementType);
        SpellCard player2SpellCard = new SpellCard(player2CardName, player2CardDamage, player2CardElementType);

        Battle testBattle = new Battle(player1, player2);

        double calculatedDamage = testBattle.CalculateDamageTest(player1SpellCard, player2SpellCard, player1CardDamage);

        Assert.AreEqual(player1CardDamage * 2, calculatedDamage);
    }

    [TestMethod]
    public void CalculateDamage_SpellVsSpellDamage_SpellAttackAgainstNotEffectiveElementHalvesCardDamage()
    {
        string player1CardName = "Player1SpellCard";
        string player2CardName = "Player2SpellCard";
        double player1CardDamage = 20;
        double player2CardDamage = 20;
        ElementType player1CardElementType = ElementType.Fire;
        ElementType player2CardElementType = ElementType.Water;

        SpellCard player1SpellCard = new SpellCard(player1CardName, player1CardDamage, player1CardElementType);
        SpellCard player2SpellCard = new SpellCard(player2CardName, player2CardDamage, player2CardElementType);

        Battle testBattle = new Battle(player1, player2);

        double calculatedDamage = testBattle.CalculateDamageTest(player1SpellCard, player2SpellCard, player1CardDamage);

        Assert.AreEqual(player1CardDamage / 2, calculatedDamage);
    }

    [TestMethod]
    public void CalculateDamage_SpellVsSpellDamage_SpellAttackAgainstSameElementDoesNotChangeDamage()
    {
        string player1CardName = "Player1SpellCard";
        string player2CardName = "Player2SpellCard";
        double player1CardDamage = 20;
        double player2CardDamage = 20;
        ElementType player1CardElementType = ElementType.Fire;
        ElementType player2CardElementType = ElementType.Fire;

        SpellCard player1SpellCard = new SpellCard(player1CardName, player1CardDamage, player1CardElementType);
        SpellCard player2SpellCard = new SpellCard(player2CardName, player2CardDamage, player2CardElementType);

        Battle testBattle = new Battle(player1, player2);

        double calculatedDamage = testBattle.CalculateDamageTest(player1SpellCard, player2SpellCard, player1CardDamage);

        Assert.AreEqual(player1CardDamage, calculatedDamage);
    }

    [TestMethod]
    public void CalculateDamage_SpellVsMonsterDamage_SpellAttackAgainstKrakenNullifiesDamage()
    {
        string player1CardName = "Player1SpellCard";
        string player2CardName = "Player2MonsterCard";
        double player1CardDamage = 20;
        double player2CardDamage = 20;
        ElementType player1CardElementType = ElementType.Fire;
        ElementType player2CardElementType = ElementType.Fire;
        MonsterType player2CardMonsterType = MonsterType.Kraken;
        double spellDamageAgainstKraken = 0;

        SpellCard player1SpellCard = new SpellCard(player1CardName, player1CardDamage, player1CardElementType);
        MonsterCard player2MonsterCard = new MonsterCard(player2CardName, player2CardMonsterType, player2CardDamage, player2CardElementType);

        Battle testBattle = new Battle(player1, player2);

        double calculatedDamage = testBattle.CalculateDamageTest(player1SpellCard, player2MonsterCard, player1CardDamage);

        Assert.AreEqual(spellDamageAgainstKraken, calculatedDamage);
    }

    [TestMethod]
    public void CalculateDamage_SpellVsMonsterDamage_SpellAttackAgainstMonsterWithEffectiveElementDoublesDamage()
    {
        string player1CardName = "Player1SpellCard";
        string player2CardName = "Player2MonsterCard";
        double player1CardDamage = 20;
        double player2CardDamage = 20;
        ElementType player1CardElementType = ElementType.Water;
        ElementType player2CardElementType = ElementType.Fire;
        MonsterType player2CardMonsterType = MonsterType.Goblin;

        SpellCard player1SpellCard = new SpellCard(player1CardName, player1CardDamage, player1CardElementType);
        MonsterCard player2MonsterCard = new MonsterCard(player2CardName, player2CardMonsterType, player2CardDamage, player2CardElementType);

        Battle testBattle = new Battle(player1, player2);

        double calculatedDamage = testBattle.CalculateDamageTest(player1SpellCard, player2MonsterCard, player1CardDamage);

        Assert.AreEqual(player1CardDamage * 2, calculatedDamage);
    }

    [TestMethod]
    public void CalculateDamage_SpellVsMonsterDamage_SpellAttackAgainstSameElementDoesNotChangeDamage()
    {
        string player1CardName = "Player1SpellCard";
        string player2CardName = "Player2MonsterCard";
        double player1CardDamage = 20;
        double player2CardDamage = 20;
        ElementType player1CardElementType = ElementType.Fire;
        ElementType player2CardElementType = ElementType.Fire;
        MonsterType player2CardMonsterType = MonsterType.Goblin;

        SpellCard player1SpellCard = new SpellCard(player1CardName, player1CardDamage, player1CardElementType);
        MonsterCard player2MonsterCard = new MonsterCard(player2CardName, player2CardMonsterType, player2CardDamage, player2CardElementType);

        Battle testBattle = new Battle(player1, player2);

        double calculatedDamage = testBattle.CalculateDamageTest(player1SpellCard, player2MonsterCard, player1CardDamage);

        Assert.AreEqual(player1CardDamage, calculatedDamage);
    }

    [TestMethod]
    public void CalculateDamage_MonsterVsMonsterDamage_GoblinAttackAgainstDragonNullifiesDamage()
    {
        string player1CardName = "Player1SpellCard";
        string player2CardName = "Player2MonsterCard";
        double player1CardDamage = 20;
        double player2CardDamage = 20;
        ElementType player1CardElementType = ElementType.Fire;
        MonsterType player1CardMonsterType = MonsterType.Goblin;
        ElementType player2CardElementType = ElementType.Fire;
        MonsterType player2CardMonsterType = MonsterType.Dragon;
        double goblinDamageAgainstDragon = 0;

        MonsterCard player1MonsterCard = new MonsterCard(player1CardName, player1CardMonsterType, player1CardDamage, player1CardElementType);
        MonsterCard player2MonsterCard = new MonsterCard(player2CardName, player2CardMonsterType, player2CardDamage, player2CardElementType);

        Battle testBattle = new Battle(player1, player2);

        double calculatedDamage = testBattle.CalculateDamageTest(player1MonsterCard, player2MonsterCard, player1CardDamage);

        Assert.AreEqual(goblinDamageAgainstDragon, calculatedDamage);
    }

    [TestMethod]
    public void CalculateDamage_MonsterVsMonsterDamage_OrkAttackAgainstWizardNullifiesDamage()
    {
        string player1CardName = "Player1SpellCard";
        string player2CardName = "Player2MonsterCard";
        double player1CardDamage = 20;
        double player2CardDamage = 20;
        ElementType player1CardElementType = ElementType.Fire;
        MonsterType player1CardMonsterType = MonsterType.Ork;
        ElementType player2CardElementType = ElementType.Fire;
        MonsterType player2CardMonsterType = MonsterType.Wizard;
        double orkDamageAgainstWizard = 0;

        MonsterCard player1MonsterCard = new MonsterCard(player1CardName, player1CardMonsterType, player1CardDamage, player1CardElementType);
        MonsterCard player2MonsterCard = new MonsterCard(player2CardName, player2CardMonsterType, player2CardDamage, player2CardElementType);

        Battle testBattle = new Battle(player1, player2);

        double calculatedDamage = testBattle.CalculateDamageTest(player1MonsterCard, player2MonsterCard, player1CardDamage);

        Assert.AreEqual(orkDamageAgainstWizard, calculatedDamage);
    }

    [TestMethod]
    public void CalculateDamage_MonsterVsMonsterDamage_DragonAttackAgainstFireElvesNullifiesDamage()
    {
        string player1CardName = "Player1SpellCard";
        string player2CardName = "Player2MonsterCard";
        double player1CardDamage = 20;
        double player2CardDamage = 20;
        ElementType player1CardElementType = ElementType.Fire;
        MonsterType player1CardMonsterType = MonsterType.Dragon;
        ElementType player2CardElementType = ElementType.Fire;
        MonsterType player2CardMonsterType = MonsterType.FireElve;
        double dragonDamageAgainstFireElve = 0;

        MonsterCard player1MonsterCard = new MonsterCard(player1CardName, player1CardMonsterType, player1CardDamage, player1CardElementType);
        MonsterCard player2MonsterCard = new MonsterCard(player2CardName, player2CardMonsterType, player2CardDamage, player2CardElementType);

        Battle testBattle = new Battle(player1, player2);

        double calculatedDamage = testBattle.CalculateDamageTest(player1MonsterCard, player2MonsterCard, player1CardDamage);

        Assert.AreEqual(dragonDamageAgainstFireElve, calculatedDamage);
    }

    [TestMethod]
    public void CalculateDamage_MonsterVsMonsterDamage_AttackWithoutSynergyDamageDoesNotChange()
    {

        string player1CardName = "Player1SpellCard";
        string player2CardName = "Player2MonsterCard";
        double player1CardDamage = 20;
        double player2CardDamage = 20;
        ElementType player1CardElementType = ElementType.Fire;
        MonsterType player1CardMonsterType = MonsterType.Knight;
        ElementType player2CardElementType = ElementType.Fire;
        MonsterType player2CardMonsterType = MonsterType.FireElve;

        MonsterCard player1MonsterCard = new MonsterCard(player1CardName, player1CardMonsterType, player1CardDamage, player1CardElementType);
        MonsterCard player2MonsterCard = new MonsterCard(player2CardName, player2CardMonsterType, player2CardDamage, player2CardElementType);

        Battle testBattle = new Battle(player1, player2);

        double calculatedDamage = testBattle.CalculateDamageTest(player1MonsterCard, player2MonsterCard, player1CardDamage);

        Assert.AreEqual(player1CardDamage, calculatedDamage);
    }

    [TestMethod]
    public void CompareDamage_Player1Wins_ReturnsVictory()
    {
        int player1Damage = 20;
        int player2Damage = 10;

        Battle testBattle = new Battle(player1, player2);

        RoundResults result = testBattle.CompareDamageTest(player1Damage, player2Damage);
        RoundResults victory = RoundResults.Victory;

        Assert.AreEqual(result, victory);
    }

    [TestMethod]
    public void CompareDamage_Player1Draws_ReturnsDraw()
    {
        int player1Damage = 10;
        int player2Damage = 10;

        Battle testBattle = new Battle(player1, player2);

        RoundResults result = testBattle.CompareDamageTest(player1Damage, player2Damage);
        RoundResults draw = RoundResults.Draw;

        Assert.AreEqual(result, draw);
    }

    [TestMethod]
    public void CompareDamage_Player1Looses_ReturnsDefeat()
    {
        int player1Damage = 10;
        int player2Damage = 20;

        Battle testBattle = new Battle(player1, player2);

        RoundResults result = testBattle.CompareDamageTest(player1Damage, player2Damage);
        RoundResults defeat = RoundResults.Defeat;

        Assert.AreEqual(result, defeat);
    }
}

