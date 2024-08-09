using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WayEnemyPoint : MonoBehaviour
{
    public Collider[] sphereColliderInfo;
    public LayerMask enemyMask;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            sphereColliderInfo = Physics.OverlapSphere(transform.position, 2f, enemyMask);   
            if(sphereColliderInfo.Length > 1)
            {
                foreach (Collider c in sphereColliderInfo)
                {
                    if (c.gameObject.tag == "Enemy")
                    {
                        c.gameObject.GetComponent<SlimeIA>().ChangeWaitPoint();
                    }
                }
            }           
        }
    }

}
