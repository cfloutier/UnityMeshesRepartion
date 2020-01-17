using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshSurfaceDistribution : ExtendedDistribution
{
    public GameObject MeshPrefab;
    public List<float> Weights;

    Mesh mesh = null;
    Transform trMesh = null;

    float TriangleSurface(Vector3 A, Vector3 B, Vector3 C)
    {
        Vector3 V = Vector3.Cross(A - B, A - C);
        return V.magnitude * 0.5f;
    }

    void InitWeight()
    {
        if (MeshPrefab == null)
            return;

        if (mesh == null)
        {
            mesh = MeshPrefab.GetComponent<MeshFilter>().sharedMesh;
        }
        else
        {
            var newMesh = MeshPrefab.GetComponent<MeshFilter>().sharedMesh;
            if (newMesh != mesh)
            {
                mesh = newMesh;
            }
            else
                return;
        }

        Weights = new List<float>();
       
        var triangles = mesh.triangles;
        var points = mesh.vertices;
        float totalSurface = 0;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            float surface = TriangleSurface(points[triangles[i]], points[triangles[i + 1]], points[triangles[i + 2]]);
            totalSurface += surface;
            Weights.Add(totalSurface);
        }
    }

    int findTriangle(float surfaceProgress)
    {
        return Weights.BinarySearch(surfaceProgress);
    }

    public void pointOnTriangle(int triangleIndex)
    {
        triangleIndex *= 3;
        Vector3 A = points[triangles[triangleIndex]];
        Vector3 B = points[triangles[triangleIndex+1]];
        Vector3 C = points[triangles[triangleIndex+2]];

        Vector3 nA = points[triangles[triangleIndex]];
        Vector3 nB = points[triangles[triangleIndex+1]];
        Vector3 nC = points[triangles[triangleIndex+2]];

        float R = Random.Range(0, 1f);
        float S = Random.Range(0, 1f);

        if (R + S >= 1f)
        {
            R = 1 - R;
            S = 1 - S;
        }

        Vector3 AB = B - A;
        Vector3 AC = C - A;

        pt = A + R * AB + S * AC;
        norm = Vector3.Cross(AB.normalized, AC.normalized);
        if (norm == Vector3.zero)
            norm = Vector3.forward;
    }

    Vector3 pt;
    Vector3 norm;

    int[] triangles;
    Vector3[] points;

    protected override void Compute()
    {
        if (MeshPrefab == null)
            return;
        trMesh = MeshPrefab.transform;
        Random.InitState(RandomSeed + 500);
        InitLists();
        InitWeight();

        float totalSurface = Weights[Weights.Count - 1];

        triangles = mesh.triangles;
        points = mesh.vertices;
        Vector3 translate = trMesh.InverseTransformPoint(transform.position);
        for (int i = 0; i < Nb; i++)
        {
            float random = Random.Range(0, totalSurface);
            int triangleIndex = findTriangle(random);
            if (triangleIndex < 0)
                triangleIndex = -triangleIndex -1;

       //     Debug.Log(triangleIndex);

            pointOnTriangle(triangleIndex);

            Vector3 localPoint = trMesh.TransformPoint(pt + translate);
            localPoint = transform.InverseTransformPoint(localPoint) ;

            Vector3 normal = trMesh.TransformDirection(norm);

            positions.Add(localPoint);
            normals.Add(normal);
        }


        base.Compute();
    }
}