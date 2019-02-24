using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct FloatPropery
{
    public readonly string name;
    public float max;
    public float value;
    public float growth;
    public float limitedValue => Mathf.Min(value, max);

    public FloatPropery(string name, float value, float growth, float max)
    {
        this.name = name;
        this.value = value;
        this.growth = growth;
        this.max = max;
    }
}

public class FloatBuff
{
    public float effect { get; }
    public string desc { get; }
    private Action<EarthModel, FloatBuff> buff { get; set; }

    public FloatBuff(string desc, float effect, Action<EarthModel, FloatBuff> buff)
    {
        this.desc = desc;
        this.effect = effect;
        this.buff = buff;
    }

    public EarthModel BuffIt(EarthModel model)
    {
        buff(model, this);
        return model;
    }
}

public class Buff
{
    public string name { get; }
    public string desc => string.Join("\n", buffs.Select(buff=>string.Format(buff.desc, buff.effect)));
    private FloatBuff[] buffs;

    public Buff(string name, params FloatBuff[] buffs)
    {
        this.name = name;
        this.buffs = buffs;
    }

    public EarthModel BuffIt(EarthModel model)
    {
        foreach (var buff in buffs)
        {
            buff.BuffIt(model);
        }

        return model;
    }
    
}

public class EarthModel : MonoBehaviour
{
    public static EarthModel instance { get; private set; }

    void Awake()
    {
        instance = this;
    }

    public void ResetData()
    {
        FloatPropery e = this.energy;
        e.value = 10500;
        this.energy = e;
        FloatPropery p = this.population;
        p.value = 3500;
        this.population = p;
        FloatPropery t = this.tech;
        t.value = 5000;
        this.tech = t;
        FloatPropery d = this.defense;
        d.value = 0;
        this.defense = d;
        FloatPropery v = this.velocity;
        v.value = 0.5f;
        this.velocity = v;
    }

    private readonly Dictionary<string, FloatPropery> m_Properties = new Dictionary<string, FloatPropery>
    {
        {"velocity", new FloatPropery("velocity", 0.5f, 0, 1)},
        {"energy", new FloatPropery("energy", 10500, -100, 21000)},
        {"population", new FloatPropery("population", 3500, 0, 7000)},
        {"tech", new FloatPropery("tech", 5000, 50, 20000)},
        {"defense", new FloatPropery("defense", 0, 0, float.MaxValue)},
    };

    public static Buff[] s_BuffList =
    {
        new Buff("BlackHole",
            new FloatBuff("能源增速 {0:N0}", -100, (model, buff) =>
            {
                var p = model.energy;
                p.growth += buff.effect;
                model.energy = p;
            }),
            new FloatBuff("人口增速 {0:N0}", -100, (model, buff) =>
            {
                var p = model.population;
                p.growth += buff.effect;
                model.population = p;
            }),
            new FloatBuff("速度加成 {0:P0}", -.5f, (model, buff) =>
            {
                var p = model.velocity;
                p.value += buff.effect;
                model.velocity = p;
            })
        ),
        new Buff("SpeedUp",
            new FloatBuff("速度加成 {0:P0}", .5f, (model, buff) =>
            {
                var p = model.velocity;
                p.value += buff.effect;
                model.velocity = p;
            })
        ),
        new Buff("Expansion",
            new FloatBuff("能源上限 {0:N0}", 1000, (model, buff) =>
            {
                var p = model.energy;
                p.max += buff.effect;
                model.energy = p;
            }),
            new FloatBuff("人口上限 {0:N0}", 1000, (model, buff) =>
            {
                var p = model.population;
                p.max += buff.effect;
                model.population = p;
            }),
            new FloatBuff("科技上限 {0:N0}", 1000, (model, buff) =>
            {
                var p = model.tech;
                p.max += buff.effect;
                model.tech = p;
            })
        ),
        new Buff("Reactor",
            new FloatBuff("能源增速 {0:P0}", .1f, (model, buff) =>
            {
                var p = model.energy;
                p.growth *= 1 + buff.effect;
                model.energy = p;
            })
        ),
        new Buff("Mining",
            new FloatBuff("科技增速 {0:P0}", .3f, (model, buff) =>
            {
                var p = model.tech;
                p.growth *= 1 + buff.effect;
                model.tech = p;
            })
        ),
        new Buff("Dungeons",
            new FloatBuff("人口增速 {0:N0}", 100, (model, buff) =>
            {
                var p = model.population;
                p.growth += buff.effect;
                model.population = p;
            })
        ),
        new Buff("EfficientEnergy",
            new FloatBuff("能源增速 {0:P0}", -.05f, (model, buff) =>
            {
                var p = model.energy;
                p.growth *= 1 + buff.effect;
                model.energy = p;
            })
        ),
        new Buff("Atmosphere",
            new FloatBuff("能源增速 {0:P0}", -.005f, (model, buff) =>
            {
                var p = model.energy;
                p.growth *= 1 + model.population.limitedValue / 100 * buff.effect;
                model.energy = p;
            })
        ),
        new Buff("Sheild",
            new FloatBuff("碰撞人口损失 -{0:P0}", .5f, (model, buff) =>
            {
                var p = model.defense;
                p.value += buff.effect;
                model.defense = p;
            })
        ),
        new Buff("Evasion",
            new FloatBuff("黑洞中速度增加 {0:P0}", .7f, (model, buff) =>
            {
                if (model.buffList.ContainsKey("BlackHole"))
                {
                    var p = model.velocity;
                    p.value += buff.effect;
                    model.velocity = p;
                }
            })
        ),
    };

