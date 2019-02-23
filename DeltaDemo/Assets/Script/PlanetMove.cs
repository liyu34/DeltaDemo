using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetMove : MonoBehaviour
{
    public float speed;

    void Update()
    {
        transform.position = transform.position - Vector3.right * (Time.deltaTime * speed);
    }
}
