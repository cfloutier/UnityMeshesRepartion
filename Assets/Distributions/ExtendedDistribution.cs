using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MeshFusion;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class ExtendedDistribution : Distribution
{


    public enum Scale
    {
        NoChange,
        Uniform,
        Rescale,
        RandomUniform,
        FullRandom,
        Progress
    }

    [Header("Scaling Mode")]
    public Scale scaling = Scale.NoChange;

    protected bool ShowUniform() { return scaling == Scale.Uniform; }
    [Visible("ShowUniform()")]
    public float size = 1;

    protected bool ShowRescale() { return scaling == Scale.Rescale; }
    [Visible("ShowRescale()")]
    public Vector3 scale = new Vector3(1, 1, 1);

    protected bool ShowScaleUniform() { return scaling == Scale.RandomUniform; }
    [Visible("ShowScaleUniform()")]
    public Vector2 rangeScaleUniform = new Vector2(1, 2);

    protected bool ShowScaleFull() { return scaling == Scale.FullRandom || scaling == Scale.Progress; }
    [Visible("ShowScaleFull()")]
    public Vector3 rangeScale_Min = new Vector3(1, 1, 1);
    [Visible("ShowScaleFull()")]
    public Vector3 rangeScale_Max = new Vector3(1, 1, 1);

    protected bool ShowCurveScale() { return scaling == Scale.Progress; }
    [Visible("ShowCurveScale()")]
    public AnimationCurve scaleCurve = AnimationCurve.Linear(0, 0, 1, 1);

    public enum Rotate
    {
        NoChange,
        Set,
        RandomOneValue,
        RandomTwovalues,
        Progress
    }

    [Header("Rotation Mode")]
    public Rotate rotating = Rotate.NoChange;

    [Tooltip("Use Normal as base rotation")]
    public bool addNormal;

    protected bool ShowOneValue() { return rotating == Rotate.RandomOneValue || rotating == Rotate.Set; }
    [Visible("ShowOneValue()")]
    public Vector3 Rotation = new Vector3(180, 0, 0);

    protected bool ShowTwovalues() { return rotating == Rotate.RandomTwovalues || rotating == Rotate.Progress; }
    [Visible("ShowTwovalues()")]
    public Vector3 Rotation_Min = new Vector3(-180, 0, 0);
    [Visible("ShowTwovalues()")]
    public Vector3 Rotation_Max = new Vector3(-180, 0, 0);
    void combineScale(int index, Vector3 scale)
    {
        Vector3 a = scales[index];

        scales[index] = new Vector3(a.x * scale.x, a.y * scale.y, a.z * scale.z);
    }

    void combineRotations(int index, Quaternion rot)
    {
        Quaternion a = rotations[index];
        rotations[index] = a * rot;
    }


    protected override void Compute()
    {
        base.Compute();

        //     Debug.Log("Positions " + positions.Count);

        switch (scaling)
        {
            default:
            case Scale.NoChange:
                break;
            case Scale.Uniform:
                for (int i = 0; i < positions.Count; i++)
                    combineScale(i, Vector3.one * size);
                break;
            case Scale.Rescale:
                for (int i = 0; i < positions.Count; i++)
                    combineScale(i, scale);
                break;
            case Scale.RandomUniform:
                for (int i = 0; i < positions.Count; i++)
                    combineScale(i, Random.Range(rangeScaleUniform.x, rangeScaleUniform.y) * Vector3.one);
                break;
            case Scale.FullRandom:
                for (int i = 0; i < positions.Count; i++)
                    combineScale(i, new Vector3(
                       Random.Range(rangeScale_Min.x, rangeScale_Max.x),
                       Random.Range(rangeScale_Min.y, rangeScale_Max.y),
                       Random.Range(rangeScale_Min.z, rangeScale_Max.z)));
                break;
            case Scale.Progress:
                for (int i = 0; i < positions.Count; i++)
                {
                    float progress = ((float)i) / Nb;
                    progress = scaleCurve.Evaluate(progress);
                    combineScale(i, new Vector3(
                       Mathf.Lerp(rangeScale_Min.x, rangeScale_Max.x, progress),
                       Mathf.Lerp(rangeScale_Min.y, rangeScale_Max.y, progress),
                       Mathf.Lerp(rangeScale_Min.z, rangeScale_Max.z, progress)));
                }
                break;  
        }

        Random.InitState(RandomSeed + 500);

        if (addNormal )
        { 
            for (int i = 0; i < positions.Count; i++)
                combineRotations(i,Quaternion.LookRotation(normals[i]));
        }


        switch (rotating)
        {
            case Rotate.NoChange:
                break;
            case Rotate.Set:
                for (int i = 0; i < positions.Count; i++)
                    combineRotations(i, Quaternion.Euler(Rotation));
                break;
            case Rotate.RandomOneValue:
                for (int i = 0; i < positions.Count; i++)
                    combineRotations(i, Quaternion.Euler(
                        Random.Range(-Rotation.x, Rotation.x),
                        Random.Range(-Rotation.y, Rotation.y),
                        Random.Range(-Rotation.z, Rotation.z)));
                break;
            case Rotate.RandomTwovalues:
                for (int i = 0; i < positions.Count; i++)
                    combineRotations(i, Quaternion.Euler(
                        Random.Range(Rotation_Min.x, Rotation_Max.x),
                        Random.Range(Rotation_Min.y, Rotation_Max.y),
                        Random.Range(Rotation_Min.z, Rotation_Max.z)));
                break;
            case Rotate.Progress:
                for (int i = 0; i < positions.Count; i++)
                {
                    float progress = ((float)i) / (Nb - 1);
                    combineRotations(i, Quaternion.Euler(
                       Mathf.Lerp(Rotation_Min.x, Rotation_Max.x, progress),
                       Mathf.Lerp(Rotation_Min.y, Rotation_Max.y, progress),
                       Mathf.Lerp(Rotation_Min.z, Rotation_Max.z, progress)));
                }
                break;
        }
    }

}
