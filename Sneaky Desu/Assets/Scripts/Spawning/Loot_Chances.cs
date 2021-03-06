﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Random;

public class Loot_Chances : MonoBehaviour
{

    public ObjectPooler objectpooler;

    int randomSpawn;

    private void Awake()
    {
        objectpooler = FindObjectOfType<ObjectPooler>();
       
    }

    private void Start()
    {
        randomSpawn = Random.Range(1, 25);
        int spawnAmount = 0;

        //You get a 10/24 chance of not getting anything,
        //a 5/24 on getting gems, 
        //a 4/24 chance on getting a live
        //a 4/24 chance on getting a large loot of gems
        //a 2/24 chance on getting a large loot of lives

        if (randomSpawn < 2)
            objectpooler.SpawnFromPool("xLives", transform.position, Quaternion.identity);

        else if (randomSpawn > 2 && randomSpawn < 7)
        {
            spawnAmount = 5;
            for (int i = 0; i < spawnAmount; i++)
                objectpooler.SpawnFromPool("xGem", transform.position, Quaternion.identity);
        }

        else if (randomSpawn > 7 && randomSpawn < 12)
            objectpooler.SpawnFromPool("Lives", transform.position, Quaternion.identity);

        else if (randomSpawn > 12 && randomSpawn < 17)
        {
            spawnAmount = 20;
            for (int i = 0; i < spawnAmount; i++)
                objectpooler.SpawnFromPool("Gem", transform.position, Quaternion.identity);
        }
    }
}
