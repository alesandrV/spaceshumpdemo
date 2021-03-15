using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Weapon types enumeration.
/// Here is a shield type (for changing its level).
/// </summary>
public enum WeaponType
{
    none,
    blaster,
    spread,
    shield
}

[System.Serializable]
public class WeaponDefinition
{
    public WeaponType type = WeaponType.none;
    public string letter; // the letter on the bonus box
    public Color color = Color.white; // bonus box color & weapon color
    public GameObject projectilePrefab;
    public Color projectileColor = Color.white;
    public float damageOnHit = 0;
    public float delayBetweenShots = 0;
    public float velocity = 20; // projectile speed
}

public class Weapon : MonoBehaviour
{
    static public Transform PROJECTILE_ANCHOR;

    [Header("Set Dynamically")]
    [SerializeField]
    private WeaponType _type = WeaponType.none;
    public WeaponDefinition def;
    public GameObject collar;
    public float lastShotTime;
    private Renderer collarRend;

    private void Start()
    {
        collar = transform.Find("Collar").gameObject;
        collarRend = collar.GetComponent<Renderer>();

        // Change weapon type to default - WeaponType.none.
        SetType(_type);

        // Сreate an anchor point dynamically.
        if (PROJECTILE_ANCHOR == null)
        {
            GameObject go = new GameObject("_ProjectileAnchor");
            PROJECTILE_ANCHOR = go.transform;
        }

        // Find a root game object - Hero.
        // Check availability Hero script and add fireDelegate to a game object. 
        GameObject rootGO = transform.root.gameObject;
        if (rootGO.GetComponent<Hero>() != null)
        {
            rootGO.GetComponent<Hero>().fireDelegate += Fire;
        }
    }

    public WeaponType type
    {
        get { return (_type); }
        set { SetType(value);  }
    }

    public void SetType(WeaponType wt)
    {
        // Deactivate game object if WeaponType.none.
        _type = wt;
        if (type == WeaponType.none)
        {
            this.gameObject.SetActive(false);
            return;
        }
        else
        {
            this.gameObject.SetActive(true);
        }

        // Define the weapon type, set color and set last shot field as 0.
        def = Main.GetWeaponDefinition(_type);
        collarRend.material.color = def.color;
        lastShotTime = 0;
    }

    public void Fire()
    {
        // Check if game object active.
        if (!gameObject.activeInHierarchy)
            return;

        // Check is time after shots have passed enough .
        if (Time.time - lastShotTime < def.delayBetweenShots)
            return;

        // Set the initial projectile speed for the hero or enemies.
        Projectile p;
        Vector3 vel = Vector3.up * def.velocity;
        if (transform.up.y < 0)
            vel.y = -vel.y;

        switch (type)
        {
            // One projectile that moves upward.Returns a reference to the projectile instance.
            case WeaponType.blaster:
                p = MakeProjectile();
                p.rigid.velocity = vel;
                break;
            // Create three projectiles and rotate right and left 10 degrees to the right / left.
            case WeaponType.spread:
                p = MakeProjectile();
                p.rigid.velocity = vel;
                p = MakeProjectile();
                p.transform.rotation = Quaternion.AngleAxis(10, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;
                p = MakeProjectile();
                p.transform.rotation = Quaternion.AngleAxis(-10, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;
                break;

        }
    }

    // Creates instances of the WeaponDefinition template and returns a link to an instance of the Projectile class.
    public Projectile MakeProjectile()
    {
        GameObject go = Instantiate<GameObject>(def.projectilePrefab);

        // Сheck who fired the projectile.
        if (transform.parent.gameObject.tag == "Hero")
        {
            go.tag = "ProjectileHero";
            go.layer = LayerMask.NameToLayer("ProjectileHero");
        }
        else
        {
            go.tag = "ProjectileEnemy";
            go.layer = LayerMask.NameToLayer("ProjectileEnemy");
        }

        go.transform.position = collar.transform.position;

        // We make a PROJECTILE_ANCHOR as parent to Projectile to keep our Hiteracy clean.
        go.transform.SetParent(PROJECTILE_ANCHOR, true);
        Projectile p = go.GetComponent<Projectile>();
        p.type = type;

        lastShotTime = Time.time;
        return (p);
    }
}
