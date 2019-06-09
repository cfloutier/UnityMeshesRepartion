using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple random distribution depending on a radius (distance)
/// </summary>
public class RadialDistribution : Distribution
{
    public enum Mode
    {
        SphereSurface,
        SphereVolume,
        Circle
    }

    [Header("Radial Distribution")]
    public float radius = 1;

    public Mode mode;

    public bool RescaleByRadius = false;

    [Visible("RescaleByRadius")]
    public Vector3 minScale = Vector3.one * 0.5f;
    [Visible("RescaleByRadius")]
    public Vector3 maxScale = Vector3.one;
    [Visible("RescaleByRadius")]
    public AnimationCurve radiusScaleCurve = AnimationCurve.Linear(0, 1, 1, 0);

    protected override void Compute()
    {
        Random.InitState(RandomSeed + 500);

        InitLists();
        if (Nb < 1)
            Nb = 1;

        if (radius < 0)
            radius = 0;

        for (int i = 0; i < Nb; i++)
        {
            switch (mode)
            {
                case Mode.SphereSurface:
                    positions.Add(Random.onUnitSphere * radius);
                    normals.Add(positions[i].normalized);
                    break;
                case Mode.SphereVolume:
                    positions.Add(Random.insideUnitSphere * radius);
                    normals.Add(positions[i].normalized);
                    break;
                case Mode.Circle:
                    Vector3 pos = Random.insideUnitCircle * radius;
                    positions.Add(new Vector3(pos.x, 0, pos.y));
                    normals.Add(positions[i].normalized);
                    break;
                default:
                    break;
            }
        }


        if (RescaleByRadius)
        {
            for (int i = 0; i < Nb; i++)
            {
                float value = positions[i].magnitude / radius;
                value = radiusScaleCurve.Evaluate(value);
                scales.Add(Vector3.Lerp(minScale, maxScale, value));
            }
        }

        base.Compute();
    }
}
