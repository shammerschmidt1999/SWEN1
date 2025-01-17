using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWEN1_MCTG.Classes;

namespace SWEN1_MCTG.Interfaces
{
    public interface IUser
    {
        public void ChangeUsername(string username);
        public void ChangePassword(string password);
        public void ChangeUserDeck(Stack newDeck);
        public void ApplyBattleResult(GlobalEnums.RoundResults result);

    }
}
