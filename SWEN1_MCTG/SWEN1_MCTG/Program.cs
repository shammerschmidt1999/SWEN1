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
            Coin bronzeTestCoin = new Coin(GlobalEnums.CoinType.Bronze);
            Console.WriteLine("------- COIN TEST ------");
            Console.WriteLine("Coin type: " + bronzeTestCoin.CoinType);
            Console.WriteLine("Coin value: " + bronzeTestCoin.Value);

            Console.WriteLine("\n------- COIN 2ND TEST ------");
            Coin silverTestCoin = new Coin(GlobalEnums.CoinType.Silver);
            Console.WriteLine("Coin type: " + silverTestCoin.CoinType);
            Console.WriteLine("Coin value: " + silverTestCoin.Value);

            // Test Spell Card class
            SpellCard testSpellCard = new SpellCard("Test SpellCard", GlobalEnums.ElementType.Normal, 10);
            Console.WriteLine("\n------- SPELLCARD TEST ------");
            testSpellCard.PrintInformation();
            testSpellCard.Action();

            // Test Monster Card class
            Console.WriteLine("\n------- MONSTERCARD TEST ------");
            MonsterCard testMonsterCard = new MonsterCard("Test MonsterCard", GlobalEnums.MonsterType.Goblin, 20);
            testMonsterCard.PrintInformation();
            testMonsterCard.Action();

            // Test User class
            User testUser = new User("Samuel", "Passwort");
            Console.WriteLine("\n------- USER TEST ------");
            testUser.UserStack.AddCardToStack(testSpellCard);
            testUser.UserStack.AddCardToStack((testMonsterCard));
            testUser.UserCoinPurse.AddCoin(bronzeTestCoin);
            testUser.UserCoinPurse.AddCoin(new Coin(GlobalEnums.CoinType.Platinum));
            testUser.PrintUser();
            Console.WriteLine("\n------- USER 2ND TEST ------");
            testUser.UserStack.RemoveCardFromStack(testSpellCard.Name);
            testUser.UserCoinPurse.RemoveCoin(bronzeTestCoin);
            testUser.PrintUser();


        }
    }
}