    public static Dictionary<string, Buff> s_BuffDict = s_BuffList.ToDictionary(buff => buff.name);

    public Dictionary<string, Buff> buffList { get; } = new Dictionary<string, Buff>();

    public void AddBuff(string buffName)
    {
        if (!s_BuffDict.ContainsKey(buffName))
            return;
        if (buffList.ContainsKey(buffName))
            return;

        Debug.Log($"AddBuff {buffName}");
        buffList.Add(buffName, s_BuffDict[buffName]);
        ExecuteBuffEffects();
    }

    public void RemoveBuff(string buffName)
    {
        if (s_BuffDict.ContainsKey(buffName))
            return;
        if (!buffList.ContainsKey(buffName))
            return;

        Debug.Log($"RemoveBuff {buffName}");
        buffList.Remove(buffName);
        ExecuteBuffEffects();
    }

    private void ExecuteBuffEffects()
    {
        foreach (var buff in buffList.Values)
        {
            buff.BuffIt(this);
        }
    }

    public void Impact(float energy, float population, float tech, bool benefit)
    {
        FloatPropery p = this.energy;
        p.value += energy;
        this.energy = p;

        p = this.population;
        p.value += population - this.defense.limitedValue;
        this.population = p;

        p = this.tech;
        p.value += tech;
        this.tech = p;

        ControlPanel.instance.OnImpact(benefit);
    }

    public FloatPropery velocity
    {
        get => m_Properties["velocity"];
        set
        {
            if (m_Properties["velocity"].Equals(value))
                return;

            m_Properties["velocity"] = value;
        }
    }

    public FloatPropery energy
    {
        get => m_Properties["energy"];
        set
        {
            if (m_Properties["energy"].Equals(value))
                return;

            m_Properties["energy"] = value;
        }
    }

    public FloatPropery population
    {
        get => m_Properties["population"];
        set
        {
            if (m_Properties["population"].Equals(value))
                return;

            m_Properties["population"] = value;
        }
    }

    public FloatPropery tech
    {
        get => m_Properties["tech"];
        set
        {
            if (m_Properties["tech"].Equals(value))
                return;

            m_Properties["tech"] = value;
        }
    }

    public FloatPropery defense
    {
        get => m_Properties["defense"];
        set
        {
            if (m_Properties["defense"].Equals(value))
                return;

            m_Properties["defense"] = value;
        }
    }

    public void UpdateData()
    {
        FloatPropery p = energy;
        p.value += energy.growth;
        energy = p;

        p = population;
        p.value += population.growth;
        population = p;

        p = tech;
        p.value += tech.growth;
        tech = p;
    }

    public void UpdatePer1s()
    {
        UpdateData();
    }

    private float _t = 0;

    public void Update()
    {
        _t += Time.deltaTime;
        if (_t >= 1)
        {
            UpdatePer1s();
            _t = 0;
        }
    }

}
