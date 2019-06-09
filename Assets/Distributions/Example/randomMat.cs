using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class randomMat : MonoBehaviour
{
    public Material[] mats;

    [ButtonsBar("Apply")]
    public bool bt;

    void Apply()
    {
        if (mats.Length == 0) return;
        for (int i = 0; i < transform.childCount; i++)
        {
            int index = Random.Range(0, mats.Length);
            var rend = transform.GetChild(i).GetComponent<Renderer>();
            if (rend == null)
                continue;

            rend.sharedMaterial = mats[index];
        }
    }
}
