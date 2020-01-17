#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Curves.Tools
{
    using System;


    using UnityEditor;

    public class MyAssetModificationProcessor : AssetModificationProcessor
    {
        public static string[] OnWillSaveAssets(string[] paths)
        {
            var saves = GameObject.FindObjectsOfType<CleanupBeforeSave>();
            foreach (var save in saves)
                if (save.CleanUpOnSave)
                    save.CleanupNow();

            return paths;
        }
    }



    /// <summary>
    /// This class is used to cleanup the save before save 
    /// to avoid having huges meshes to save in the scene
    /// </summary>
    public class CleanupBeforeSave : MonoBehaviour
    {
        [Header("All curve renderers will be cleaned before saving")]
        public bool CleanUpOnSave = true;

        [Header("If set, all curve rendererer will be rebuild on start")]
        public bool RebuildOnAwake = true;

        [ButtonsBar("CleanupNow", "BuildNow")]
        public bool bt;

        public void CleanupNow()
        {
            var renderers = gameObject.GetComponentsInChildren<Distribution>();
            foreach (var r in renderers)
            {
                r.Clean();
            }

        }


        public void BuildNow()
        {
            var renderers = gameObject.GetComponentsInChildren<Distribution>();
            foreach (var r in renderers)
            {
                r.Build();
            }
        }

        public void RebuildNow()
        {
            Awake();
        }


        private void Awake()
        {
            if (RebuildOnAwake)
            {
                RebuildNow();
            }
        }
    }

}
#endif
