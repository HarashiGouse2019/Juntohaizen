using UnityEngine;

public class NPCPlug : MonoBehaviour
{
    public NPC npcCharacter;

    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        spriteRenderer.sprite = npcCharacter.NPCSprite;
    }
}
