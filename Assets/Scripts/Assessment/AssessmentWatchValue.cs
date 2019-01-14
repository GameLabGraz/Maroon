using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Evaluation.UnityInterface;

public class AssessmentWatchValue : IAssessmentValue {

    //public bool ContinousUpdate = false;
    [TextArea]
    public string Attributes = "";

    [HideInInspector]
    public bool Internal = false;

    private List<Property> properties = new List<Property>();


    public delegate void OnValueRegisteredHandler(IterationResult result);
    public static event OnValueRegisteredHandler OnValueRegistered;

    // Use this for initialization
    void Start() {
        
        foreach(string line in Attributes.Split('\n'))
        {
            if (line.Trim() == "")
                continue;


            Property prop = new Property(line);
            string[] parts = line.Split('.');
            string ComponentName = parts[0];
            if (!Internal && (prop.RootComponent = GetComponent(ComponentName)) == null )
                throw new Exception(String.Format("The component '{0}' was not found on the object '{1}'. Use the Assessment Manager to watch global variables.", ComponentName, name));
            else if (Internal && (prop.StaticObject = GetGlobalTypeByName(ComponentName)) == null) {
                throw new Exception(String.Format("The Object '{0}' was not found in the global context.", ComponentName, name));
            }

            parts = parts.Skip(1).ToArray();
            foreach (string part in parts)
            {
               // Debug.Log("Type " + prop.GetType());
                var ppi = GetMember(part, prop.GetType());

                if (ppi == null)
                    throw new Exception(String.Format("The attribute '{0}.{1}' was not found", prop.ToString(), part));

                prop.ParentLine.Add(ppi);
            }

            if(! prop.IsOK) 
                throw new Exception(String.Format("At least one property needs to be specified for {0}, line: {1}", prop.RootComponent.name, line));
            else
                properties.Add(prop);
        }

        if (OnValueRegistered != null)
            OnValueRegistered(AssessmentManager.Instance.RegisterValue(this));
    }
	
    public static ParentProp GetMember(string name, Type ParentType)
    {
        ParentProp ret = null;
        foreach (PropertyInfo pi in ParentType.GetProperties())
        {
            //Debug.Log(String.Format("{0}, {1}", name, pi.Name.ToLower()));
            if (pi.Name.ToLower() == name.ToLower())
            {
                ret = new ParentProp(pi);
                break;
            }
        }


        if ( ret == null)
            foreach (FieldInfo fi in ParentType.GetFields())
            {
                //Debug.Log(String.Format("{0}, {1}", name, fi.Name.ToLower()));
                if (fi.Name.ToLower() == name.ToLower())
                {
                    ret = new ParentProp(fi);
                    break;
                }
            }

        return ret;
    }

    public static Type GetGlobalTypeByName(string name)
    {
        foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (Type t in a.GetTypes())
            {
                if (t.Name == name)
                    return t;
            }
        }

        

        return Type.GetType(name);
    }

	// Update is called once per frame
	void Update () {

        if (!ContinuousUpdate)
            foreach (Property prop in properties)
                prop.Update();
        else 
            AssessmentManager.Instance.Send(GameEvent);
    }

    public override GameEvent GameEvent {
        get {
            GameEvent ev = new GameEvent();

            foreach (Property prop in properties)
            {
                prop.Update();
                ev.Add(GameEventBuilder.EnvironmentVariable(this.gameObject.name, prop.FullName, prop.GetValue));
            }

            return ev;
        }
        set {
            throw new AccessViolationException("Set GameEvent is not allowed on this object. GameEvent gets calculated.");
        }
    }

    public class Property
    {
        public string FullName;
        public Component RootComponent = null;
        public Type StaticObject = null;
        public List<ParentProp> ParentLine;
        private object lastVal;
        private object currentVal;
        private bool dirty;


        public Property(string name)
        {
            FullName = name;
            ParentLine = new List<ParentProp>();
        }

        public Property(Property other)
        {
            FullName = other.FullName;
            ParentLine = new List<ParentProp>(other.ParentLine);
        }

        private object intGetValue(int i)
        {
            if (i == 0)
                return ParentLine[0].GetValue(RootComponent, null);
            else
                return ParentLine[i].GetValue(intGetValue(i - 1), null);
        }
        
        public new Type GetType()
        {
            if (ParentLine.Count > 0)
                return ParentLine[ParentLine.Count - 1].PropertyType;
            else if (RootComponent != null)
                return RootComponent.GetType();
            else if (StaticObject != null)
                return StaticObject;
            else
                return null;
        }

        public bool IsOK {
            get {
                return (ParentLine.Count > 0);
            }
        }
       
        public bool IsDirty {
            get {
                return dirty;
            }
        }

        public object GetValue {
            get {
                return currentVal;
            }
        }

        public void Update()
        {
            //Debug.Log("Updating " + FullName);
            if (!IsOK)
                currentVal = null;
            else
                currentVal = intGetValue(ParentLine.Count - 1);

            dirty = (lastVal != currentVal);
            lastVal = currentVal;
        }
    }

    public class ParentProp
    {
        private MemberInfo info;
        private Type type;
        public Type PropertyType {
            get {
                return (type == typeof(PropertyInfo)) ? ((PropertyInfo)info).PropertyType : ((FieldInfo)info).FieldType;
            }
        }

        public ParentProp(PropertyInfo pi)
        {
            info = (MemberInfo)pi;
            type = typeof(PropertyInfo);
        }
        public ParentProp(FieldInfo fi)
        {
            info = (MemberInfo)fi;
            type = typeof(FieldInfo);
        }

        public new Type GetType()
        {
            return type;
        }
        
        public object GetValue(object obj, object[] index)
        {
            //Debug.Log(String.Format("getValue: {0}, {1}", obj, type));
            if (type == typeof(PropertyInfo))
                return ((PropertyInfo)info).GetValue(obj, index);
            else
                return ((FieldInfo)info).GetValue(obj);
        }

    }
}


