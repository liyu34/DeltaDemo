using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetMove : MonoBehaviour
{
    public float speed;

    void Update()
    {
        if (Main.instance.isPaused)
        {
            return;
        }
        transform.position = transform.position - Vector3.right * (Time.deltaTime * speed);
    }
}
