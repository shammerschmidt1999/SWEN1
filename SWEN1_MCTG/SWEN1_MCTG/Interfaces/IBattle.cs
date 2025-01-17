using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWEN1_MCTG.Classes;
using static SWEN1_MCTG.GlobalEnums;

namespace SWEN1_MCTG.Interfaces
{
    public interface IBattle
    { 
        double CalculateDamage(Card card1, Card card2, double card1Damage);
        double CalculateSpellVsSpellDamage(SpellCard card1, ElementType card2ElementType, double card1Damage);
        double CalculateSpellVsMonsterDamage(SpellCard card1, MonsterCard card2, double card1Damage);
        double CalculateMonsterVsMonsterDamage(MonsterCard card1, MonsterCard card2, double card1Damage);
        RoundResults CompareDamage(double player1Damage, double player2Damage);
    }
}
