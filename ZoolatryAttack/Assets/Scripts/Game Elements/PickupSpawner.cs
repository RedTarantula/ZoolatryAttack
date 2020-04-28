using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupSpawner : MonoBehaviour
{
    float timer = 0f;
    public Transform spawnPoint;
    public GameObject healthPickup;
    public GameObject ammoPickup;
    public GameObject currentObj;

    private void Start()
    {
        // Run on master only
        StartTimer();
    }


    private void FixedUpdate()
    {
        // Run on master only
        if (timer > 0)
        {
            timer -= Time.fixedDeltaTime;
        }
        else if (currentObj == null)
        {
            SpawnRandomPickup();
        }
    }

    public void StartTimer()
    {
        currentObj = null;
        timer = Random.Range(Zoolatry.PICKUP_SPAWNER_TIMER_MIN,Zoolatry.PICKUP_SPAWNER_TIMER_MAX);
    }

    void SpawnRandomPickup()
    {
        int r = Random.Range(0,2);
        GameObject go = healthPickup;
        if(r == 0)
        {
            go = ammoPickup;
        }

        currentObj = Instantiate(go,spawnPoint.position,Quaternion.identity,null);
        currentObj.transform.parent = spawnPoint;
        currentObj.GetComponent<PickupBase>().Initialize(this);
    }
}
