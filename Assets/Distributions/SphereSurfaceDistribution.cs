using System.Collections.Generic;
using UnityEngine;


public class SphereSurfaceDistribution : Distribution
{
    [Header("Sphere Surface")]
    public float radius = 1;
    public bool random = false;

 
    public bool addExtremums = false;
    [Visible("random", true)]
    public float rotation = 0;

    void addPoint(Vector3 normal)
    {
        positions.Add(normal * radius);
        normals.Add(normal);
    }

    protected override  void Compute()
    {
     
        Random.InitState(RandomSeed+500);

        InitLists();
        if (Nb < 1)
            Nb = 1;

        if (radius < 0)
            radius = 0;

        if (random)
        {
            for (int i = 0; i < Nb; i++)
            {
                addPoint( Random.onUnitSphere);
            }
        }
        else
        {
            int k;                /* index */
            float phi1, phi, theta, h, x, y, z;

            phi1 = 0.0f;

            if (addExtremums)
                addPoint(new Vector3(0, 0, -1));
     
            for (k = 2; k <= Nb - 1; k++)
            {
                h = -1 + 2 * (k - 1) / (float)(Nb - 1);
                theta = Mathf.Acos(h);

                if (theta < 0 || theta > Mathf.PI)
                {
                    Debug.LogError("Invalid value");
                    return;
                }

                phi = phi1 + 3.6f / (Mathf.Sqrt((float)Nb * (1 - h * h)));
                phi = phi % (2 * Mathf.PI);
                phi1 = phi;

                phi += rotation;

                x = Mathf.Cos(phi) * Mathf.Sin(theta);
                y = Mathf.Sin(phi) * Mathf.Sin(theta);
                /* z = cos ( theta ); But z==h, so: */
                z = h;

                addPoint(new Vector3(x, y, z));             
            }
            if (Nb > 1 && addExtremums)
            {
                addPoint(new Vector3(0, 0, 1));
            }
        }

        base.Compute();
    }

}
