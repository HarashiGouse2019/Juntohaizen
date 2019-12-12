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
        GameManager.instance.Scene_Name = _location;
        entry.scene_name = GameManager.instance.Scene_Name;
        GameManager.instance.posx = _x;
        GameManager.instance.posy = _y;

        Player_Spawn.instance.coordinates = new Vector3(GameManager.instance.posx, GameManager.instance.posy, 0);

        entry.player.gameObject.transform.position = Player_Spawn.instance.coordinates;

        GameManager.instance.Goto_Scene(_location);
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
