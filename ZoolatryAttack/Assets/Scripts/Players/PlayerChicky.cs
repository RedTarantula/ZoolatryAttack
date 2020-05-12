using UnityEngine;
using static Zoolatry;

public class PlayerChicky : PlayerBase
{
    public override void StartLocalVariables()
    {
        character = PLAYER_CHARACTER.Chicky;

        pVars = new PlayerVariables(character);
    }
    public override void ShootProjectiles(Vector3 pos,Vector3 dir,float lag)
    {
        GameObject go = Instantiate(projectilePrefab,pos,Quaternion.identity) as GameObject;
        float spread = Random.Range(-CHICKY_SHOOTING_SPREAD,CHICKY_SHOOTING_SPREAD);

        go.transform.forward = dir;
        go.GetComponent<Bullet>().InitializeBullet(CHICKY_PROJECTILE_RANGE,CHICKY_PROJECTILE_DAMAGE,CHICKY_PROJECTILE_SPEED,BULLET_TARGET.Enemies,lag);
    }
    public override void PickupReaction(PICKUP_TYPE pickupType)
    {
        if (pickupType == PICKUP_TYPE.Ammo)
        {
            SetAmmoInv(CHICKY_AMMO_MAX);
        }
        else if (pickupType == PICKUP_TYPE.Health)
        {
            HealPercent(PICKUP_HEALTH_PERCENT);
        }
    }
}
