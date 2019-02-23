using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        _earthTransform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        _CalcVelocity(_HandleInput());
        _SetEarthVelocity();
    }

    private Vector2 _HandleInput()
    {
        Vector2 direction = Vector2.zero;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            direction += Vector2.up;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            direction += Vector2.down;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            direction += Vector2.left;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            direction += Vector2.right;
        }
        return direction;
    }

    private void _CalcVelocity(Vector2 direction)
    {
        _horizontalVelocity += direction.x * _accelerationCoefficient;
        float dirY = direction.y;// [-1, 0, 1]
        float posY = _earthTransform.position.y;
        if (dirY == -1)
        {
            // go down
            _verticalVelocity = -_Curve(-posY / _mapBound);
        }
        else if (dirY == 0)
        {
            // go center
            int a = posY > 0 ? 1 : -1;
            _verticalVelocity = -a * _Curve((_mapBound - a * posY) / _mapBound);
        }
        else if (dirY == 1)
        {
            // go up
            _verticalVelocity = _Curve(posY / _mapBound);
        }
    }

    private void _SetEarthVelocity()
    {
        bool useHorizontalV = true;
        Vector3 velocity = new Vector3(useHorizontalV ? _horizontalVelocity : 0, _verticalVelocity, 0);
        _earthTransform.position += velocity;
    }

    private float _Curve(float value)
    {
        return _curve.Evaluate(value);
    }

    public AnimationCurve _curve;
    private Transform _earthTransform;
    private float _horizontalVelocity;
    private float _verticalVelocity;
    private float _accelerationCoefficient = 0.01f;
    private float _mapBound = 8.5f;
}
