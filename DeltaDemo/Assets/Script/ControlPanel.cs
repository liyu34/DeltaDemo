using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FloatTweener<T>
{
    private T obj;
    private Func<T, float> getter;
    private Action<T, float> setter;
    private float from;
    private float to;
    private float t = 0;
    private float duration;
    private Action<T, float> onUpdate;
    private Action onFinished;

    public FloatTweener(Func<T, float> getter, Action<T, float> setter)
    {
        this.getter = getter;
        this.setter = setter;
    }

    public void Start(T obj, float from, float to, float duration, Action<T, float> onUpdate = null, Action onFinished = null)
    {
        if (this.obj != null)
        {
            setter(this.obj, this.to);
            this.onFinished?.Invoke();
        }

        t = 0;
        this.obj = obj;
        this.@from = from;
        this.to = to;
        this.duration = duration;
        this.onUpdate = onUpdate;
        this.onFinished = onFinished;
    }

    public void Update(float dt)
    {
        if (obj != null && t < duration)
        {
            t += dt;
            var now = getter(obj) + (to - from) / duration * dt;
            setter(obj, t < duration ? now : to);
            onUpdate?.Invoke(obj, t < duration ? t : duration);
            if (t >= duration) onFinished?.Invoke();
        }
    }
}

public class Vector2Tweener<T>
{
    private T obj;
    private Func<T, Vector2> getter;
    private Action<T, Vector2> setter;
    private Vector2 from;
    private Vector2 to;
    private float t = 0;
    private float duration;
    private Action<T, float> onUpdate;
    private Action onFinished;

    public Vector2Tweener(Func<T, Vector2> getter, Action<T, Vector2> setter)
    {
        this.getter = getter;
        this.setter = setter;
    }

    public void Start(T obj, Vector2 from, Vector2 to, float duration, Action<T, float> onUpdate = null, Action onFinished = null)
    {
        if (this.obj != null)
        {
            setter(this.obj, this.to);
            this.onFinished?.Invoke();
        }

        t = 0;
        this.obj = obj;
        this.@from = from;
        this.to = to;
        this.duration = duration;
        this.onUpdate = onUpdate;
        this.onFinished = onFinished;
    }

    public void Update(float dt)
    {
        if (obj != null && t < duration)
        {
            t += dt;
            var now = getter(obj) + (to - from) / duration * dt;
            setter(obj, t < duration ? now : to);
            onUpdate?.Invoke(obj, t < duration ? t : duration);
            if (t >= duration) onFinished?.Invoke();
        }
    }
}

public class ControlPanel : MonoBehaviour
{
    public static ControlPanel instance { get; private set; }

    public void Awake()
    {
        instance = this;
    }

    static Dictionary<State, string> s_TipsDict = new Dictionary<State, string>
    {
        {State.Hidden, "[Space] 打开面板"},
        {State.ShowCover, "[space] 继续航行  [←] 星球状况  [→] 科技树"},
        {State.ShowLeft, "[Space] 返回上层"},
        {State.ShowRight, "[Space] 返回上层  [Arrow] 选择科技  [Enter] 升级科技"},
    };

    public RectTransform rectTransform => transform as RectTransform;

    [Serializable]
    public struct LeftPanel
    {
        public RectTransform cover;
        public CanvasGroup panel;

        [Serializable]
        public struct TechInfo
        {
            [Serializable]
            public struct TechState
            {
                public Text total;
                public Text growth;
            }

            public TechState energy;
            public TechState population;
            public TechState tech;
        }

        public TechInfo techInfo;
    }

    [Serializable]
    public struct RightPanel
    {
        public RectTransform cover;
        public CanvasGroup panel;

        [Serializable]
        public struct TechTree
        {
            public RectTransform techMatrix;
            public Text techDetail;
        }

        public TechTree techTree;
    }

    [Serializable]
    public struct InfoBar
    {
        public Color benefitColorTint;
        public Color loseColorTint;
        public RectTransform root;
        public Slider energy;
        public Slider population;
        public Slider tech;
    }

    public InfoBar infoBar;

    [Serializable]
    public struct StatusInfo
    {
        public Text currentGalaxyName;
        public Text tips;
        public Slider progress;
    }

    [SerializeField] private StatusInfo statusInfo;

    [SerializeField] private LeftPanel leftPanel;
    [SerializeField] private RightPanel rightPanel;

    private Vector2Tweener<RectTransform> m_PositionTweener;

    private Vector2Tweener<RectTransform> positionTweener
    {
        get
        {
            if (m_PositionTweener is null)
            {
                m_PositionTweener = new Vector2Tweener<RectTransform>(rt => rt.anchoredPosition,
                    (rt, value) => { rt.anchoredPosition = value; });
            }

            return m_PositionTweener;
        }
    }

    private Vector2Tweener<RectTransform> m_SizeTweener;

    private Vector2Tweener<RectTransform> sizeTweener
    {
        get
        {
            if (m_SizeTweener is null)
            {
                m_SizeTweener = new Vector2Tweener<RectTransform>(rt => rt.sizeDelta,
                    (rt, value) => { rt.sizeDelta = value; });
            }

            return m_SizeTweener;
        }
    }

