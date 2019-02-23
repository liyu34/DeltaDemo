using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct CrashedPlanet
{
    public float lifeTime;
    public GameObject go;
}

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
        if (_surroundState)
        {
            _Surrounding();
        }
        else
        {
            _CalcVelocity(_HandleInput());
            _SetFlame();
            _SetEarthPosition();
        }
        float deltaTime = Time.deltaTime;
        for (int i = _crashedPlanets.Count; i >= 0; i--)
        {
            CrashedPlanet temp = new CrashedPlanet
            {
                lifeTime = _crashedPlanets[i].lifeTime - deltaTime
            };
            if (temp.lifeTime <= 0)
            {
                _planetsCache.Add(_crashedPlanets[i].go);
                _crashedPlanets.RemoveAt(i);
                continue;
            }
            temp.go = _crashedPlanets[i].go;
            _crashedPlanets[i] = temp;
        }
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
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (_surroundState)
            {
                _stopSurround();
            }
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

    private GameObject _spawnCrashedPlanet(GameObject planet)
    {
        if (_planetsCache.Count > 0)
        {
            GameObject go = _planetsCache[1];
            _planetsCache.RemoveAt(1);
            return go;
        }
        GameObject p = new GameObject();
        p.transform.SetParent(_earthTransform);
        p.transform.position = planet.transform.position;
        p.AddComponent<ParticleSystem>();
        ParticleSystem particle = p.GetComponent<ParticleSystem>();
        return p;
    }

    private void _SetFlame()
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
        Quaternion rotation = Quaternion.Euler(angle, -90, 0);

        // 速度刺激火焰大小
        float speed = Mathf.Sqrt
            (_verticalVelocity * _verticalVelocity + _horizontalVelocity * _horizontalVelocity);
        _setFlameParameter(rotation, speed);
    }

    private void _setFlameParameter(Quaternion rotation, float speed)
    {
        _flame.transform.rotation = rotation;
        float lifeTime = speed / maxFlameSpeed * maxFlameLifeTime + minFlameLifeTime;
        _flame.startLifetime = lifeTime;
    }

    private void _startSurround(float radius, float angleSpeed, Vector3 center)
    {
        _surroundRadius = radius;
        _surroundAngleSpeed = angleSpeed;
        Vector3 v = gameObject.transform.position - center;
        _surroundAngle = Vector3.Angle(Vector3.left, v);
        _surroundCenter = center;
        _surroundState = true;
        // 停止喷火
        _flame.startLifetime = 0;
    }

    private void _Surrounding()
    {
        float deltaTime = Time.deltaTime;
        _surroundAngle += _surroundAngleSpeed * deltaTime;
        Vector3 newPos = _calcSurroundPos();
        gameObject.transform.position = newPos;
    }

    private void _stopSurround()
    {
        _surroundState = false;
        Vector3 leavePos = _calcSurroundPos();
        Quaternion rotation = Quaternion.Euler(_surroundAngle, 0, 0);
        _setFlameParameter(rotation, maxFlameSpeed);
        Vector3 leaveVelocity = rotation * Vector3.right;
        _horizontalVelocity = leaveVelocity.x;
        _verticalVelocity = leaveVelocity.y;
    }

    private Vector3 _calcSurroundPos()
    {
        Vector3 v = Quaternion.Euler(_surroundAngle, 0, 0) * Vector3.left;
        v.Normalize();
        return v * _surroundRadius;
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

    private List<CrashedPlanet> _crashedPlanets;
    private List<GameObject> _planetsCache;
    private bool _surroundState = false;
    private float _surroundAngle;
    private float _surroundRadius;
    private float _surroundAngleSpeed;
    private Vector3 _surroundCenter;
    private ParticleSystem _flame;
    private Transform _earthTransform;
    private float _horizontalVelocity;
    private float _verticalVelocity;
    private float _accelerationCoefficient = 0.01f;
}
