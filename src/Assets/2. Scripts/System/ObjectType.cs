using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectType
{
    CONNECTING_TOOL = 1 << 0,
    CUBE            = 1 << 1,
    BEARING         = 1 << 2,
    WHEEL           = 1 << 3,
    DRIVER_SEAT     = 1 << 4,
    ENGINE          = 1 << 5,
    SWITCH          = 1 << 6,
    CONTROLLER      = 1 << 7,
    PISTON          = 1 << 8,
    TIMER           = 1 << 9,
    LOGIC_GATE      = 1 << 10,
    BUTTON          = 1 << 11,
}

