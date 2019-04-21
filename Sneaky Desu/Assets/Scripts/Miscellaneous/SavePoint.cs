using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    public SpriteRenderer saveAlpha;

    Color alpha;

    public static SavePoint savepoint;

    float value = 0f;
    const float maxOpacity = 1, minOpacity = 0;

    public bool toggle = false;

    // Start is called before the first frame update
    void Start()
    {
        savepoint = this;
        alpha = saveAlpha.color;
        alpha.a = 0f;
        
    }

    private void Update()
    {
        saveAlpha.color = alpha;
        if (toggle == true)
        {
            if (alpha.a != minOpacity)
                StartCoroutine(Hide());
        }
        if (alpha.a <= minOpacity)
        {
            toggle = false;
            StopCoroutine(Hide());
        }
        else if (alpha.a >= maxOpacity)
        {
            alpha.a = maxOpacity;
            StopCoroutine(Show(0.005f));
        }
    }

    public IEnumerator Show(float duration)
    {

            value = 0.12f;
            alpha.a += value;
        yield return new WaitForSeconds(duration);
    }

    public IEnumerator Hide()
    {
            value = 0.12f;
            alpha.a -= value;
            yield return new WaitForSeconds(0.005f);
    }
}
