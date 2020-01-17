using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


/// <summary>
/// a class used to compute the first simples polyhedon vertexes, edges and faces
/// </summary>
public class PlatonSolidsTester : MonoBehaviour
{
    public PlatoSolid.Type type;

    PlatoSolid solid = null;

    void Build()
    {
        solid = new PlatoSolid();
        solid.type = type;
        solid.Build();
    }

    private void OnValidate()
    {
        Build();
    }

    public int Face = -1;


    [ButtonsBar("Prev", "Next")]
    public bool bt;

    void Prev() { Face--; UnityEditorInternal.InternalEditorUtility.RepaintAllViews(); }
    void Next() { Face++; UnityEditorInternal.InternalEditorUtility.RepaintAllViews(); }


    private void OnDrawGizmosSelected()
    {
        if (solid == null)
            return;

        if (Face < 0)
            Face = 0;
        if (Face > solid.faces.Count)
        {
            Face = solid.faces.Count;
        }

        Gizmos.color = Color.white;
        Handles.color = Color.black;

        for (int i = 0; i < solid.points.Count; i++)
        {
            var p = solid.points[i];
            p = transform.TransformPoint(p);


            Gizmos.DrawSphere(p, 0.05f * HandleUtility.GetHandleSize(p));


            Handles.Label(p, "" + i);
        }
        Gizmos.color = Color.cyan;

        for (int i = 0; i < solid.edges.Count; i++)
        {
            var e = solid.edges[i];

            var p1 = transform.TransformPoint(solid.points[e.start]);
            var p2 = transform.TransformPoint(solid.points[e.end]);
            Gizmos.DrawLine(p1, p2);
        }

        Gizmos.color = Color.red;

        if (Face > 0)
        {
            var face = solid.faces[Face - 1];

            for (int i = 0; i < face.Count; i++)
            {
                int start;
                int end;
                if (i == face.Count - 1)
                {
                    start = face[i];
                    end = face[0];
                }
                else
                {
                    start = face[i];
                    end = face[i + 1];
                }

                var p1 = transform.TransformPoint(solid.points[start]);
                var p2 = transform.TransformPoint(solid.points[end]);

                Gizmos.DrawLine(p1, p2);
            }

            var center = transform.TransformPoint(face.center);
            var norm = transform.TransformDirection(face.normal);
            Gizmos.DrawSphere(center, 0.05f * HandleUtility.GetHandleSize(center));

            Gizmos.DrawLine(center, center + norm * HandleUtility.GetHandleSize(center));
        }
    }
}
