using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN1_MCTG.Interfaces
{
    public interface ICard
    {
        GlobalEnums.ElementType Element { get; set; }
        string Name { get; set; }
        double Damage { get; set; }
        void Action();
        void PrintInformation();
    }

}
