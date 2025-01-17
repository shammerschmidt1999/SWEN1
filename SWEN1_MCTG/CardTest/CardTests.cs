using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWEN1_MCTG;
using SWEN1_MCTG.Classes;

namespace CardTest;
using static SWEN1_MCTG.GlobalEnums;

[TestClass]
public class CardTests
{
    [TestMethod]
    public void Constructor_CreateSpellCard_NameDamageElementTypeEqualToParameters()
    {
        string name = "TestCardName";
        double damage = 10;
        ElementType elementType = ElementType.Fire;

        Card TestSpellCard = new SpellCard(name, damage, elementType);

        Assert.AreEqual(name, TestSpellCard.Name);
        Assert.AreEqual(damage, TestSpellCard.Damage);
        Assert.AreEqual(elementType, TestSpellCard.ElementType);
    }

    [TestMethod]
    public void Constructor_CreateMonsterCard_NameDamageElementTypeMonsterTypeEqualToParameters()
    {
        string name = "TestCardName";
        double damage = 10;
        ElementType elementType = ElementType.Fire;
        MonsterType monsterType = MonsterType.Ork;

        MonsterCard TestMonsterCard = new MonsterCard(name, monsterType, damage, elementType);

        Assert.AreEqual(name, TestMonsterCard.Name);
        Assert.AreEqual(damage, TestMonsterCard.Damage);
        Assert.AreEqual(elementType, TestMonsterCard.ElementType);
        Assert.AreEqual(monsterType, TestMonsterCard.MonsterType);
    }

}
