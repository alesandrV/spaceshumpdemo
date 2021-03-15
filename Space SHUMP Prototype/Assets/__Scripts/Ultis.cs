using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ultis : MonoBehaviour
{
    // Returns a list of all a game object`s materials (and its children objects`).

    static public Material[] GetAllMaterials(GameObject go)
    {
        // Check a game object with its children object and returns an array with <Renreder> components
        Renderer[] rends = go.GetComponentsInChildren<Renderer>();

        // Get all values from the list with renderer materials
        List<Material> mats = new List<Material>();
        foreach(Renderer rend in rends)
        {
            mats.Add(rend.material);
        }

        // Convert a list to an array and returns it
        return (mats.ToArray());
    }
}