    private FloatTweener<CanvasGroup> m_AlphaTweener;

    private FloatTweener<CanvasGroup> alphaTweener
    {
        get
        {
            if (m_AlphaTweener is null)
            {
                m_AlphaTweener = new FloatTweener<CanvasGroup>(cg => cg.alpha, (cg, value) => { cg.alpha = value; });
            }

            return m_AlphaTweener;
        }
    }

    public enum State
    {
        Hidden,
        ShowCover,
        ShowLeft,
        ShowRight,
    }

    public enum Action
    {
        Cancel,
        Enter,
        Left,
        Right,
    }

    private State m_State = State.Hidden;

    private void SwitchState(Action action)
    {
        State oldState = m_State;
        switch (m_State)
        {
            case State.Hidden:
                if (action == Action.Enter)
                {
                    OnShow();
                    m_State = State.ShowCover;
                    statusInfo.tips.text = s_TipsDict[m_State];
                }

                break;
            case State.ShowCover:
                if (action == Action.Cancel)
                {
                    OnHide();
                    m_State = State.Hidden;
                    statusInfo.tips.text = s_TipsDict[m_State];
                }
                else if (action > Action.Enter)
                {
                    OnEnter(action);
                    ;
                    m_State = action == Action.Left ? State.ShowLeft : State.ShowRight;
                    statusInfo.tips.text = s_TipsDict[m_State];
                }

                break;
            case State.ShowLeft:
            case State.ShowRight:
                if (action == Action.Cancel)
                {
                    OnCancel();
                    m_State = State.ShowCover;
                    statusInfo.tips.text = s_TipsDict[m_State];
                }

                break;
        }

        Debug.Log($"{oldState} -({action})-> {m_State}");
    }

    public bool isActive
    {
        get { return m_State != State.Hidden; }
    }

    [SerializeField] private float m_TransformAnimSpeed = 1f;
    [SerializeField] private float m_AlphaAnimSpeed = 1f;

    public void OnEnterRoom()
    {
        statusInfo.currentGalaxyName.text = Constant.galaxyName[Main.instance.CurGalaxyNum];
    }

    void OnShow()
    {
        Main.instance.PauseGame();

        Vector2 from = rectTransform.anchoredPosition;
        Vector2 to = Vector2.up * rectTransform.rect.height;
        float duration = (to - from).magnitude / m_TransformAnimSpeed;
        positionTweener.Start(rectTransform, from, to, duration);
    }

    void OnHide()
    {
        Main.instance.ResumeGame();

        Vector2 from = rectTransform.anchoredPosition;
        Vector2 to = Vector2.zero;
        float duration = (to - from).magnitude / m_TransformAnimSpeed;
        positionTweener.Start(rectTransform, from, to, duration);
    }

    static Vector2 GetOriginSize(RectTransform rt)
    {
        RectTransform parent = rt.parent as RectTransform;
        Vector2 anchorMinPoint = Rect.NormalizedToPoint(parent.rect, rt.anchorMin);
        Vector2 anchorMaxPoint = Rect.NormalizedToPoint(parent.rect, rt.anchorMax);
        return anchorMaxPoint - anchorMinPoint;
    }

    void OnEnter(Action action)
    {
        if (m_State != State.ShowCover || action < Action.Left)
            return;

        RectTransform toSelected = action == Action.Left ? leftPanel.cover : rightPanel.cover;
        RectTransform toHidden = action == Action.Left ? rightPanel.cover : leftPanel.cover;
        CanvasGroup panelToShow = action == Action.Left ? rightPanel.panel : leftPanel.panel;

        Vector2 from = toSelected.sizeDelta;
        Vector2 to = Vector2.one * toSelected.rect.height - GetOriginSize(toSelected);
        float duration = (to - from).magnitude / m_TransformAnimSpeed;
        sizeTweener.Start(toSelected, from, to, duration);

        toHidden.gameObject.SetActive(false);

        panelToShow.gameObject.SetActive(true);
        alphaTweener.Start(panelToShow, panelToShow.alpha = 0.0f, 1.0f, 1f / m_AlphaAnimSpeed, (obj, t) =>
        {
            if (t > Time.deltaTime)
                return;

            if (m_State == State.ShowRight)
            {
                var techMatrix = rightPanel.techTree.techMatrix;
                for(int i = 0; i < techMatrix.childCount; ++i)
                {
                    var techItem = techMatrix.GetChild(i);
                    bool isLearned = EarthModel.instance.buffList.ContainsKey(techItem.name);
                    var learned = GameObject.Find($"{techItem.name}/unlearned");
                    var img = learned?.GetComponent<Image>();
                    if (img) img.enabled = !isLearned;
                    var unlearned = GameObject.Find($"{techItem.name}/learned");
                    img = unlearned?.GetComponent<Image>();
                    if (img) img.enabled = isLearned;
                }
                EventSystem.current.SetSelectedGameObject(techMatrix.GetChild(0).gameObject);
            }
        });

        Debug.Log($"{toSelected.name}, {from}, {to}, {duration}");
    }

