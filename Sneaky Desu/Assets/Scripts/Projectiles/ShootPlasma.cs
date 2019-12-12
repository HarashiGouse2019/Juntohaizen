using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootPlasma : MonoBehaviour
{
    public static ShootPlasma script;

    public ObjectPooler pooler;

    public float time = 0.5f;
    public float resetTime;

    [HideInInspector] public bool active;

    Vector3 direction;
    public GameObject target;
    public float speed;
    private void Awake()
    {
        script = this;
        pooler = FindObjectOfType<ObjectPooler>();
        resetTime = time;
    }
    private void Update()
    {
        if (active == true)
        {
            time -= Time.deltaTime;
            if (time <= 0)
            {
                pooler.SpawnFromPool("enemyPlasma", transform.position, Quaternion.identity);
                time = resetTime;
                
            }
        }
    }
}
