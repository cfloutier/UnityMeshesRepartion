using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildOrClean : MonoBehaviour
{
    [ButtonsBar("CleanAll", "BuildAll", "RemoveAutoBuild")]
    public bool bt;

    [ButtonsBar( "RemoveAutoBuild", "SetAutoBuild")]
    public bool bt2;


    void CleanAll()
    {
        var distribs = FindObjectsOfType<Distribution>();
        foreach (var d in distribs)
            d.Clean();
    }

    void BuildAll()
    {
        var distribs = FindObjectsOfType<Distribution>();
        foreach (var d in distribs)
            d.Build();
    }

    void RemoveAutoBuild()
    {
        var distribs = FindObjectsOfType<Distribution>();
        foreach (var d in distribs)
            d.BuildOnEditorChange = false;
    }

    void SetAutoBuild()
    {
        var distribs = FindObjectsOfType<Distribution>();
        foreach (var d in distribs)
            d.BuildOnEditorChange = true;
    }

}
