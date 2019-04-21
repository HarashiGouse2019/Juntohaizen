using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    public static Dialogue dialogueList;

    public string[] dialogue;

    [HideInInspector] public float delay = 0;

    private void Awake()
    {
        if (dialogueList == null)
        {
            dialogueList = this;
        }
    }

    private void Update()
    {
        delay -= Time.timeScale;
    }

    public void Run(int index, float speed, float delay)
    {
        StartCoroutine(GameManager.instance.DisplayText(dialogue[index], speed));   
    }
}
