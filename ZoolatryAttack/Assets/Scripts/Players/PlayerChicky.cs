using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChicky : PlayerBase
{
    public override void StartLocalVariables()
    {
        ammoCarrying = Zoolatry.CHICKY_AMMO_MAX;
        speed = Zoolatry.CHICKY_SPEED_BASE;
        playerShootSpeed = Zoolatry.CHICKY_SHOOTING_SPEED;
        playerReloadTimer = Zoolatry.CHICKY_RELOAD_TIMER;
        magazineCapacity = magazineBullets = Zoolatry.CHICKY_MAGAZINE_CAPACITY;
    }

    public override void ShootProjectiles(Vector3 pos,Vector3 dir,float lag)
    {
        GameObject go = Instantiate(projectilePrefab,pos,Quaternion.identity) as GameObject;
        float spreadX = UnityEngine.Random.Range(-Zoolatry.CHICKY_SHOOTING_SPREAD,Zoolatry.CHICKY_SHOOTING_SPREAD);
        float spreadZ = UnityEngine.Random.Range(-Zoolatry.CHICKY_SHOOTING_SPREAD,Zoolatry.CHICKY_SHOOTING_SPREAD);

        go.transform.LookAt(dir+new Vector3(spreadX,0,spreadZ));
        go.transform.forward = dir;
        go.GetComponent<Bullet>().InitializeBullet(Zoolatry.CHICKY_PROJECTILE_RANGE,Zoolatry.CHICKY_PROJECTILE_DAMAGE,Zoolatry.CHICKY_PROJECTILE_SPEED,Zoolatry.BULLET_TARGET.Enemies,lag);
    }

}
