using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // for loading/reloading screens

public class Main : MonoBehaviour
{
    static public Main S;
    static Dictionary<WeaponType, WeaponDefinition> WEAP_DICT;

    [Header("Set in Inspector")]
    public GameObject[] prefabEnemies;
    public float enemySpawnPerSecond = 0.5f;
    public float enemyDefaultPadding = 1.5f;

    public WeaponDefinition[] weaponDefinitions;
    public GameObject prefabPowerUp; // Template for each bonus.
    public WeaponType[] powerUpFrequency = new WeaponType[]{ // Array for the frequency of PowerUps. 
                        WeaponType.blaster, WeaponType.blaster, //Blasters will spawn twice as often.
                        WeaponType.spread, WeaponType.shield };

    private BoundsCheck bndCheck;

    // Enemy ship instance will call this method to leave a power-up sometimes after been destroyed.
    public void ShipDestroyed(Enemy e)
    {
        // Generate random chance to drop power-up.
        if (Random.value <= e.powerUpDropChance)
        {
            // Choose random lines from "powerUpFrequency" array.
            int ndx = Random.Range(0, powerUpFrequency.Length);
            WeaponType puType = powerUpFrequency[ndx];

            // Create instance PowerUp and set chosen type.
            GameObject go = Instantiate(prefabPowerUp) as GameObject;
            PowerUp pu = go.GetComponent<PowerUp>();
            pu.SetType(puType);

            // Place bonus at the position where was enemy ship destroyed.
            pu.transform.position = e.transform.position;
        }
    }

    private void Awake()
    {
        S = this;

        bndCheck = GetComponent<BoundsCheck>();
        Invoke("SpawnEnemy", 1f / enemySpawnPerSecond); // spawn enemy each 2 sec in default settings

        // create and fill a new dictionary with weapons
        WEAP_DICT = new Dictionary<WeaponType, WeaponDefinition>();
        foreach(WeaponDefinition def in weaponDefinitions)
        {
            WEAP_DICT[def.type] = def;
        }
    }

    public void SpawnEnemy()
    {
        // choosing a random number and take appropriate prefab from array
        int ndx = Random.Range(0, prefabEnemies.Length);
        GameObject go = Instantiate<GameObject>(prefabEnemies[ndx]);

        // set the default radius for indent from the top of the screen
        float enemyPadding = enemyDefaultPadding;
        if (go.GetComponent<BoundsCheck>() != null)
        {
            enemyPadding = Mathf.Abs(go.GetComponent<BoundsCheck>().radius);
        }

        // get the position of a new spaceship and set random x coordinates and default y
        Vector3 pos = Vector3.zero;
        float xMin = -bndCheck.camWidth + enemyPadding;
        float xMax = bndCheck.camWidth - enemyPadding;
        pos.x = Random.Range(xMin, xMax);
        pos.y = bndCheck.camHeight + enemyPadding;
        go.transform.position = pos;

        Invoke("SpawnEnemy", 1f / enemySpawnPerSecond);
    }

    // restart scene calls in "hero" script
    public void DelayedRestart(float delay)
    {
        Invoke("Restart", delay);
    }

    public void Restart()
    {
        SceneManager.LoadScene("_Scene_0");
    }

    /// <summary>
    /// Static function, which gets WeaponDefinition from static WEAP_DICT, Main class.
    /// </summary>
    /// <param name="wt"> WeaponType which is needed to get WeaponDefinition. </param>
    /// <returns> Existed copy of WeaponDefinition or a new copy of Weapondefinition with definite type. </returns>

    static public WeaponDefinition GetWeaponDefinition (WeaponType wt)
    {
        if (WEAP_DICT.ContainsKey(wt))
        {
            return (WEAP_DICT[wt]);
        }

        return (new WeaponDefinition());
    }
}
