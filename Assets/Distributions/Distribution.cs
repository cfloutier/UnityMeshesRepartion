using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MeshFusion;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class Distribution : MonoBehaviour
{
    [Header("Main Options")]
    [Tooltip("Shown positions before Build")]
    public bool drawGizmos = true;
    [Tooltip("Build For each Parameter change.")]
    public bool BuildOnEditorChange = false;

    [Tooltip("Build On start of the play mode")]
    public bool BuildOnStart = false;

    public enum MeshMode
    {
        DifferentObjects,
        ApplyTransform,
        Merge
    }

    [Tooltip("Merge result into one or several meshes")]
    public MeshMode MergeMode = MeshMode.DifferentObjects;

    [Header("Distribution")]
    [Space(20)]

    public int Nb = 10;

    public GameObject[] prefabs;
    [Range(0, 4096)]
    public int RandomSeed;

    bool needRebuild = false;

#if UNITY_EDITOR
    [ButtonsBar("Dice", "DiceAndBuild")]
    public bool btRandom;

    protected void Dice()
    {
        RandomSeed = (RandomSeed + 1) % 4096;
        Compute();
        EditorUtility.SetDirty(gameObject);
    }

    protected void DiceAndBuild()
    {
        RandomSeed = (RandomSeed + 1) % 4096;
        Build();
    }
#endif

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

    // the result Positions
    protected List<Vector3> positions = new List<Vector3>();
    protected List<Vector3> normals = new List<Vector3>();
    protected List<Vector3> scales = new List<Vector3>();
    protected List<Quaternion> rotations = new List<Quaternion>();

    [ButtonsBar("Build", "Clean")]
    public bool bt;

    private void Start()
    {
        if (BuildOnStart && Application.isPlaying)
            Build();
    }

    public void Clean()
    {
        transform.CleanChildren();
        gameObject.RemoveComponent<MeshFilter>();
        gameObject.RemoveComponent<Renderer>();
    }

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

    protected void InitLists()
    {
        positions.Clear();
        normals.Clear();
        scales.Clear();
        rotations.Clear();
    }

    protected virtual void Compute()
    {
        Random.InitState(RandomSeed);
        if (Nb < 1)
            Nb = 1;

        if (normals.Count != positions.Count)
        {
            normals = new List<Vector3>();
            for (int i = 0; i < positions.Count; i++)
                normals.Add(Vector3.up);
        }
        if (scales.Count != positions.Count)
        {
            scales = new List<Vector3>();
            for (int i = 0; i < positions.Count; i++)
                scales.Add(Vector3.one);
        }

        if (rotations.Count != positions.Count)
        {
            rotations = new List<Quaternion>();
            for (int i = 0; i < positions.Count; i++)
                rotations.Add(Quaternion.identity);
        }

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

    public void Build()
    {
        if (prefabs.Length == 0)
            return;

        Clean();
        Compute();

        /*
         Debug.Log("positions " + positions.Count);
         Debug.Log("scales " + scales.Count);

          */

        for (int i = 0; i < positions.Count; i++)
        {
            int indexPrefab = Random.Range(0, prefabs.Length);
            GameObject prefab = prefabs[indexPrefab];
            GameObject go = Instantiate(prefab);
            Transform tr = go.transform;
            Transform pTr = prefab.transform;
            tr.parent = transform;
            tr.localPosition = positions[i];

            tr.localScale = new Vector3(
                pTr.localScale.x * scales[i].x,
                pTr.localScale.z * scales[i].y,
                pTr.localScale.y * scales[i].z);

            tr.localRotation = pTr.localRotation * rotations[i];

        }

        switch (MergeMode)
        {
            default:
            case MeshMode.DifferentObjects:
                break;
            case MeshMode.ApplyTransform:
                {
                    GameObject newRoot = new GameObject();
                    newRoot.transform.parent = transform;
                    newRoot.transform.SetIdentity();
                    newRoot.transform.parent = transform.parent;


                    while (transform.childCount > 0)
                    {
                        Transform tr = transform.GetChild(0);
                        GameObject newGo = new GameObject(tr.name);
                        newGo.transform.parent = newRoot.transform;
                        newGo.transform.position = tr.position;
                        newGo.transform.localRotation = Quaternion.identity;
                        newGo.transform.localScale = Vector3.one;
                        newGo.name = tr.name;

                        tr.transform.parent = newGo.transform;

                        MeshMerge.MergeAllChilds(newGo.transform);
                    }

                    while (newRoot.transform.childCount > 0)
                        newRoot.transform.GetChild(0).parent = transform;

                    newRoot.SafeDestroy();


                }
                break;
            case MeshMode.Merge:
                MeshMerge.MergeAllChilds(transform);
                break;
        }

    }

    private void Update()
    {
        if (needRebuild)
            Build();

        needRebuild = false;
    }

#if UNITY_EDITOR

    public static float GetGizmoSize(Vector3 position)
    {
        Camera current = Camera.current;
        position = Gizmos.matrix.MultiplyPoint(position);

        if (current)
        {
            Transform transform = current.transform;
            Vector3 position2 = transform.position;
            float z = Vector3.Dot(position - position2, transform.TransformDirection(new Vector3(0f, 0f, 1f)));
            Vector3 a = current.WorldToScreenPoint(position2 + transform.TransformDirection(new Vector3(0f, 0f, z)));
            Vector3 b = current.WorldToScreenPoint(position2 + transform.TransformDirection(new Vector3(1f, 0f, z)));
            float magnitude = (a - b).magnitude;
            return 10f / Mathf.Max(magnitude, 0.0001f);
        }

        return 20f;
    }

    private void OnValidate()
    {
        if (BuildOnEditorChange)
            needRebuild = true;

        Compute();
    }

    protected virtual void OnDrawGizmosSelected()
    {
        if (!drawGizmos) return;

        Debug.Log("positions " + positions.Count);
        Debug.Log("normals " + normals.Count);
        Debug.Log("scales " + scales.Count);
        Debug.Log("rotations " + rotations.Count);

        Color c = Color.cyan;
        c.a = 0.5f;
        Gizmos.color = c;
        Matrix4x4 tr = transform.localToWorldMatrix;

        Quaternion rot = Quaternion.identity;
        Vector3 scale = Vector3.one;
        Vector3 pos = Vector3.zero;
        for (int i = 0; i < positions.Count; i++)
        {
            pos = positions[i];
            rot = rotations[i];
            scale = scales[i];

            Gizmos.matrix = tr * Matrix4x4.TRS(pos, rot, scale);

            Gizmos.DrawCube(Vector3.zero, Vector3.one);
        }
    }
#endif

}
