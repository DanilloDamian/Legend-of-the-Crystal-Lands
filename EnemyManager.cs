﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{

    public Dictionary<GameObject, Vector3> initialPositions = new Dictionary<GameObject, Vector3>();
    public GameObject[] enemies;

    void Awake()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
    }
    
    void Start()
    {        
        foreach (GameObject enemy in enemies)
        {
            if (!initialPositions.ContainsKey(enemy))
            {
                initialPositions.Add(enemy, enemy.transform.position);
            }
        }        
    }

    public void ResetAllEnemies()
    {        
        StartCoroutine(ResetPositionsEnemies());
    }

    IEnumerator ResetPositionsEnemies()
    {
        yield return new WaitForSeconds(2f);
        foreach (KeyValuePair<GameObject, Vector3> position in initialPositions)
        {
           position.Key.transform.position = position.Value;
           position.Key.GetComponent<SlimeIA>().Restart();
        }
    }


}