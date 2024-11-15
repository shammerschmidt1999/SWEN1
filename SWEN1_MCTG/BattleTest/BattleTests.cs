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
    [TestMethod]
    public void Constructor_CreateBattle_PropertiesAreSetCorrectly()
    {
        string player1Username = "Player1Name";
        string player1Password = "Player1Password";
        string player2Username = "Player2Name";
        string player2Password = "Player2Password";
        int roundCount = 0;
        int maxRounds = 100;

        User player1 = new User(player1Username, player1Password);
        User player2 = new User(player2Username, player2Password);

        Battle testBattle = new Battle(player1, player2);

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
        string player1Username = "Player1Name";
        string player1Password = "Player1Password";
        string player2Username = "Player2Name";
        string player2Password = "Player2Password";
        string player1CardName = "Player1SpellCard";
        string player2CardName = "Player2SpellCard";
        double player1CardDamage = 20;
        double player2CardDamage = 20;
        GlobalEnums.ElementType player1CardElementType = GlobalEnums.ElementType.Water;
        GlobalEnums.ElementType player2CardElementType = GlobalEnums.ElementType.Fire;

        User player1 = new User(player1Username, player1Password);
        User player2 = new User(player2Username, player2Password);
        SpellCard player1SpellCard = new SpellCard(player1CardName, player1CardDamage, player1CardElementType);
        SpellCard player2SpellCard = new SpellCard(player2CardName, player2CardDamage, player2CardElementType);

        Battle testBattle = new Battle(player1, player2);

        double calculatedDamage = testBattle.CalculateDamage(player1SpellCard, player2SpellCard, player1CardDamage);

        Assert.AreEqual(player1CardDamage * 2, calculatedDamage);
    }

    [TestMethod]
    public void CalculateDamage_SpellVsSpellDamage_SpellAttackAgainstNotEffectiveElementHalvesCardDamage()
    {
        string player1Username = "Player1Name";
        string player1Password = "Player1Password";
        string player2Username = "Player2Name";
        string player2Password = "Player2Password";
        string player1CardName = "Player1SpellCard";
        string player2CardName = "Player2SpellCard";
        double player1CardDamage = 20;
        double player2CardDamage = 20;
        GlobalEnums.ElementType player1CardElementType = GlobalEnums.ElementType.Fire;
        GlobalEnums.ElementType player2CardElementType = GlobalEnums.ElementType.Water;

        User player1 = new User(player1Username, player1Password);
        User player2 = new User(player2Username, player2Password);
        SpellCard player1SpellCard = new SpellCard(player1CardName, player1CardDamage, player1CardElementType);
        SpellCard player2SpellCard = new SpellCard(player2CardName, player2CardDamage, player2CardElementType);

        Battle testBattle = new Battle(player1, player2);

        double calculatedDamage = testBattle.CalculateDamage(player1SpellCard, player2SpellCard, player1CardDamage);

        Assert.AreEqual(player1CardDamage / 2, calculatedDamage);
    }

    [TestMethod]
    public void CalculateDamage_SpellVsSpellDamage_SpellAttackAgainstSameElementDoesNotChangeDamage()
    {
        string player1Username = "Player1Name";
        string player1Password = "Player1Password";
        string player2Username = "Player2Name";
        string player2Password = "Player2Password";
        string player1CardName = "Player1SpellCard";
        string player2CardName = "Player2SpellCard";
        double player1CardDamage = 20;
        double player2CardDamage = 20;
        GlobalEnums.ElementType player1CardElementType = GlobalEnums.ElementType.Fire;
        GlobalEnums.ElementType player2CardElementType = GlobalEnums.ElementType.Fire;

        User player1 = new User(player1Username, player1Password);
        User player2 = new User(player2Username, player2Password);
        SpellCard player1SpellCard = new SpellCard(player1CardName, player1CardDamage, player1CardElementType);
        SpellCard player2SpellCard = new SpellCard(player2CardName, player2CardDamage, player2CardElementType);

        Battle testBattle = new Battle(player1, player2);

        double calculatedDamage = testBattle.CalculateDamage(player1SpellCard, player2SpellCard, player1CardDamage);

        Assert.AreEqual(player1CardDamage, calculatedDamage);
    }

    [TestMethod]
    public void CalculateDamage_SpellVsMonsterDamage_SpellAttackAgainstKrakenNullifiesDamage()
    {
        string player1Username = "Player1Name";
        string player1Password = "Player1Password";
        string player2Username = "Player2Name";
        string player2Password = "Player2Password";
        string player1CardName = "Player1SpellCard";
        string player2CardName = "Player2MonsterCard";
        double player1CardDamage = 20;
        double player2CardDamage = 20;
        GlobalEnums.ElementType player1CardElementType = GlobalEnums.ElementType.Fire;
        GlobalEnums.ElementType player2CardElementType = GlobalEnums.ElementType.Fire;
        GlobalEnums.MonsterType player2CardMonsterType = GlobalEnums.MonsterType.Kraken;
        double spellDamageAgainstKraken = 0;

        User player1 = new User(player1Username, player1Password);
        User player2 = new User(player2Username, player2Password);
        SpellCard player1SpellCard = new SpellCard(player1CardName, player1CardDamage, player1CardElementType);
        MonsterCard player2MonsterCard = new MonsterCard(player2CardName, player2CardMonsterType, player2CardDamage, player2CardElementType);

        Battle testBattle = new Battle(player1, player2);

        double calculatedDamage = testBattle.CalculateDamage(player1SpellCard, player2MonsterCard, player1CardDamage);

        Assert.AreEqual(spellDamageAgainstKraken, calculatedDamage);
    }

    [TestMethod]
    public void CalculateDamage_SpellVsMonsterDamage_SpellAttackAgainstMonsterWithEffectiveElementDoublesDamage()
    {
        string player1Username = "Player1Name";
        string player1Password = "Player1Password";
        string player2Username = "Player2Name";
        string player2Password = "Player2Password";
        string player1CardName = "Player1SpellCard";
        string player2CardName = "Player2MonsterCard";
        double player1CardDamage = 20;
        double player2CardDamage = 20;
        GlobalEnums.ElementType player1CardElementType = GlobalEnums.ElementType.Water;
        GlobalEnums.ElementType player2CardElementType = GlobalEnums.ElementType.Fire;
        GlobalEnums.MonsterType player2CardMonsterType = GlobalEnums.MonsterType.Goblin;

        User player1 = new User(player1Username, player1Password);
        User player2 = new User(player2Username, player2Password);
        SpellCard player1SpellCard = new SpellCard(player1CardName, player1CardDamage, player1CardElementType);
        MonsterCard player2MonsterCard = new MonsterCard(player2CardName, player2CardMonsterType, player2CardDamage, player2CardElementType);

        Battle testBattle = new Battle(player1, player2);

        double calculatedDamage = testBattle.CalculateDamage(player1SpellCard, player2MonsterCard, player1CardDamage);

        Assert.AreEqual(player1CardDamage * 2, calculatedDamage);
    }

    [TestMethod]
    public void CalculateDamage_SpellVsMonsterDamage_SpellAttackAgainstSameElementDoesNotChangeDamage()
    {
        string player1Username = "Player1Name";
        string player1Password = "Player1Password";
        string player2Username = "Player2Name";
        string player2Password = "Player2Password";
        string player1CardName = "Player1SpellCard";
        string player2CardName = "Player2MonsterCard";
        double player1CardDamage = 20;
        double player2CardDamage = 20;
        GlobalEnums.ElementType player1CardElementType = GlobalEnums.ElementType.Fire;
        GlobalEnums.ElementType player2CardElementType = GlobalEnums.ElementType.Fire;
        GlobalEnums.MonsterType player2CardMonsterType = GlobalEnums.MonsterType.Goblin;

        User player1 = new User(player1Username, player1Password);
        User player2 = new User(player2Username, player2Password);
        SpellCard player1SpellCard = new SpellCard(player1CardName, player1CardDamage, player1CardElementType);
        MonsterCard player2MonsterCard = new MonsterCard(player2CardName, player2CardMonsterType, player2CardDamage, player2CardElementType);

        Battle testBattle = new Battle(player1, player2);

        double calculatedDamage = testBattle.CalculateDamage(player1SpellCard, player2MonsterCard, player1CardDamage);

        Assert.AreEqual(player1CardDamage, calculatedDamage);
    }

    [TestMethod]
    public void CalculateDamage_MonsterlVsMonsterDamage_GoblinAttackAgainstDragonNullifiesDamage()
    {
        string player1Username = "Player1Name";
        string player1Password = "Player1Password";
        string player2Username = "Player2Name";
        string player2Password = "Player2Password";
        string player1CardName = "Player1SpellCard";
        string player2CardName = "Player2MonsterCard";
        double player1CardDamage = 20;
        double player2CardDamage = 20;
        GlobalEnums.ElementType player1CardElementType = GlobalEnums.ElementType.Fire;
        GlobalEnums.MonsterType player1CardMonsterType = GlobalEnums.MonsterType.Goblin;
        GlobalEnums.ElementType player2CardElementType = GlobalEnums.ElementType.Fire;
        GlobalEnums.MonsterType player2CardMonsterType = GlobalEnums.MonsterType.Dragon;
        double goblinDamageAgainstDragon = 0;

        User player1 = new User(player1Username, player1Password);
        User player2 = new User(player2Username, player2Password);
        MonsterCard player1MonsterCard = new MonsterCard(player1CardName, player1CardMonsterType, player1CardDamage, player1CardElementType);
        MonsterCard player2MonsterCard = new MonsterCard(player2CardName, player2CardMonsterType, player2CardDamage, player2CardElementType);

        Battle testBattle = new Battle(player1, player2);

        double calculatedDamage = testBattle.CalculateDamage(player1MonsterCard, player2MonsterCard, player1CardDamage);

        Assert.AreEqual(goblinDamageAgainstDragon, calculatedDamage);
    }

    [TestMethod]
    public void CalculateDamage_MonsterlVsMonsterDamage_OrkAttackAgainstWizardNullifiesDamage()
    {
        string player1Username = "Player1Name";
        string player1Password = "Player1Password";
        string player2Username = "Player2Name";
        string player2Password = "Player2Password";
        string player1CardName = "Player1SpellCard";
        string player2CardName = "Player2MonsterCard";
        double player1CardDamage = 20;
        double player2CardDamage = 20;
        GlobalEnums.ElementType player1CardElementType = GlobalEnums.ElementType.Fire;
        GlobalEnums.MonsterType player1CardMonsterType = GlobalEnums.MonsterType.Ork;
        GlobalEnums.ElementType player2CardElementType = GlobalEnums.ElementType.Fire;
        GlobalEnums.MonsterType player2CardMonsterType = GlobalEnums.MonsterType.Wizard;
        double orkDamageAgainstWizard = 0;

        User player1 = new User(player1Username, player1Password);
        User player2 = new User(player2Username, player2Password);
        MonsterCard player1MonsterCard = new MonsterCard(player1CardName, player1CardMonsterType, player1CardDamage, player1CardElementType);
        MonsterCard player2MonsterCard = new MonsterCard(player2CardName, player2CardMonsterType, player2CardDamage, player2CardElementType);

        Battle testBattle = new Battle(player1, player2);

        double calculatedDamage = testBattle.CalculateDamage(player1MonsterCard, player2MonsterCard, player1CardDamage);

        Assert.AreEqual(orkDamageAgainstWizard, calculatedDamage);
    }

    [TestMethod]
    public void CalculateDamage_MonsterlVsMonsterDamage_DragonAttackAgainstFireElvesNullifiesDamage()
    {
        string player1Username = "Player1Name";
        string player1Password = "Player1Password";
        string player2Username = "Player2Name";
        string player2Password = "Player2Password";
        string player1CardName = "Player1SpellCard";
        string player2CardName = "Player2MonsterCard";
        double player1CardDamage = 20;
        double player2CardDamage = 20;
        GlobalEnums.ElementType player1CardElementType = GlobalEnums.ElementType.Fire;
        GlobalEnums.MonsterType player1CardMonsterType = GlobalEnums.MonsterType.Dragon;
        GlobalEnums.ElementType player2CardElementType = GlobalEnums.ElementType.Fire;
        GlobalEnums.MonsterType player2CardMonsterType = GlobalEnums.MonsterType.FireElve;
        double dragonDamageAgainstFireElve = 0;

        User player1 = new User(player1Username, player1Password);
        User player2 = new User(player2Username, player2Password);
        MonsterCard player1MonsterCard = new MonsterCard(player1CardName, player1CardMonsterType, player1CardDamage, player1CardElementType);
        MonsterCard player2MonsterCard = new MonsterCard(player2CardName, player2CardMonsterType, player2CardDamage, player2CardElementType);

        Battle testBattle = new Battle(player1, player2);

        double calculatedDamage = testBattle.CalculateDamage(player1MonsterCard, player2MonsterCard, player1CardDamage);

        Assert.AreEqual(dragonDamageAgainstFireElve, calculatedDamage);
    }

    [TestMethod]
    public void CalculateDamage_MonsterlVsMonsterDamage_AttackWithoutSynergyDamageDoesNotChange()
    {
        string player1Username = "Player1Name";
        string player1Password = "Player1Password";
        string player2Username = "Player2Name";
        string player2Password = "Player2Password";
        string player1CardName = "Player1SpellCard";
        string player2CardName = "Player2MonsterCard";
        double player1CardDamage = 20;
        double player2CardDamage = 20;
        GlobalEnums.ElementType player1CardElementType = GlobalEnums.ElementType.Fire;
        GlobalEnums.MonsterType player1CardMonsterType = GlobalEnums.MonsterType.Knight;
        GlobalEnums.ElementType player2CardElementType = GlobalEnums.ElementType.Fire;
        GlobalEnums.MonsterType player2CardMonsterType = GlobalEnums.MonsterType.FireElve;

        User player1 = new User(player1Username, player1Password);
        User player2 = new User(player2Username, player2Password);
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
        string player1Username = "Player1Name";
        string player1Password = "Player1Password";
        string player2Username = "Player2Name";
        string player2Password = "Player2Password";

        User player1 = new User(player1Username, player1Password);
        User player2 = new User(player2Username, player2Password);

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
        string player1Username = "Player1Name";
        string player1Password = "Player1Password";
        string player2Username = "Player2Name";
        string player2Password = "Player2Password";

        User player1 = new User(player1Username, player1Password);
        User player2 = new User(player2Username, player2Password);

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
        string player1Username = "Player1Name";
        string player1Password = "Player1Password";
        string player2Username = "Player2Name";
        string player2Password = "Player2Password";

        User player1 = new User(player1Username, player1Password);
        User player2 = new User(player2Username, player2Password);

        Battle testBattle = new Battle(player1, player2);

        GlobalEnums.RoundResults result = testBattle.CompareDamage(player1Damage, player2Damage);
        GlobalEnums.RoundResults defeat = GlobalEnums.RoundResults.Defeat;

        Assert.AreEqual(result, defeat);
    }
}

