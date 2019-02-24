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

public class EarthModel
{
    private static EarthModel s_Instance = null;
    public static EarthModel instance
    {
        get
        {
            if (s_Instance is null)
            {
                s_Instance = new EarthModel();
            }

            return s_Instance;
        }
    }

    private readonly Dictionary<string, FloatPropery> m_Properties = new Dictionary<string, FloatPropery>
    {
        { "velocity", new FloatPropery("velocity", 1, 0, float.MaxValue)},
        { "energy", new FloatPropery("energy", 10500, -100, 21000)},
        { "population", new FloatPropery("population", 3500, 0, 7000)},
        { "tech", new FloatPropery("tech", 5000, 50, 20000)},
    };

    public static Buff[] s_BuffList = {
        new Buff("BlackHole",
            new FloatBuff("能源增速 {0}", -100, (model, buff) => {
                var p = model.energy;
                p.growth += buff.effect;
                model.energy = p;
            }),
            new FloatBuff("人口增速 {0}", -100, (model, buff) => {
                var p = model.population;
                p.growth += buff.effect;
                model.population = p;
            }),
            new FloatBuff("速度加成 {0:P}", -.5f, (model, buff) => {
                var p = model.velocity;
                p.value += buff.effect;
                model.velocity = p;
            })
        ),
        new Buff("SpeedUp", 
            new FloatBuff("速度加成 {0:P}", .1f, (model, buff) => {
                var p = model.velocity;
                p.value += buff.effect;
                model.velocity = p;
            })
        ),
        new Buff("Expansion",
            new FloatBuff("能源上限 {0}", 1000, (model, buff) => {
                var p = model.energy;
                p.max += buff.effect;
                model.energy = p;
            }),
            new FloatBuff("人口上限 {0}", 1000, (model, buff) => {
                var p = model.population;
                p.max += buff.effect;
                model.population = p;
            }),
            new FloatBuff("科技上限 {0}", 1000, (model, buff) => {
                var p = model.tech;
                p.max += buff.effect;
                model.tech = p;
            })
        ),
        new Buff("Reactor",
            new FloatBuff("能源增速 {0:P}", .1f, (model, buff) => {
                var p = model.energy;
                p.growth *= 1 + buff.effect;
                model.energy = p;
            })
        ),
        new Buff("Mining",
            new FloatBuff("科技增速 {0:P}", .3f, (model, buff) => {
                var p = model.tech;
                p.growth *= 1 + buff.effect;
                model.tech = p;
            })
        ),
        new Buff("Dungeons",
            new FloatBuff("人口增速 {0}", 100, (model, buff) => {
                var p = model.population;
                p.growth += buff.effect;
                model.population = p;
            })
        ),
        new Buff("EfficientEnergy",
            new FloatBuff("能源增速 {0:P}", -.05f, (model, buff) => {
                var p = model.energy;
                p.growth *= 1 + buff.effect;
                model.energy = p;
            })
        ),
        new Buff("Atmosphere",
            new FloatBuff("能源增速 {0:P}", -.005f, (model, buff) => {
                var p = model.energy;
                p.growth *= 1 + model.population.limitedValue / 100 * buff.effect;
                model.energy = p;
            })
        ),
    };

    public static Dictionary<string, Buff> s_BuffDict = s_BuffList.ToDictionary(buff => buff.name);

    public Dictionary<string, Buff> buffList { get; } = new Dictionary<string, Buff>();

    public bool AddBuff(string buffName)
    {
        if (s_BuffDict.ContainsKey(buffName))
            return false;
        if (buffList.ContainsKey(buffName))
            return false;

        buffList.Add(buffName, s_BuffDict[buffName]);
        ExecuteBuffEffects();
        return true;
    }

    public bool RemoveBuff(string buffName)
    {
        if (s_BuffDict.ContainsKey(buffName))
            return false;
        if (!buffList.ContainsKey(buffName))
            return false;

        buffList.Remove(buffName);
        ExecuteBuffEffects();
        return true;
    }

    private void ExecuteBuffEffects()
    {
        foreach (var buff in buffList.Values)
        {
            buff.BuffIt(this);
        }
    }

    public void Impact(float energy, float population, float tech)
    {
        FloatPropery p = this.energy;
        p.value += energy;
        this.energy = p;

        p = this.population;
        p.value += population;
        this.population = p;

        p = this.tech;
        p.value += tech;
        this.tech = p;
    }

    public FloatPropery velocity
    {
        get => m_Properties["velocity"];
        set
        {
            if (m_Properties["velocity"].Equals(value))
                return;

            m_Properties["velocity"] = value;
            ControlPanel.instance.UpdateTechInfo();
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
            ControlPanel.instance.UpdateTechInfo();
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
            ControlPanel.instance.UpdateTechInfo();
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
            ControlPanel.instance.UpdateTechInfo();
        }
    }
}
