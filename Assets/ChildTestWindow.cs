using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChildTestWindow : TestWindow
{
    [TestAssign] public Text text;
    [TestAssign] public Image image;
    
    // Start is called before the first frame update
    private void Start()
    {
        Debug.Log($"{text != null} | {image != null}");
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
