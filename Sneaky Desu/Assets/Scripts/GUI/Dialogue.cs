using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    public string[] dialogue;

    public void Run(int index, float speed)
    {
        bool running = false;
        if (running == false)
        {
            running = true;
            StartCoroutine(GameManager.instance.DisplayText(dialogue[index], speed));
        }
    }
}