    void OnCancel()
    {
        if (m_State < State.ShowLeft)
            return;

        RectTransform selected = m_State == State.ShowLeft ? leftPanel.cover : rightPanel.cover;
        RectTransform hidden = m_State == State.ShowLeft ? rightPanel.cover : leftPanel.cover;
        CanvasGroup panelToShow = m_State == State.ShowLeft ? rightPanel.panel : leftPanel.panel;

        Vector2 from = selected.sizeDelta;
        Vector2 to = Vector2.zero;
        float duration = (to - from).magnitude / m_TransformAnimSpeed;
        sizeTweener.Start(selected, from, to, duration);

        panelToShow.gameObject.SetActive(false);

        hidden.gameObject.SetActive(true);
        alphaTweener.Start(panelToShow, panelToShow.alpha = 0.0f, 1.0f, 1f / m_AlphaAnimSpeed);

        Debug.Log($"{selected.name}, {from}, {to}, {duration}");
    }

    public void OnSelectTech(string buffName)
    {
        if (m_State != State.ShowRight)
            return;

        if (EarthModel.s_BuffDict.ContainsKey(buffName))
        {
            rightPanel.techTree.techDetail.text = EarthModel.s_BuffDict[buffName].desc;
        }
    }

    public void OnLearnTech(string buffName)
    {
        const float COST = 5000;
        if (EarthModel.instance.population.limitedValue <= COST)
        {
            FloatPropery p = EarthModel.instance.population;
            p.value += COST - EarthModel.instance.defense.limitedValue;
            EarthModel.instance.population = p;
            EarthModel.instance.AddBuff(buffName);
            OnImpact(true);
        }
    }

    private void InfoBarTint(Color colorTint, float duration, bool reverse)
    {
        var graphics = infoBar.root.GetComponentsInChildren<Graphic>();
        foreach (var graphic in graphics)
        {
            graphic.CrossFadeColor(reverse ? Color.white : colorTint, duration, true, true);
        }
    }

    public void OnImpact(bool benefit)
    {
        const float COLOR_TINT_DURATION = 0.1f;
        Color colorTint = benefit ? infoBar.benefitColorTint : infoBar.loseColorTint;
        InfoBarTint(colorTint, COLOR_TINT_DURATION, false);
        DelayInvoke(COLOR_TINT_DURATION, () => InfoBarTint(colorTint, COLOR_TINT_DURATION, true));
    }

    private void DelayInvoke(float delay, System.Action func)
    {
        StartCoroutine(DelayInvoker(delay, func));
    }

    private IEnumerator DelayInvoker(float delay, System.Action func)
    {
        yield return new WaitForSeconds(delay);

        func?.Invoke();
    }

    public void OnBuff(string buffName)
    {
        if (m_State == State.ShowRight)
        {
            var unlearned = GameObject.Find($"{buffName}/unlearned");
            var img = unlearned?.GetComponent<Image>();
            if (img) img.enabled = false;
            var learned = GameObject.Find($"{buffName}/learned");
            img = learned?.GetComponent<Image>();
            if (img) img.enabled = true;
        }
    }

    public void UpdateTechInfo()
    {
        var model = EarthModel.instance;
        infoBar.energy.value = model.energy.limitedValue;
        infoBar.energy.maxValue = model.energy.max;
        infoBar.population.value = model.population.limitedValue;
        infoBar.population.maxValue = model.population.max;
        infoBar.tech.value = model.tech.limitedValue;
        infoBar.tech.maxValue = model.tech.max;

        if (m_State == State.ShowLeft)
        {
            var techInfo = leftPanel.techInfo;

            string sign = "+";
            techInfo.energy.total.text = $"动力能源：{model.energy.limitedValue}/{model.energy.max}";
            sign = model.energy.growth > 0 ? "+" : "-";
            techInfo.energy.growth.text = sign + $"{Mathf.Abs(model.energy.growth)} / s";

            techInfo.population.total.text = $"人口数量：{model.population.limitedValue}/{model.population.max}";
            sign = model.population.growth > 0 ? "+" : "-";
            techInfo.population.growth.text = sign + $"{Mathf.Abs(model.population.growth)} / s";

            techInfo.tech.total.text = $"科研资源：{model.tech.limitedValue}/{model.tech.max}";
            sign = model.tech.growth > 0 ? "+" : "-";
            techInfo.tech.growth.text = sign + $"{Mathf.Abs(model.tech.growth)} / s";
        }
    }

    // Update is called once per frame
    public void Update()
    {
        statusInfo.progress.value = Main.instance.EarthProgress();
        statusInfo.progress.maxValue = 1f;

        UpdateTechInfo();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Action action = isActive ? Action.Cancel : Action.Enter;
            SwitchState(action);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SwitchState(Action.Left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SwitchState(Action.Right);
        }

        positionTweener.Update(Time.deltaTime);
        sizeTweener.Update(Time.deltaTime);
        alphaTweener.Update(Time.deltaTime);
    }
}
