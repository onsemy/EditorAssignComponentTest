using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TestAssignAttribute : System.Attribute
{
    public readonly string path;
    public TestAssignAttribute()
    {
        this.path = null;
    }
}
