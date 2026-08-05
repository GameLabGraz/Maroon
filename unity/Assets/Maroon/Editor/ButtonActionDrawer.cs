using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;

public class ButtonActionDrawer : PropertyDrawer
{
    const float rows = 3;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty targetProperty = property.FindPropertyRelative("target");
        SerializedProperty methodNameProperty = property.FindPropertyRelative("method");
        SerializedProperty candidateNamesProperty = property.FindPropertyRelative("candidates");
        SerializedProperty indexProperty = property.FindPropertyRelative("index");

        // pass through label
        EditorGUIUtility.LookLikeControls();
        EditorGUI.LabelField(new Rect(position.x, position.y, position.width / 2, position.height / rows), label);

        // target + method section
        EditorGUI.indentLevel++;
        EditorGUI.BeginChangeCheck();

        // slect target
        EditorGUI.PropertyField(
            new Rect(position.x, position.y += position.height/rows, position.width, position.height/ rows),
            targetProperty
            );

        if (targetProperty.objectReferenceValue == null)
            return; // null objects have no methods - don't continue

        // populate method condidate names
        string[] methodCandidateNames;
        if (EditorGUI.EndChangeCheck())
        {
            methodCandidateNames = RepopulateCandidateList(targetProperty, candidateNamesProperty, indexProperty);
        }
        else
        {
            methodCandidateNames = new string[candidateNamesProperty.arraySize];
            int i = 0;
            foreach (SerializedProperty element in candidateNamesProperty)
            {
                methodCandidateNames[i++] = element.stringValue;
            }
        }

        // place holder when no candidates are available
        if(methodCandidateNames.Length == 0)
        {
            EditorGUI.LabelField(
                new Rect(position.x, position.y += position.height/rows, position.width, position.height/rows),
                "Method",
                "none"
                );
            return; // no names no games
        }

        // select method from candidates
        indexProperty.intValue = EditorGUI.Popup(
            new Rect(position.x, position.y += position.height / rows, position.width, position.height / rows),
            "Method (" + targetProperty.objectReferenceValue.GetType().ToString() + ")",
            indexProperty.intValue,
            methodCandidateNames
            );

        methodNameProperty.stringValue = methodCandidateNames[indexProperty.intValue];
        EditorGUI.indentLevel--;
    }

    public string[] RepopulateCandidateList(
        SerializedProperty targetProperty,
        SerializedProperty candidateNamesProperty,
        SerializedProperty indexProperty
        )
    {
        System.Type type = targetProperty.objectReferenceValue.GetType();
        System.Type[] paramTypes = this.paramTypes;
        IList<MemberInfo> candidateList = new List<MemberInfo>();
        string[] candidateNames;
        int i = 0;

        Debug.Log("Candidate Criteria:");
        Debug.Log("\treturn type:" + returnType.ToString());
        Debug.Log("\tparam count:" + paramTypes.Length);
        foreach (System.Type paramType in paramTypes)
            Debug.Log("\t\t" + paramType.ToString());

        type.FindMembers(
            MemberTypes.Method,
            BindingFlags.Instance | BindingFlags.Public,
            (member, criteria) =>
            {
                Debug.Log("matching " + member.Name);
                MethodInfo method;
                if ((method = type.GetMethod(member.Name, paramTypes)) != null && method.ReturnType == returnType)
                {
                    candidateList.Add(method);
                    return true;
                }
                return false;
            },
            null
        );

        // clear/resize/initalize storage containers
        candidateNamesProperty.ClearArray();
        candidateNamesProperty.arraySize = candidateList.Count;
        candidateNames = new string[candidateList.Count];

        // assign storage containers
        i = 0;
        foreach(SerializedProperty element in candidateNamesProperty)
        {
            element.stringValue = candidateNames[i] = candidateList[i++].Name;
        }

        // reset popup index
        indexProperty.intValue = 0;

        return candidateNames;
    }

    public System.Type returnType
    {
        get
        {
            return attribute != null ? (attribute as ButtonActionAttribute).returnType : typeof(void);
        }
    }

    public System.Type[] paramTypes
    {
        get
        {
            return (attribute != null && (attribute as ButtonActionAttribute).paramTypes != null) ? (attribute as ButtonActionAttribute).paramTypes : new System.Type[0];
        }
    }

    public System.Delegate method
    {
        get
        {
            return attribute != null ? (attribute as ButtonActionAttribute).method : null;
        }
        set
        {
            (attribute as ButtonActionAttribute).method = value;
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label) * rows;
    }
}
