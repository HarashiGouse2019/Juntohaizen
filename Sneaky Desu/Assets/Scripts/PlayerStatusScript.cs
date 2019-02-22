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

        public TextMeshProUGUI levelUI;


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
            if (Input.GetKey(KeyCode.Return)) IncreaseLevel(1f);
            if (Input.GetKey(KeyCode.Backspace)) DecreaseLevel(1f);
            if (Input.GetKey(KeyCode.Equals)) IncreaseHealth(1f);
            if (Input.GetKey(KeyCode.Minus)) DecreaseHealth(1f);

            levelUI.text = level.ToString();

        }

        public float IncreaseLevel(float value)
        {
            levelProgressionUI.fillAmount += value / 100f;
            Debug.Log(levelProgressionUI.fillAmount);
            if (levelProgressionUI.fillAmount == currentHealth / maxHealth)
            {
                level += 1;
                levelProgressionUI.fillAmount = 0f;
            }
            return value;
        }

        public float DecreaseLevel(float value)
        {
            levelProgressionUI.fillAmount -= value / 100f;
            Debug.Log(levelProgressionUI.fillAmount);
            if (levelProgressionUI.fillAmount < 1f / maxHealth && level != 0f)
            {
                level -= 1;
                levelProgressionUI.fillAmount = maxHealth - 1f;
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
