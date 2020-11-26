using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class ExSystem
{
    public static Type GetMemberType(this MemberInfo info)
    {
        switch (info.MemberType)
        {
            case MemberTypes.Event:
                var e = info as EventInfo;
                return e?.EventHandlerType;

            case MemberTypes.Field:
                var f = info as FieldInfo;
                return f?.FieldType;

            case MemberTypes.Method:
                var m = info as MethodInfo;
                return m?.ReturnType;

            case MemberTypes.Property:
                var p = info as PropertyInfo;
                return p?.PropertyType;

            case MemberTypes.Constructor:
            case MemberTypes.TypeInfo:
            case MemberTypes.Custom:
            case MemberTypes.NestedType:
            case MemberTypes.All:
            default:
                return null;
        }
    }
    
    public static void SetValue(this MemberInfo info, object obj, object value)
    {
        switch (info.MemberType)
        {
            case MemberTypes.Field:
                var f = (info as FieldInfo);
                f.SetValue(obj, value);
                break;

            case MemberTypes.Property:
                var p = (info as PropertyInfo);
                p.SetValue(obj, value, null);
                break;

            case MemberTypes.Constructor:
            case MemberTypes.Method:
            case MemberTypes.Event:
            case MemberTypes.TypeInfo:
            case MemberTypes.Custom:
            case MemberTypes.NestedType:
            case MemberTypes.All:
            default:
                break;
        }
    }
}

[CustomEditor(typeof(TestWindow), true)]
public class TestWindowEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var behaviour = (TestWindow) target;
        if (GUILayout.Button("Assign Components"))
        {
            var behaviour_type = behaviour.GetType();
            var comp_type = typeof(Component);
            var path_type = typeof(TestAssignAttribute);

            var members = behaviour_type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                          .Where(m =>
                                 (m.MemberType == MemberTypes.Field || m.MemberType == MemberTypes.Property)
                                 && m.GetMemberType().IsSubclassOf(comp_type)
                                 && m.GetCustomAttributes(path_type, true).Length == 1
                                );

            foreach (var item in members)
            {
                var attribute = item.GetCustomAttributes(path_type, true)[0] as TestAssignAttribute;
                var path = item.Name;
                var member_type = item.GetMemberType();

                var child = behaviour.transform.Find(path);

                if (child == null)
                {
                    // NOTE(JJO): 모든 자식들로부터 찾아본다. 
                    var childList = behaviour.transform.GetComponentsInChildren<Transform>(true);
                    foreach (var t in childList)
                    {
                        if (t.name.Equals(path))
                        {
                            child = t;
                            break;
                        }
                    }

                    if (child == null)
                    {
                        UnityEngine.Debug.LogError($"can't find child in {path}");
                        continue;
                    }
                }

                var member_comp = child.GetComponent(member_type);

                if (member_comp == null)
                    member_comp = child.gameObject.AddComponent(member_type);

                if (member_comp == null)
                {
                    UnityEngine.Debug.LogError($"can't find componnet {path} {member_type}");
                    continue;
                }

                item.SetValue(behaviour, member_comp);
            }
        }
    }
}
