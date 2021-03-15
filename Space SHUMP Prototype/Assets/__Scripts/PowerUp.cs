using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [Header("Set in Inspector")]
    public Vector2 rotMinMax = new Vector2(15, 90);
    public Vector2 driftMinMax = new Vector2(.25f, 2);
    public float lifeTime = 6f;
    public float fadeTime = 4f; // Seconds it will then fade

    [Header("Set Dynamically")]
    public WeaponType type;
    public GameObject cube;
    public TextMesh letter;
    public Vector3 rotPerSecond;
    public float birthTime;

    private Rigidbody rigid;
    private BoundsCheck bndCheck;
    private Renderer cubeRend;

    private void Awake()
    {
        // Get a link to cube
        cube = transform.Find("Cube").gameObject;

        // Get links to all other necessary components
        letter = GetComponent<TextMesh>();
        rigid = GetComponent<Rigidbody>();
        bndCheck = GetComponent<BoundsCheck>();
        cubeRend = cube.GetComponent<Renderer>();

        // Choose the random velocity XYZ
        // Random.onUnitSphere - returns a random point on the surface of a sphere with radius 1
        Vector3 vel = Random.onUnitSphere;

        // Display constant Z on XY
        // Normalize our vector
        // Choose a random speed
        vel.z = 0;
        vel.Normalize();
        vel *= Random.Range(driftMinMax.x, driftMinMax.y);
        rigid.velocity = vel;

        // Making a rotation angle of equals 0.
        transform.rotation = Quaternion.identity;

        // Choose a random rotation speed for the nested cube.
        rotPerSecond = new Vector3(Random.Range(rotMinMax.x, rotMinMax.y),
            Random.Range(rotMinMax.x, rotMinMax.y),
            Random.Range(rotMinMax.x, rotMinMax.y));

        birthTime = Time.time;
    }

    private void Update()
    {
        // Rotate our cube slowly in each frame.
        cube.transform.rotation = Quaternion.Euler(rotPerSecond * Time.time);

        //  Cube has to disappear when the time comes but it must be slowly, in 4 seconds.
        float u = (Time.time - (birthTime + lifeTime) / fadeTime);

        if (u >= 1)
        {
            Destroy(this.gameObject);
            return;
        }

        if (u > 0)
        {
            // // Make our cube become darker by using u and alpha-value.
            Color c = cubeRend.material.color;
            c.a = 1f - u;
            cubeRend.material.color = c;

            // The letter will become darker slower than cube.
            c = letter.color;
            c.a = 1f - (u * 0.5f);
            letter.color = c;
        }

        // Destroy PowerUp if it's out of screen.
        if (!bndCheck.isOnscreen)
        {
            Destroy(gameObject);
        }
    }

    public void SetType(WeaponType wt)
    {
        // Get Weapontype from the "Main" script.
        WeaponDefinition def = Main.GetWeaponDefinition(wt);

        // Set color for child cube.
        cubeRend.material.color = def.color;

        // Set necessary letter.
        letter.text = def.letter;

        // Set the actual type.
        type = wt;
    }

    // This function is called with "Hero" class when the player picks up a bonus.
    public void AbsorbedBy(GameObject target)
    {
        Destroy(this.gameObject);
    }
}
