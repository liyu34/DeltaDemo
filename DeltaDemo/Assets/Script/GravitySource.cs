using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Serialization;

public class GravitySource: MonoBehaviour
{
    [SerializeField] private float m_Factor = 0f;
    public float factor
    {
        get
        {
            return m_Factor;
        }
    }

    [SerializeField] private List<Transform> m_Objects;
    public List<Transform> objects
    {
        get
        {
            return m_Objects;
        }
    }

    [field: NonSerialized] private bool isPlaying { get; set; } = false;

    public void Play()
    {
        isPlaying = true;
    }

    public void Stop()
    {
        isPlaying = false;
    }

    protected float CalculateAngularVelocityForObject(Transform @object)
    {
        float radius = (@object.position - transform.position).magnitude;
        float velocity = Mathf.Sqrt(factor / radius);
        float angularVelocity = velocity / radius;
        return angularVelocity;
    }

    public void Update()
    {
        if (isPlaying)
        {
            Vector3 pos = transform.position;
            foreach (var o in objects)
            {
                float deltaAngular = CalculateAngularVelocityForObject(o) * Time.deltaTime;
                o.RotateAround(pos, Vector3.back, deltaAngular);
            }
        }
    }
}
