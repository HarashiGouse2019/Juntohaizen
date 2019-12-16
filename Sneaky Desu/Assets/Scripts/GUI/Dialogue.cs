using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    public string[] dialogue;

    GameManager manager;
    public void Run(int _index, float _speed)
    {
        manager = GameManager.Instance;
        StartCoroutine(manager.DisplayText(dialogue[_index], _speed));
    }
}
