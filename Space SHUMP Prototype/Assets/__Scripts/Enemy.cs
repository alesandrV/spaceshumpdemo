using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// parent class fo all enemies
public class Enemy : MonoBehaviour
{
    [Header("Set in Inspecror: Enemy")]
    public float speed = 10f;
    public float fireRate = 0.3f;
    public float health = 10;
    public float score = 100; // points for destroying the spaceship
    public float showDamageDuration = 0.1f;
    public float powerUpDropChance = 1f; // Chance to leave a PowerUp after been destroyed.

    [Header("Set Dynamically: Enemy")]
    public Color[] originalColors;
    public Material[] materials; // all game object`s materials (and its children's objects)
    public bool showingDamage = false;
    public float damageDoneTime; // time to end the damage effect
    public bool notifiedOfDestruction = false;

    protected BoundsCheck bndCheck;// link to another script in this game object

    private void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();

        // Fill the array with materials
        materials = Ultis.GetAllMaterials(gameObject);
        originalColors = new Color[materials.Length];
        //check and save all materials with their default colors
        for (int i=0; i<materials.Length; i++)
        {
            originalColors[i] = materials[i].color;
        }
    }

    public Vector3 pos
    {
        get { return (this.transform.position); }
        set { this.transform.position = value; }
    }

    private void Update()
    {
        Move();

        // Check how long the enemy is red
        if (showingDamage && Time.time > damageDoneTime)
        {
            UnShowDamage();
        }

        // Check if the spaceship out of bottom border of the screen
        if (bndCheck != null && bndCheck.offDown)
        {
            Destroy(gameObject);
        }
    }

    // virtual cuz we will override it 
    public virtual void Move()
    {
        Vector3 temPos = pos;
        temPos.y -= speed * Time.deltaTime;
        pos = temPos;
    }

    // Check the collision object tag, add damage or delete both objects
    private void OnCollisionEnter(Collision coll)
    {
        GameObject otherGO = coll.gameObject;
        switch (otherGO.tag)
        {
            case "ProjectileHero":
                Projectile p = otherGO.GetComponent<Projectile>();

                if (!bndCheck.isOnscreen)
                {
                    Destroy(otherGO);
                    break;
                }

                ShowDamage();

                health -= Main.GetWeaponDefinition(p.type).damageOnHit;
                if (health <= 0)
                {
                    // Notify Main object before the enemy destroyed.
                    if (!notifiedOfDestruction)
                    {
                        Main.S.ShipDestroyed(this);
                    }

                    notifiedOfDestruction = true;

                    Destroy(this.gameObject);
                }

                Destroy(otherGO);
                break;

            default:
                print("Enemy hit by non-ProjectileHero: " + otherGO.name);
                break;
        }
    }

    // Colors all materials in red
    void ShowDamage()
    {
        foreach (Material m in materials)
        {
            m.color = Color.red;
        }

        showingDamage = true;
        damageDoneTime = Time.time + showDamageDuration;
    }

    // Color all material in an array with their default colors
    void UnShowDamage()
    {
        for (int i = 0; i<materials.Length; i++)
        {
            materials[i].color = originalColors[i];
        }

        showingDamage = false;
    }
}
