#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RemoveCollidersTool : MonoBehaviour
{
    //[MenuItem("Tools/Remove Colliders %#p")]
    [MenuItem("Tools/Remove Colliders")]
    static public void removeColliderAndApplyPrefabChanges()
    {
        string log = "";
        var obj = Selection.gameObjects;

        if (obj != null)
        {
            for (int i = 0; i < obj.Length; i++)
            {
                log += "<color=white>CHECKING PREFAB:</color> " + obj[i].name + "\n";

                // now check to see if has a collider
                Collider[] colliders = obj[i].GetComponentsInChildren<Collider>();
                foreach (Collider collider in colliders)
                {
                    log += "\t<color=red>REMOVING COLLIDER:</color> " + collider.name + "\n";
                    EditorUtility.SetDirty(collider.gameObject);
                    // remove the collider
                    DestroyImmediate(collider);
                }
            }
            Debug.Log(log);
        }
        else
        {
            Debug.Log("Nothing selected");
        }
    }

    //[MenuItem("Tools/Remove Colliders %#p")]
    //static public void removeColliderAndApplyPrefabChanges()
    //{
    //    string log = "";
    //    var obj = Selection.gameObjects;
    //    if (obj != null)
    //    {
    //        for (int i = 0; i < obj.Length; i++)
    //        {
    //            bool modified = false;
    //            // check to see if selected object is connected to a prefab
    //            var prefab_root = PrefabUtility.FindPrefabRoot(obj[i]);
    //            var prefab_src = PrefabUtility.GetPrefabParent(prefab_root);
    //            if (prefab_src != null)
    //            {
    //                log += "<color=white>CHECKING PREFAB:</color> " + obj[i].name + "\n";

    //                // now check to see if has a collider
    //                Collider[] colliders = obj[i].GetComponentsInChildren<Collider>();
    //                foreach (Collider collider in colliders)
    //                {
    //                    log += "\t<color=red>REMOVING COLLIDER:</color> " + collider.name + "\n";

    //                    // remove the collider
    //                    DestroyImmediate(collider);
    //                    modified = true;
    //                }

    //                if (modified)
    //                {
    //                    // apply updated
    //                    PrefabUtility.ReplacePrefab(prefab_root, prefab_src, ReplacePrefabOptions.ConnectToPrefab);
    //                    log += "\t<color=yellow>APPLYING PREFAB:</color> " + AssetDatabase.GetAssetPath(prefab_src) + "\n\n";
    //                }
    //            }
    //        }
    //        Debug.Log(log);
    //    }
    //    else
    //    {
    //        Debug.Log("Nothing selected");
    //    }
    //}
}
#endif