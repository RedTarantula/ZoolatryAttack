using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPiggy : PlayerBase
{
    public override void StartLocalVariables()
    {
        magazineBullets = Zoolatry.PIGGY_MAGAZINE_CAPACITY;
        ammoCarrying = Zoolatry.PIGGY_AMMO_MAX;
        speed = Zoolatry.PIGGY_SPEED_BASE;
    }
}
