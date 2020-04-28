using UnityEngine;
using static Zoolatry;

public class PlayerPiggy : PlayerBase
{
    public override void StartLocalVariables()
    {
        character = PLAYER_CHARACTER.Piggy;

        pVars = new PlayerVariables(character);
        pGun = new PlayerGun(character);
    }
    public override void ShootProjectiles(Vector3 pos,Vector3 dir,float lag)
    {
        GameObject go = Instantiate(projectilePrefab,pos,Quaternion.identity) as GameObject;
        float spread = Random.Range(-PIGGY_SHOOTING_SPREAD,PIGGY_SHOOTING_SPREAD);

        go.transform.forward = dir;
        go.GetComponent<Bullet>().InitializeBullet(PIGGY_PROJECTILE_RANGE,PIGGY_PROJECTILE_DAMAGE,PIGGY_PROJECTILE_SPEED,BULLET_TARGET.Enemies,lag);
    }
    public override void PickupReaction(PICKUP_TYPE pickupType)
    {
        if (pickupType == PICKUP_TYPE.Ammo)
        {
            SetAmmoInv(PIGGY_AMMO_MAX);
        }
        else if (pickupType == PICKUP_TYPE.Health)
        {
            HealPercent(PICKUP_HEALTH_PERCENT);
        }
    }
}
