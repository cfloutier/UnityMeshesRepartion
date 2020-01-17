using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatoSolidDistribution : Distribution
{
    [Header("Plato Solid Distribution Params")]
    public PlatoSolid.Type solidType;


    public float radius = 1;

    public int nbInterations = 1;
    public float deltaAngle = 1;
    public float deltaSize = 0.9f;

    public float sizeEdge = 1;

    public float AddedStartLenght = 0.1f;
    public float AddedEndLenght = 0.1f;

    public bool edgeNormal = false;

    void addEdge(Vector3 pa, Vector3 pb)
    {
        Vector3 delta = pb - pa;

        pa = pa - delta * AddedStartLenght;
        pb = pb + delta * AddedEndLenght;
        delta = pb - pa;
        Vector3 pos = (pa + pb) / 2;

        pos = pos * radius;

        Quaternion rot = Quaternion.identity;
        if (edgeNormal)
            rot = Quaternion.LookRotation(delta, pos.normalized);
        else
            rot = Quaternion.LookRotation(delta);

        Vector3 scale = new Vector3(sizeEdge, sizeEdge, delta.magnitude * radius);

        positions.Add(pos);
        rotations.Add(rot);
        scales.Add(scale);
    }

    void addFace(PlatoSolid.Face face)
    {
        var points = solid.points;

        float ratio = 1;
        float angle = 0;
        Quaternion rot = Quaternion.identity;
        for (int i = 0; i < nbInterations; i++)
        {
            rot = Quaternion.AngleAxis(angle, face.normal);

            for (int index = 0; index < face.Count; index++)
            {
                var pa = solid.points[face[index]];
                var pb = index < face.Count - 1 ? solid.points[face[index + 1]] : solid.points[face[0]];

                pa *= ratio;
                pb *= ratio;

                pa = rot * pa;
                pb = rot * pb;

                addEdge(pa, pb);
            }

            ratio = ratio - deltaSize;
            angle += deltaAngle;
        }
    }

    PlatoSolid solid;
    protected override void Compute()
    {
        InitLists();

        solid = new PlatoSolid();
        solid.type = solidType;
        solid.Build();

        if (nbInterations < 1)
            nbInterations = 1;
        else if (nbInterations > 40)
            nbInterations = 40;


        // face
        foreach (var f in solid.faces)
        {
            addFace(f);
        }

        base.Compute();
    }
}
