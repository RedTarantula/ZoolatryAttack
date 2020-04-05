using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupBase : MonoBehaviour
{
    PickupSpawner spawner;
    public void Initialize(PickupSpawner spawner)
    {
        this.spawner = spawner;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject player = other.gameObject;
            if (gameObject.CompareTag("HealthPickup"))
            {
                player.GetComponent<PlayerBase>().PickupReaction(Zoolatry.PICKUP_TYPE.Health);
            }
            else if (gameObject.CompareTag("AmmoPickup"))
            {
                player.GetComponent<PlayerBase>().PickupReaction(Zoolatry.PICKUP_TYPE.Ammo);
            }
            spawner.StartTimer();
            Destroy(gameObject);
        }
    }
}
