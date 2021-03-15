using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    // using a "Singleton" pattern cuz we can have only one spaceship
    static public Hero S;

    //fields for our spaceship movement
    [Header("Set in Inspector")]
    public float speed = 30;
    public float rollMult = -45;
    public float pitchMult = 30;
    public float gameRestartDelay = 2f;
    public GameObject projectilePrefab;
    public float projectileSpeed = 40;
    public Weapon[] weapons;

    [Header("Set Dynamically")]
    [SerializeField]
    private float _shieldLevel = 1;

    //link to last object trigger collided 
    private GameObject lastTriggerGo = null;

    // This delegate will help us to do shots from each weapon.
    public delegate void WeaponFireDelegate();
    public WeaponFireDelegate fireDelegate;

    private void Start()
    {
        if (S == null)
        {
            S = this; // creation link to singleton object
        }
        else
        {
            Debug.LogError("Hero.Awake() - Attempted to assign second Hero.S");
        }

        // Clean weapons array and add one blaster.
        ClearWeapons();
        weapons[0].SetType(WeaponType.blaster);

        // fireDelegate += TempFire;
    }

    void Update()
    {
        // take info from Input class
        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");

        // change transform.position using info from Input 
        Vector3 pos = transform.position;
        pos.x += xAxis * speed * Time.deltaTime;
        pos.y += yAxis * speed * Time.deltaTime;
        transform.position = pos;

        // rotate the spaceship to make it looks more dynamic
        transform.rotation = Quaternion.Euler(yAxis * pitchMult, xAxis * rollMult, 0);

        // Open fire with all guns. 
        if (Input.GetAxis("Jump") == 1 && fireDelegate != null)
        {
            fireDelegate();
        }
    }

    // (!!!)
    void TempFire()
    {
        GameObject projGO = Instantiate<GameObject>(projectilePrefab);
        projGO.transform.position = transform.position;
        Rigidbody rigidB = projGO.GetComponent<Rigidbody>();

        // Extract information from WeaponType and use it to determine projGO speed.
        Projectile proj = projGO.GetComponent<Projectile>();
        proj.type = WeaponType.blaster;
        float tSpeed = Main.GetWeaponDefinition(proj.type).velocity;
        rigidB.velocity = Vector3.up * tSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;

        // impossibility of collision with the same object twice
        if (go == lastTriggerGo)
        {
            return;
        }

        lastTriggerGo = go;

        // Reduce the shield level and destroy the enemy or use PowerUp or do nothing.
        if (go.tag == "Enemy")
        {
            shieldLevel--;
            Destroy(go);
        }
        else if (go.tag == "PowerUp")
        {
            AbsorbPowerUp(go);
        }
        else
        {
            print("Triggered not by enemy: " + go.name);
        }
    }

    // Increase the shield level using power-up or upgrade/change weapon.
    public void AbsorbPowerUp(GameObject go)
    {
        PowerUp pu = go.GetComponent<PowerUp>();
        switch (pu.type)
        {
            case WeaponType.shield:
                shieldLevel++;
                break;

            default:
                if (pu.type == weapons[0].type)
                {
                    Weapon w = GetEmptyWeaponSlot(); // Find slot for a new gun.
                    if (w != null)
                    {
                        w.SetType(pu.type);
                    }
                    else
                    {
                        ClearWeapons();
                        weapons[0].SetType(pu.type); // Change weapon.
                    }
                }
                break;
        }
        pu.AbsorbedBy(this.gameObject);
    }

    // Property to control shield lev'el and restart scene.
    public float shieldLevel
    {
        get
        {
            return _shieldLevel;
        }
        set
        {
            _shieldLevel = Mathf.Min(value, 4);

            if (value < 0)
            {
                Destroy(this.gameObject);

                Main.S.DelayedRestart(gameRestartDelay);
            }
        }
    }
    
    // Find empty slot for weapon.
    Weapon GetEmptyWeaponSlot()
    {
        for(int i = 0; i<weapons.Length; i++)
        {
            if (weapons[i].type == WeaponType.none)
            {
                return (weapons[i]);
            }
        }
        return (null);
    }

    // Clean weapon slots.
    void ClearWeapons()
    {
        foreach(Weapon w in weapons)
        {
            w.SetType(WeaponType.none);
        }
    }
}
