using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingZoneRotate : MonoBehaviour
{
    [SerializeField]
    float rotationSpeed;
    void Update()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }
}
