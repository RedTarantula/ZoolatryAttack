using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPiggy : PlayerBase
{
    public override void StartLocalVariables()
    {
        ammoCarrying = Zoolatry.PIGGY_AMMO_MAX;
        speed = Zoolatry.PIGGY_SPEED_BASE;
        playerShootSpeed = Zoolatry.PIGGY_SHOOTING_SPEED;
        playerReloadTimer = Zoolatry.PIGGY_RELOAD_TIMER;
        magazineCapacity = magazineBullets = Zoolatry.PIGGY_MAGAZINE_CAPACITY;
        playerMaxhealth = Zoolatry.PIGGY_HP_MAX;
        health = playerMaxhealth / 2f;
    }
    public override void ShootProjectiles(Vector3 pos,Vector3 dir,float lag)
    {
        GameObject go = Instantiate(projectilePrefab,pos,Quaternion.identity) as GameObject;
        float spread = UnityEngine.Random.Range(-Zoolatry.PIGGY_SHOOTING_SPREAD,Zoolatry.PIGGY_SHOOTING_SPREAD);
        
        go.transform.forward = dir;
        //go.transform.Rotate(new Vector3(0f,0f,spread));
        go.GetComponent<Bullet>().InitializeBullet(Zoolatry.PIGGY_PROJECTILE_RANGE,Zoolatry.PIGGY_PROJECTILE_DAMAGE,Zoolatry.PIGGY_PROJECTILE_SPEED,Zoolatry.BULLET_TARGET.Enemies,lag);
    }
        public override void PickupReaction(Zoolatry.PICKUP_TYPE pickupType)
    {
        if (pickupType == Zoolatry.PICKUP_TYPE.Ammo)
        {
            SetAmmoInv(Zoolatry.PIGGY_AMMO_MAX);
        }
        else if (pickupType == Zoolatry.PICKUP_TYPE.Health)
        {
            HealPercent(Zoolatry.PICKUP_HEALTH_PERCENT);
        }
    }
}
