using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    float distanceTraveled;
    float reach;
    float damage;
    float lag;
    public float speed;
    Zoolatry.BULLET_TARGET target;

    private void Start()
    {
    }

    private void Update()
    {
        Vector3 prevPos = transform.position;
        transform.position += transform.forward * speed;
        Vector3 newPos = transform.position;

        distanceTraveled += Vector3.Distance(prevPos,newPos);
        if(distanceTraveled>=reach)
        {
            KillBullet();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        KillBullet();
    }

    void KillBullet()
    {
        Destroy(gameObject);
    }

    public void InitializeBullet(float reach, float damage, float speed, Zoolatry.BULLET_TARGET target, float lag)
    {
        //Debug.Log($"Initialized bullet with forward vector being <{transform.forward.x}> x <{transform.forward.y}> x <{transform.forward.z}>");
        this.reach = reach;
        this.damage = damage;
        this.target = target;
        this.lag = lag;
        this.speed = speed;
        distanceTraveled = speed * lag;
        transform.position += transform.forward * speed * lag;
    }
}
