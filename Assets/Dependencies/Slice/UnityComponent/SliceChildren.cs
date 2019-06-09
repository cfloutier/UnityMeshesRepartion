using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;


// a simple Slicer of child objects.
// each object must be convex and have a pivot in its center
// and each transform could not have a non unifom scale (x != y != z)
public class SliceChildren : MonoBehaviour
{
    public enum SourceMode
    {
        Shown,
        Hide,
        Destroy
    }


    public SourceMode sourceMode = SourceMode.Hide;

  //  public bool slicesOnly;
    public bool mergeResult;

    public Material sliceMaterial = null;

    public GameObject slicerQuad;

    [ButtonsBar("Slice")]
    public bool bt;

    Transform resultUp;
    Transform resultDown;

    Transform sources;

    Vector3 QuadPos;
    Vector3 QuadDir;

    public void Slice()
    {    
        checkSource();
        checkResultTR();

        QuadPos = slicerQuad.transform.position;
        QuadDir = -slicerQuad.transform.forward;

        var mat = sliceMaterial;
        if (mat == null)
        {
           var rend = transform.GetComponentInChildren<MeshRenderer>();
            if (rend != null)
                mat = rend.sharedMaterial;
        }

        for (int i = 0; i < sources.childCount; i++)
        {
            var sourceObj = sources.GetChild(i).gameObject;
            var localQuadPos = sourceObj.transform.TransformPoint(QuadPos);
            var localQuadDir = sourceObj.transform.TransformDirection(QuadDir);

            var objects = sources.GetChild(i).gameObject.SliceInstantiate(QuadPos, QuadDir, mat);
            if (objects != null)
            {
                foreach (var o in objects)
                {
                    if (o.name.StartsWith("Upper"))
                    {
                        o.transform.parent = resultUp;
                    }
                    else
                    {
                        o.transform.parent = resultDown;
                    }

                    o.transform.localPosition = sourceObj.transform.localPosition;
                    o.transform.localRotation = sourceObj.transform.localRotation;
                    o.transform.localEulerAngles = sourceObj.transform.localEulerAngles;
                }
            }
            else //if (!slicesOnly)
            {
                var o = Instantiate(sourceObj);              

                Vector3 deltaPos = sourceObj.transform.position - QuadPos;
                float direction = Vector3.Dot(deltaPos, QuadDir);

                if (direction >= 0 )
                {
                    o.transform.parent = resultUp;
                }
                else
                {
                    o.transform.parent = resultDown;
                }

                o.transform.localPosition = sourceObj.transform.localPosition;
                o.transform.localRotation = sourceObj.transform.localRotation;
                o.transform.localScale = sourceObj.transform.localScale;
            }        
        }

        switch (sourceMode)
        {
            default:
            case SourceMode.Shown:
                break;
            case SourceMode.Hide:
                sources.gameObject.SetActive(false);
                break;
            case SourceMode.Destroy:
                sources.gameObject.SafeDestroy();
                break;
           
        }

        if (mergeResult)
        {
            MeshFusion.MeshMerge.MergeAllChilds(resultUp);
            MeshFusion.MeshMerge.MergeAllChilds(resultDown);
        }
    }

    void checkSource()
    {
        if (sources == null)
        {
            sources = transform.Find("Sources");
        }

        if (sources == null)
        {
            sources = new GameObject().transform;
            sources.name = "Sources";
            sources.parent = transform;
            sources.SetIdentity();
            sources.parent = transform.parent;

            while (transform.childCount > 0)
            {
                transform.GetChild(0).parent = sources;
            }

            sources.parent = transform;
        }
    }

    void checkResultTR()
    {
        if (resultUp != null)
            DestroyImmediate(resultUp.gameObject);
        

        resultUp = new GameObject().transform;
        resultUp.name = "Up";
        resultUp.parent = transform;
        resultUp.SetIdentity();

        if (resultDown != null)
            DestroyImmediate(resultDown.gameObject);

        resultDown = new GameObject().transform;
        resultDown.name = "Down";
        resultDown.parent = transform;
        resultDown.SetIdentity();


    }
}
