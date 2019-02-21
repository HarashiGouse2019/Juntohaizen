using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PlayerStats
{

    public class PlayerStatusScript : MonoBehaviour
    {

        public float maxHealth = 100, maxMana = 100;
        private float currentHealth, currentMana;

        [HideInInspector] public float levelProgression = 0, level = 0;

        [Header("Player Status UI Reference")]
        public Image healthUI;
        public Image manaUI;
        public Image levelProgressionUI;


        // Start is called before the first frame update
        void Start()
        {
            currentHealth = maxHealth;
            currentMana = maxMana;

            healthUI.fillAmount = currentHealth / maxHealth;
            manaUI.fillAmount = currentMana / maxMana;
            levelProgressionUI.fillAmount = levelProgression;

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return)) IncreaseLevel(1f);
            if (Input.GetKeyDown(KeyCode.Equals)) IncreaseHealth(1f);
            if (Input.GetKeyDown(KeyCode.Minus)) DecreaseHealth(1f);

        }

        public float IncreaseLevel(float value)
        {
            levelProgressionUI.fillAmount += value / 100f;
            Debug.Log(levelProgressionUI.fillAmount);
            if (levelProgressionUI.fillAmount == 1f)
            {
                levelProgressionUI.fillAmount = 0;
            }
            return value;
        }

        public float IncreaseHealth(float value)
        {
            if (healthUI.fillAmount != maxHealth) healthUI.fillAmount += value / maxHealth;
            return value;
        }

        public float DecreaseHealth(float value)
        {
            if (healthUI.fillAmount != 0) healthUI.fillAmount -= value / maxHealth;
            return value;
        }

        public float IncreaseMana(float value)
        {

            return value;
        }
    }
}
