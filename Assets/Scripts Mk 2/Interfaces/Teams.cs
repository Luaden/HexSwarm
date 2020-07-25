using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public enum Teams
{
    Player = 1 << 0,
    AI1 = 1 << 1,
    AI2 = 1 << 2,
    AI3 = 1 << 3,
    AI4 = 1 << 4,
    AI5 = 1 << 5,
    AI6 = 1 << 6,
    AI7 = 1 << 7,
    AI8 = 1 << 8,
    AI9 = 1 << 9,
    //AIS = AI1 | AI2 | AI3 | AI4 | AI5 | AI6 | AI7 | AI8 | AI9
}