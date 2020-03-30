using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChicky : PlayerBase
{
    public override void StartLocalVariables()
    {
        magazineBullets = Zoolatry.CHICKY_MAGAZINE_CAPACITY;
        ammoCarrying = Zoolatry.CHICKY_AMMO_MAX;
        speed = Zoolatry.CHICKY_SPEED_BASE;
    }
}
