using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Magic_Discharge : MonoBehaviour
{

    public static Magic_Discharge magic_discarge;

    ObjectPooler objectPooler;
    GameManager manager;

    public float speedValue;
    public static float speed;

    [Range(1, 20)] public int recoilSpeed;

    bool ready = true;

    enum DISCHARGE_AMOUNT
    {
        WEAK,
        MODERATE,
        STRONG
    };

    public float dischargeAmount = (float) DISCHARGE_AMOUNT.WEAK;

    public static float buffSpeed;

    public Transform magicSource;
    public List<GameObject> magicDischarge;
    public int type;

    bool canUseMana = true;

    IEnumerator coroutine;

    // Start is called before the first frame update
    void Awake()
    {
        objectPooler = ObjectPooler.instance;
        manager = GameManager.instance;

        speed = speedValue;
        buffSpeed = speed * dischargeAmount;
        type = (int) dischargeAmount - 1;
    }

    // Update is called once per frame
    public void Update()
    {
        if (manager.currentMana != 0) canUseMana = true;
        else canUseMana = false;

        type = (int) manager.level - 1;

        //When the player shots lazers
        if (Input.GetKey(Player_Controller.player_controller.shoot))
        {
            coroutine = Recoil();
            if (manager.currentMana != 0)
            {
                if (ready == true)
                {
                    manager.DecreaseMana(1f);
                    UseMagic();
                    StartCoroutine(coroutine);
                    
                    ready = false;
                }
            }
            else
            {
                StopCoroutine(coroutine);
            }
        }
        else if (Input.GetKeyUp(Player_Controller.player_controller.shoot))
        {
            ready = true;
            StopCoroutine(coroutine);
        }
    }

    public void UseMagic()
    {
        if (canUseMana == true)
        {
            switch (type)
            {
                case 0:
                    AudioManager.audioManager.Play("Shoot000");
                    objectPooler.SpawnFromPool("LowMana", magicSource.position, magicSource.localRotation);
                    break;
                case 1:
                    AudioManager.audioManager.Play("Shoot001");
                    objectPooler.SpawnFromPool("MediumMana", magicSource.position, magicSource.localRotation);
                    break;
                case 2:
                    AudioManager.audioManager.Play("Shoot002");
                    objectPooler.SpawnFromPool("HighMana", magicSource.position, magicSource.localRotation);
                    break;
            }
        }
    }

    private IEnumerator Recoil()
    {
        float value = (float)recoilSpeed;
        yield return new WaitForSeconds(1 / value);
        ready = true;
    }
}
