using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestActionClass : MonoBehaviour
{

    [System.Serializable]
    public class ActionsWhenOpening
    {
        public enum Actions
        {
            INCREASE_HEALTH,
            INCREASE_MANA,
            INCREASE_CURRENCY,
            ADD_TO_INVENTORY
        }

        public Actions action;

        public int increaseValue;

    }

    public ActionsWhenOpening OnOpen;

    public void InvokeAction()
    {
        switch (OnOpen.action)
        {
            case ActionsWhenOpening.Actions.INCREASE_HEALTH:
                GameManager.Instance.IncreaseHealth(OnOpen.increaseValue);
                break;
            case ActionsWhenOpening.Actions.INCREASE_MANA:
                GameManager.Instance.IncreaseMana(OnOpen.increaseValue);
                break;
            case ActionsWhenOpening.Actions.INCREASE_CURRENCY:
                GameManager.Instance.totalGems += OnOpen.increaseValue;
                break;
            case ActionsWhenOpening.Actions.ADD_TO_INVENTORY:
                break;
            default:
                break;
        }
    }
}
