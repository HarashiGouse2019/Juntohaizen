using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Magic_Discharge : MonoBehaviour
{

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
        speed = speedValue;
        buffSpeed = speed * dischargeAmount;
        type = (int) dischargeAmount - 1;

        Debug.Log("BuffSpeed is now" + buffSpeed);
    }

    // Update is called once per frame
    void Update()
    {

        //When the player shots lazers
        if (Input.GetKeyDown(KeyCode.X) || isKeyReleased == true)
        {
            if (GameManager.instance.currentMana != 0)
            {

                coroutine = Recoil();
                GameManager.instance.DecreaseMana(1f);
                isKeyReleased = false;
                Instantiate(magicDischarge[type], magicSource.position, magicSource.localRotation); //A bullet will spawn with a set direction based on the player's direction
                StartCoroutine(coroutine);
            }
            else
            {
                StopCoroutine(coroutine);
                canUseMana = false;
            }
        }
        if (Input.GetKeyUp(KeyCode.X)) StopCoroutine(coroutine);


        Debug.Log("Current Mana: " + (GameManager.instance.currentMana * GameManager.instance.maxMana));
    }

    private IEnumerator Recoil()
    {
        float value = (float)recoilSpeed;
        yield return new WaitForSeconds(1 / value);
        isKeyReleased = true;
    }
}
