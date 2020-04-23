using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChicky : PlayerBase
{
    public override void StartLocalVariables()
    {
        character = Zoolatry.PLAYER_CHARACTER.Chicky;
        ammoCarrying = Zoolatry.CHICKY_AMMO_MAX;
        speed = Zoolatry.CHICKY_SPEED_BASE;
        playerShootSpeed = Zoolatry.CHICKY_SHOOTING_SPEED;
        playerReloadTimer = Zoolatry.CHICKY_RELOAD_TIMER;
        magazineCapacity = magazineBullets = Zoolatry.CHICKY_MAGAZINE_CAPACITY;
        playerMaxhealth = Zoolatry.CHICKY_HP_MAX;
        health = playerMaxhealth / 2f;
    }

    public override void ShootProjectiles(Vector3 pos,Vector3 dir,float lag)
    {
        GameObject go = Instantiate(projectilePrefab,pos,Quaternion.identity) as GameObject;
        float spread = UnityEngine.Random.Range(-Zoolatry.CHICKY_SHOOTING_SPREAD,Zoolatry.CHICKY_SHOOTING_SPREAD);

        go.transform.forward = dir;
        //go.transform.Rotate(new Vector3(0f,0f,spread));
        go.GetComponent<Bullet>().InitializeBullet(Zoolatry.CHICKY_PROJECTILE_RANGE,Zoolatry.CHICKY_PROJECTILE_DAMAGE,Zoolatry.CHICKY_PROJECTILE_SPEED,Zoolatry.BULLET_TARGET.Enemies,lag);
    }
    public override void PickupReaction(Zoolatry.PICKUP_TYPE pickupType)
    {

        if (pickupType == Zoolatry.PICKUP_TYPE.Ammo)
        {
            SetAmmoInv(Zoolatry.CHICKY_AMMO_MAX);

        }
        else if (pickupType == Zoolatry.PICKUP_TYPE.Health)
        {
            HealPercent(Zoolatry.PICKUP_HEALTH_PERCENT);
        }
    }
}
