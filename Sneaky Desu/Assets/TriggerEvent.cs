using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class TriggerEvent : MonoBehaviour
{
    public static TriggerEvent system;

    public enum Events
    {
        Transition,
        CutScene,
        Ambush,
        Trap
    }
    public Events Event;


    public class StringInput
    {
        public string stringValue;
    }

    [Header("TransitionTo")]
    public string location;
    public Vector2 position;
    DoorEntry entry;

    // Start is called before the first frame update
    void Awake()
    {
        system = this;
        switch(Event)
        {
            case Events.Transition:
                entry = gameObject.AddComponent<DoorEntry>();
                entry.scene_name = location;
                entry.value_x = position.x;
                entry.value_y = position.y;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Trigger()
    {
        switch (Event)
        {
            case Events.Transition:
                Debug.Log("Transition");
                TransitionTo(location, position.x, position.y);
                break;
            case Events.CutScene:
                ActivateCutScene();
                break;
            case Events.Ambush:
                BeginAmbush();
                break;
            case Events.Trap:
                ActivateTrap();
                break;
            default:
                break;
        }
    }

    void TransitionTo(string _location, float _x, float _y)
    {
        GameManager.Instance.Scene_Name = _location;
        entry.scene_name = GameManager.Instance.Scene_Name;
        GameManager.Instance.posx = _x;
        GameManager.Instance.posy = _y;

        Player_Spawn.Instance.coordinates = new Vector3(GameManager.Instance.posx, GameManager.Instance.posy, 0);

        entry.player.gameObject.transform.position = Player_Spawn.Instance.coordinates;

        GameManager.Instance.Goto_Scene(_location);
    }

    void ActivateCutScene()
    {

    }

    void BeginAmbush()
    {

    }

    void ActivateTrap()
    {

    }
}
