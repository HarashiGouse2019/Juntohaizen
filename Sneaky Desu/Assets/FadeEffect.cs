using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeEffect : MonoBehaviour, IPooledObject
{
    float time = 0;
    float fadeDuration = 0.3f;

    public void OnObjectSpawn()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.g, 1);
    }

    private void Update()
    {
        time += Time.deltaTime;

        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.g, 1 - (time / fadeDuration));

        if (time > fadeDuration)
            Disappear();
    }
    public void Disappear()
   {
        gameObject.SetActive(false);
   }
}
