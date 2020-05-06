using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alarm;

public class CameraShakeEffect : MonoBehaviour
{
    public static CameraShakeEffect camse;

    //TODO: Make it to were a camera can add a cumulative amount of "shock" in the scene.

    #region Public Members
    [Header("Main Camera")]
    public Camera mainCamera;

    #endregion

    #region Private Members
    private float intensity;

    #endregion

    void Awake()
    {
        if (camse == null)
        {
            camse = this;
            DontDestroyOnLoad(camse);
        } else
        {
            Destroy(gameObject);
        }

        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    //List of Different Camera Effects
    public void Shake(float _intensity, float _duration)
    {
        intensity = _intensity;
        InvokeRepeating("BeginShake", 0, 0.05f);
        if (_duration != -1) Invoke("StopShake", _duration);
    }

    void BeginShake()
    {
        if (intensity > 0)
        {
            Vector3 camPosition = mainCamera.transform.position;

            float offsetX = Random.value * intensity * 2 - intensity;
            float offSetY = Random.value * intensity * 2 - intensity;

            camPosition.x += offsetX;
            camPosition.y += offSetY;

            mainCamera.transform.position = camPosition;
        }
    }

    void StopShake()
    {
        CancelInvoke("BeginShake");
    }
}
