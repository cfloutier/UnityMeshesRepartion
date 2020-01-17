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
                pTr.localScale.y * scales[i].y,
                pTr.localScale.z * scales[i].z);

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

      /*  Debug.Log("positions " + positions.Count);
        Debug.Log("normals " + normals.Count);
        Debug.Log("scales " + scales.Count);
        Debug.Log("rotations " + rotations.Count);*/

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
