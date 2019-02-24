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
        _crashPlanetTempl = GameObject.Find("CrashPlanet");
        _crashPlanetTempl.SetActive(false);
        _crashedPlanets = new List<CrashedPlanet>();
        _planetsCache = new List<GameObject>();
        _earthTransform = GetComponent<Transform>();
        _flame = GetComponentInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        float deltaTime = Time.deltaTime;
        EndDistance -= deltaTime * _horizontalVelocity;
        if (EndDistance <= 0)
        {
            // win
            UIRootControl.instance.WinGame();
            return;
        }
        if (_earthTransform.position.x <= -20)
        {
            UIRootControl.instance.LoseGame();
            return;
        }
        if (_crossingBlackHole)
        {
            _stayInBlackHoleTime += deltaTime;
            _verticalVelocity = 0;
            float horSpeedPercent = blackHoleSpeed.Evaluate(_stayInBlackHoleTime);
            _horizontalVelocity = horSpeedPercent * _enterBlackHoleHorSpeed;
            _SetEarthPosition();
            _SetFlame();
            return;
        }
        if (_horizontalVelocity > Constant.maxHorizontalVelocity)
        {
            _horizontalVelocity -= deltaTime * Constant.overMaxHVelocityDecreaseDelta;
        }
        if (_afterStopKeepVelocityDelay > 0)
        {
            _afterStopKeepVelocityDelay -= deltaTime;
            _CalcVelocity(new Vector2(0, _afterStopKeepVerticalDirection));
            _SetEarthPosition();
            _SetFlame();
            return;
        }
        if (_stopSurroundDelay > 0)
        {
            _stopSurroundDelay -= deltaTime;
            if (_stopSurroundDelay <= 0)
            {
                _surroundState = false;
                _CrashStar(_surroundingStar);
                _afterStopKeepVelocityDelay = Constant.afterStopKeepVelocityDelay;
                Vector3 dir = _earthTransform.position - _surroundCenter;
                int a = dir.x > 0 ? 1 : -1;
                _horizontalVelocity = a * Constant.afterStopHorizontalVelocity;
                if (_horizontalVelocity < 0)
                {
                    _horizontalVelocity = -0.1f;
                }
                _afterStopKeepVerticalDirection = dir.y > 0 ? 1 : -1;
                _CalcVelocity(new Vector2(0, _afterStopKeepVerticalDirection));
            }
            return;
        }
        if (_surroundState)
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                if (_surroundState)
                {
                    _stopSurround();
                    return;
                }
            }
            _Surrounding();
        }
        else
        {
            _CalcVelocity(_HandleInput());
            _SetEarthPosition();
            _SetFlame();
        }
        for (int i = _crashedPlanets.Count - 1; i >= 0; i--) 
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

    public float GetEarthPosX()
    {
        return _earthTransform.position.x;
    }

    public void SetEarthPosX(float x)
    {
        _earthTransform.position = new Vector3(x, _earthTransform.position.y, _earthTransform.position.z);
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
        if (_horizontalVelocity < Constant.maxHorizontalVelocity)
        {
            float coe2 = EarthModel.instance.velocity.value;
            if (coe2 == 0)
            {
                coe2 = 0.3f;
            }
            _horizontalVelocity += direction.x * _accelerationCoefficient * coe2;
            if (_horizontalVelocity < _minHorizontalVelocity)
            {
                _horizontalVelocity = _minHorizontalVelocity;
            }
        }
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
        if (Mathf.Abs(newPos.y) >= mapBound) 
        {
            int a = newPos.y > 0 ? 1 : -1;
            _verticalVelocity = 0;
            newPos.y = mapBound * a;
            _afterStopKeepVelocityDelay = -1;
        }
        _earthTransform.position = newPos;
    }

    private float _Curve(float value)
    {
        return _curve.Evaluate(value);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_surroundState)
        {
            return;
        }
        if (!other.isTrigger)
        {
            if (other.gameObject.tag == "EcoStar")
            {
                _surroundingStar = other.gameObject;
                _startSurround(Constant.ecoStarRadius, Constant.surroundAngleSpeed,
                    other.transform.position);
            }
            else if( other.gameObject.tag == "Blackhole")
            {
                if (!_crossingBlackHole)
                {
                    _crossingBlackHole = true;
                    EarthModel.instance.AddBuff("BlackHole");
                    _enterBlackHoleHorSpeed = _horizontalVelocity;
                    _stayInBlackHoleTime = 0.0f;
                }
            }
            else if (other.gameObject.tag == "Resource")
            {
                EarthModel.instance.Impact(60, 0, 30, true);
                _CrashStar(other.gameObject);
            }
            else
            {
                EarthModel.instance.Impact(360, -50, 200, false);
                _CrashStar(other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Blackhole")
        {
            _crossingBlackHole = false;
            EarthModel.instance.RemoveBuff("BlackHole");
            _stayInBlackHoleTime = 0.0f;
        }
    }

    private void _CrashStar(GameObject planet)
    {
        _playCrashEffect(planet.gameObject);
        Destroy(planet.gameObject);
    }

    private void _playCrashEffect(GameObject planet)
    {
        GameObject go;
        if (_planetsCache.Count > 0)
        {
            go = _planetsCache[0];
            _planetsCache.RemoveAt(0);
        }
        else
        {
            go = Instantiate(_crashPlanetTempl);
            go.transform.SetParent(nodeRoot);
        }
        go.transform.position = planet.transform.position;
        ParticleSystem particle = go.GetComponent<ParticleSystem>();
        CrashedPlanet crashedPlanet = new CrashedPlanet
        {
            lifeTime = particle.main.duration,
            go = go
        };
        _crashedPlanets.Add(crashedPlanet);
        go.SetActive(true);
        particle.Play();
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
        if (Mathf.Abs(speed) <= 0.001)
        {
            _flame.gameObject.SetActive(false);
        }
        else
        {
            if(!_flame.gameObject.active)
            {
                _flame.gameObject.SetActive(true);
            }
            float lifeTime = speed / maxFlameSpeed * maxFlameLifeTime + minFlameLifeTime;
            _flame.startLifetime = lifeTime;
        }
    }

    private void _startSurround(float radius, float angleSpeed, Vector3 center)
    {
        _horizontalVelocity = 0;
        _verticalVelocity = 0;
        _surroundRadius = radius;
        _surroundAngleSpeed = angleSpeed;
        Vector3 v = gameObject.transform.position - center;
        _surroundAngle = Vector3.Angle(Vector3.left, v);
        center.z = _earthTransform.position.z;
        _surroundCenter = center;
        _surroundState = true;
        // 停止喷火
        _setFlameParameter(Quaternion.identity, 0);
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
        if (!_surroundState || _stopSurroundDelay > 0) 
        {
            return;
        }
        Vector3 to = _earthTransform.position - _surroundCenter;
        float hor = to.x;
        float ver = to.y;
        float angle = Mathf.Atan2(ver, hor) * 57.29f;
        Quaternion rota = Quaternion.Euler(angle, -90, 0);
        _setFlameParameter(rota, maxFlameSpeed);
        _stopSurroundDelay = Constant.stopSurroundDelayTime;
    }

    private Vector3 _calcSurroundPos()
    {
        Vector3 v = Quaternion.Euler(0, 0, _surroundAngle) * Vector3.left;
        Vector3 res = v * _surroundRadius + _surroundCenter;
        return res;
    }

    public AnimationCurve _curve;
    public AnimationCurve blackHoleSpeed;
    public float xBound = -10f;
    public float mapBound = 8.5f;
    public float maxFlameSpeed = 3;
    public float maxFlameLifeTime = 0.2f;
    public float minFlameLifeTime = 0.05f;
    public Transform nodeRoot;
    public float HorizontalVelocity
    {
        get
        {
            return _horizontalVelocity;
        }
    }
    public float EndDistance
    {
        set
        {
            _endDistance = value;
        }
        get
        {
            return _endDistance;
        }
    }

    private float _endDistance = 315f;
    private GameObject _crashPlanetTempl;
    private List<CrashedPlanet> _crashedPlanets;
    private List<GameObject> _planetsCache;
    private float _stopSurroundDelay = 0;
    private float _afterStopKeepVelocityDelay = 0;
    private int _afterStopKeepVerticalDirection = 1;
    private bool _crossingBlackHole = false;
    private float _stayInBlackHoleTime = 0.0f;
    private float _enterBlackHoleHorSpeed = 0f;
    private GameObject _surroundingStar;
    private bool _surroundState = false;
    private float _surroundAngle;
    private float _surroundRadius;
    private float _surroundAngleSpeed;
    private Vector3 _surroundCenter;
    private ParticleSystem _flame;
    private Transform _earthTransform;
    private float _horizontalVelocity = Constant.initialHorizontalVelocity;
    // 能用操作降低到的最低速度
    private float _minHorizontalVelocity = 0.5f;
    private float _verticalVelocity;
    private float _accelerationCoefficient = 0.1f;
}
