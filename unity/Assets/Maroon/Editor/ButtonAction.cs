using UnityEngine;

[System.Serializable]
public class ButtonAction<T>
{
    // public settings
    public Object target;
    public string method;

    // inspactor cache
    public string[] candidates = { };
    public int index;

    // invocation
    public System.Action<T> action;

    public void Awake()
    {
        action = System.Action<T>.CreateDelegate(typeof(System.Action<T>), target, target.GetType().GetMethod(method)) as System.Action<T>;
    }
}

public class ButtonActionAttribute : PropertyAttribute
{
    public System.Type returnType;
    public System.Type[] paramTypes;
    public ButtonActionAttribute(System.Type returnType = null, params System.Type[] paramTypes)
    {
        this.returnType = returnType != null ? returnType : typeof(void);
        this.paramTypes = paramTypes != null ? paramTypes : new System.Type[0];
    }

    public System.Delegate method;
}
