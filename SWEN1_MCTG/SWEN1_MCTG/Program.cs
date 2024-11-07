using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace SWEN1_MCTG
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // Test Coin class
            Coin testCoin = new Coin(GlobalEnums.CoinType.Platinum);
            Console.WriteLine("------- COIN TEST ------");
            Console.WriteLine(testCoin.CoinType);
            Console.WriteLine(testCoin.Value);

            // Test Spell Card class
            Card testSpellCard = new SpellCard("Test SpellCard", GlobalEnums.ElementType.Normal, 10);
            Console.WriteLine("------- SPELLCARD TEST ------");
            Console.WriteLine(testSpellCard.Name);
            Console.WriteLine(testSpellCard.Element);
            Console.WriteLine(testSpellCard.Damage);
            testSpellCard.Action();

            // Test Monster Card class
            Console.WriteLine("------- MONSTERCARD TEST ------");
            Card testMonsterCard = new MonsterCard("Test MonsterCard", GlobalEnums.ElementType.Fire, 20);
            Console.WriteLine(testMonsterCard.Name);
            Console.WriteLine(testMonsterCard.Element);
            Console.WriteLine(testMonsterCard.Damage);
            testMonsterCard.Action();

        }
    }
}
