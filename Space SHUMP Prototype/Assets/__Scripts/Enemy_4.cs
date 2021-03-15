using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Part
{
    public string name;
    public float health;
    public string[] protecredBy;

    // Two fields below initialize automatically in Start method;
    [HideInInspector]
    public GameObject go;

    [HideInInspector]
    public Material mat;
}

/// <summary>
/// Boss-spaceship with heavy armor and 4 parts.
/// It spawns out of the screen, chooses a random point on the screen, and moves to it.
/// Then he chooses a new point and moves towards it. 
/// All this movement continues until the player destroys him.
/// </summary>
public class Enemy_4 : Enemy
{

    [Header("Set in Inspector: Enemy_4")]
    public Part[] parts; // The array with parts of a spaceship.

    private Vector3 p0, p1;
    private float timeStart;
    private float duration = 4; // Movemant duration.

    private void Start()
    {
        p0 = p1 = pos; // Get spawn point from Main.Enemy.
        InitMovement();

        // Add each part of the spaceship game object and it's material.
        Transform t;
        foreach(Part prt in parts)
        {
            t = transform.Find(prt.name);
            if(t != null)
            {
                prt.go = t.gameObject;
                prt.mat = prt.go.GetComponent<Renderer>().material;
            }
        }
    }

    // Finds new points for movement and notes the birth time.
    void InitMovement()
    {
        p0 = p1;

        // Find a new point on screen.
        float widMinRad = bndCheck.camHeight - bndCheck.radius;
        float hgtMinRad = bndCheck.camHeight - bndCheck.radius;
        p1.x = Random.Range(-widMinRad, widMinRad);
        p1.y = Random.Range(-hgtMinRad, hgtMinRad);

        timeStart = Time.time;
    }

    public override void Move()
    {
        float u = (Time.time - timeStart) / duration;

        if (u >= 1)
        {
            InitMovement();
            u = 0;
        }

        u = 1 - Mathf.Pow(1 - u, 2); // Make movement smooth.
        pos = (1 - u) * p0 + u * p1; 
    }

    // Search function for a part of a spaceship by its name.
    Part FindPart(string n)
    {
        foreach(Part prt in parts)
        {
            if(prt.name == n)
            {
                return(prt);
            }
        }

        return (null);
    }

    // Search function for a part of a spaceship by a link for it`s GO.
    Part FindPart(GameObject go)
    {
        foreach(Part prt in parts)
        {
            if(prt.go == go)
            {
                return (prt);
            }
        }

        return (null);
    }

    // Three functions to check and inform were the part destroyed or it has some health.
    bool Destroyed(GameObject go)
    {
        return (Destroyed(FindPart(go)));
    }
    bool Destroyed(string n)
    {
        return (Destroyed(FindPart(n)));
    }
    bool Destroyed(Part prt)
    {
        if(prt == null)
        {
            return (true);
        }

        return (prt.health <= 0);
    }

    // Color only one part of the spaceship.
    void ShowLocalizedDamage(Material m)
    {
        m.color = Color.red;
        damageDoneTime = Time.time + showDamageDuration;
        showingDamage = true;
    }

    void OnCollisionEnter(Collision coll)
    {
        GameObject other = coll.gameObject;
        switch (other.tag)
        {
            case "ProjectileHero":

                // Don't damage the spaceship if it is out of screen.
                Projectile p = other.GetComponent<Projectile>();
                if (!bndCheck.isOnscreen)
                {
                    Destroy(other);
                    break;
                }

                // Find the part of a spaceship that was hit if it is available.
                GameObject goHit = coll.contacts[0].thisCollider.gameObject;
                Part prtHit = FindPart(goHit);
                if(prtHit == null)
                {
                    goHit = coll.contacts[0].otherCollider.gameObject;
                    prtHit = FindPart(goHit);
                }

                // Check if this part of the spaceship is still protected 
                // and damage the part that protects it.
                if (prtHit.protecredBy != null)
                {
                    foreach(string s in prtHit.protecredBy)
                    {
                        if (!Destroyed(s))
                        {
                            Destroy(other);
                            return;
                        }
                    }
                }

                // Damage the part that has no protection and show getting damage effect.
                prtHit.health -= Main.GetWeaponDefinition(p.type).damageOnHit;
                ShowLocalizedDamage(prtHit.mat);

                // Stop displaying part which has no health.
                if (prtHit.health <= 0)
                {
                    prtHit.go.SetActive(false);
                }

                // Check is there any parts of the spaceship or it is destroyed.
                bool allDestroyed = true;
                foreach(Part prt in parts)
                {
                    if (!Destroyed(prt))
                    {
                        allDestroyed = false;
                        break;
                    }
                }

                // Destroy enemy spaceship if there is no any parts of it.
                if (allDestroyed)
                {
                    Main.S.ShipDestroyed(this);
                    Destroy(this.gameObject);
                }

                // Destroy projectile.
                Destroy(other);
                break;
        }
    }
}
