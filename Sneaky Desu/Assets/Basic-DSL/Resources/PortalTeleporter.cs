using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTeleporter : MonoBehaviour
{
    public CharacterController player;
    public Transform reciever;

    bool playerIsOverlapping = false;

    void Update()
    {
        if (playerIsOverlapping)
        {
            Vector3 portalToPlayer = player.transform.position - transform.position;
            float dotProduct = Vector3.Dot(transform.up, portalToPlayer);

            if (dotProduct < 0f)
            {
                float rotationDiff = -Quaternion.Angle(transform.rotation, reciever.rotation);
                rotationDiff += 180;
                player.transform.Rotate(Vector3.up, rotationDiff);

                Vector3 positionOffset = Quaternion.Euler(0f, rotationDiff, 0f) * portalToPlayer;
                player.enabled = false;
                player.transform.position = reciever.position + positionOffset;
                player.enabled = true;
                playerIsOverlapping = false;
            }
        }
    }
}