using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    [Header("Set in Inspector")]
    public float rotationsPerSecond = 0.1f;

    [Header("Set Dynamically")]
    public int levelShown = 0;

    Material mat;

    //set "mat" as a material of Renderer component (Shield game object)
    private void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    private void Update()
    {
        //check the current shield level
        int currLevel = Mathf.FloorToInt(Hero.S.shieldLevel);

        //shield level and current level have to be the same
        if (levelShown != currLevel)
        {
            levelShown = currLevel;

            //changing the different fields
            mat.mainTextureOffset = new Vector2(0.2f * levelShown, 0);
        }

        //make the chosen shield rotate each frame
        float rZ = -(rotationsPerSecond * Time.time * 360) % 360f;
        transform.rotation = Quaternion.Euler(0, 0, rZ);
    }
}
