using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        _earth = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        _CalcVelocity(_HandleInput());
        _SetEarthVelocity();
    }

    private Vector2 _HandleInput()
    {
        Vector2 acceleration = Vector2.zero;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            acceleration += Vector2.up;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            acceleration += Vector2.down;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            acceleration += Vector2.left;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            acceleration += Vector2.right;
        }
        return acceleration;
    }

    private void _CalcVelocity(Vector2 acceleration)
    {
        acceleration *= _accelrationCoefficient;
        _verticalVelocity += acceleration.y;
        _horizontalVelocity += acceleration.x;
    }

    private void _SetEarthVelocity()
    {
        bool useHorizontalV = true;
        _earth.velocity = new Vector2(useHorizontalV ? _horizontalVelocity : 0, _verticalVelocity);
    }

    private Rigidbody2D _earth;
    private float _verticalVelocity;
    private float _horizontalVelocity;
    private float _accelrationCoefficient = 0.01f;
}
