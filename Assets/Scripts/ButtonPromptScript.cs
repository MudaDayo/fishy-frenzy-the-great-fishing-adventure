using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPromptScript : MonoBehaviour
{
    /*public float amplitude = 5000f;
    public float frequency = 0.1f;*/
    public GameObject parent;

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = parent.transform.position - new Vector3(0f, -1.5f, 1.5f);
        //transform.localScale = new Vector2(Mathf.Sin(Time.deltaTime * frequency) * amplitude, Mathf.Sin(Time.deltaTime * frequency) * amplitude);
        transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, Camera.main.transform.position, 1 , 0.0f));
    }
}
