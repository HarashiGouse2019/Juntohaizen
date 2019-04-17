using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Magic_Discharge : MonoBehaviour
{

    ObjectPooler objectPooler;

    public float speedValue;
    public static float speed;

    [Range(1, 20)] public int recoilSpeed;

    bool isKeyReleased;

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

        speed = speedValue;
        buffSpeed = speed * dischargeAmount;
        type = (int) dischargeAmount - 1;

    }

    // Update is called once per frame
    public void Update()
    {

        //When the player shots lazers
        if (Input.GetKeyDown(Player_Controller.player_controller.shoot) || isKeyReleased == true)
        {
            if (GameManager.instance.currentMana != 0 )
            {

                coroutine = Recoil();
                GameManager.instance.DecreaseMana(1f);
                isKeyReleased = false;
                FindObjectOfType<AudioManager>().Play("Shoot000");

                //Instantiate(magicDischarge[type], magicSource.position, magicSource.localRotation); //A bullet will spawn with a set direction based on the player's direction
                UseMagic();
                StartCoroutine(coroutine);
            }
            else
            {
                StopCoroutine(coroutine);
                canUseMana = false;
            }
        }
        if (Input.GetKeyUp(Player_Controller.player_controller.shoot)) StopCoroutine(coroutine);
    }

    public void UseMagic()
    {
        switch(type)
        {
            case 0:
                objectPooler.SpawnFromPool("LowMana", magicSource.position, magicSource.localRotation);
                break;
            case 1:
                objectPooler.SpawnFromPool("MediumMana", magicSource.position, magicSource.localRotation);
                break;
            case 2:
                objectPooler.SpawnFromPool("HighMana", magicSource.position, magicSource.localRotation);
                break;
        }
    }

    private IEnumerator Recoil()
    {
        float value = (float)recoilSpeed;
        yield return new WaitForSeconds(1 / value);
        isKeyReleased = true;
    }
}
