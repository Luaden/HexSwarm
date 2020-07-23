using System;

[Flags]
public enum Abilitys
{
    empty = 0,
    Spawner = 1 << 0,
    Tank = 1 << 1,
    Heli = 1 << 2,
    Infantry = 1 << 3,
    Nanos = 1 << 4,
    UberNano= 1 << 5,
    UberTank= 1 << 6,
    Opt1    = 1 << 7,
    Opt2    = 1 << 8,
    Opt3    = 1 << 9,
    Opt4    = 1 << 10,
    Opt5    = 1 << 11,
    Opt6    = 1 << 12,

}