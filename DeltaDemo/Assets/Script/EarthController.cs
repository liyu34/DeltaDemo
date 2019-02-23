using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        _earthTransform = GetComponent<Transform>();
        _flame = GetComponentInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        _CalcVelocity(_HandleInput());
        _SetEarthPosition();
        _setFlame();
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
            _verticalVelocity = -_Curve(-posY / mapBound);
        }
        else if (dirY == 0)
        {
            // go center
            int a = posY > 0 ? 1 : -1;
            _verticalVelocity = -a * _Curve((mapBound - a * posY) / mapBound);
        }
        else if (dirY == 1)
        {
            // go up
            _verticalVelocity = _Curve(posY / mapBound);
        }
    }

    private void _SetEarthPosition()
    {
        float posX = _earthTransform.position.x;
        bool useHorizontalV = (posX >= xBound && _horizontalVelocity >= 0) ? false : true;
        Vector3 velocity = new Vector3(useHorizontalV ? _horizontalVelocity : 0, _verticalVelocity, 0);
        Vector3 newPos = _earthTransform.position + velocity;
        if (newPos.x > xBound)
        {
            newPos.x = xBound;
        }
        _earthTransform.position = newPos;
    }

    private float _Curve(float value)
    {
        return _curve.Evaluate(value);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger)
        {
            Destroy(other.gameObject);
        }
    }

    private void _setFlame()
    {
        float angle = 0;
        if (_verticalVelocity == 0 || HorizontalVelocity == 0)
        {
            if (_verticalVelocity != 0)
            {
                angle = _verticalVelocity > 0 ? 90 : -90;
            }
            if (HorizontalVelocity != 0)
            {
                angle = HorizontalVelocity > 0 ? 0 : 180;
            }
        }
        else
        {
            angle = Mathf.Atan2(_verticalVelocity, _horizontalVelocity) * 57.29f;
        }
        // y = -90 是固定的
        _flame.transform.rotation = Quaternion.Euler(angle, -90, 0);

        // 速度刺激火焰大小
        float speed = Mathf.Sqrt
            (_verticalVelocity * _verticalVelocity + _horizontalVelocity * _horizontalVelocity);
        float lifeTime = speed / maxFlameSpeed * maxFlameLifeTime + minFlameLifeTime;
        _flame.startLifetime = lifeTime;
    }

    public AnimationCurve _curve;
    public float xBound = -10f;
    public float mapBound = 8.5f;
    public float maxFlameSpeed = 3;
    public float maxFlameLifeTime = 0.2f;
    public float minFlameLifeTime = 0.05f;
    public float HorizontalVelocity
    {
        get
        {
            return _horizontalVelocity;
        }
    }
    private ParticleSystem _flame;
    private Transform _earthTransform;
    private float _horizontalVelocity;
    private float _verticalVelocity;
    private float _accelerationCoefficient = 0.01f;
}
