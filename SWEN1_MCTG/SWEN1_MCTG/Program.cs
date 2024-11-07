using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using SWEN1_MCTG.Classes;

namespace SWEN1_MCTG
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // Test Coin class
            Coin testCoin = new Coin(GlobalEnums.CoinType.Bronze);
            Console.WriteLine("------- COIN TEST ------");
            Console.WriteLine("Coin type: " + testCoin.CoinType);
            Console.WriteLine("Coin value: " + testCoin.Value);

            Console.WriteLine("------- COIN 2ND TEST ------");
            testCoin.CoinType = GlobalEnums.CoinType.Silver;
            Console.WriteLine("Coin type: " + testCoin.CoinType);
            Console.WriteLine("Coin value: " + testCoin.Value);

            // Test Spell Card class
            SpellCard testSpellCard = new SpellCard("Test SpellCard", GlobalEnums.ElementType.Normal, 10);
            Console.WriteLine("------- SPELLCARD TEST ------");
            testSpellCard.PrintInformation();
            testSpellCard.Action();

            // Test Monster Card class
            Console.WriteLine("------- MONSTERCARD TEST ------");
            MonsterCard testMonsterCard = new MonsterCard("Test MonsterCard", GlobalEnums.ElementType.Fire, 20, 30);
            testMonsterCard.PrintInformation();
            testMonsterCard.Action();

            // Test User class
            User testUser = new User("Samuel", "Passwort");
            Console.WriteLine("------- USER TEST ------");
            testUser.UserStack.AddCardToStack(testSpellCard);
            testUser.UserStack.AddCardToStack((testMonsterCard));
            testUser.UserCoinPurse.AddCoin(testCoin);
            testUser.UserCoinPurse.AddCoin(new Coin(GlobalEnums.CoinType.Platinum));
            testUser.PrintUser();
            Console.WriteLine("------- USER 2ND TEST ------");
            testUser.UserStack.RemoveCardFromStack(testSpellCard.Name);
            testUser.UserCoinPurse.RemoveCoin(testCoin);
            testUser.PrintUser();


        }
    }
}
