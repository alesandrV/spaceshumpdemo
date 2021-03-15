using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//determine movement using linear interpolation with easing function
public class Enemy_2 : Enemy
{
    [Header("Srt in Inspector: Enemy_2")]
    public float sinEccentricity = 0.6f;
    public float lifeTime = 10;

    [Header("Set Dynemically: Enemy_2")]
    public Vector3 p0;
    public Vector3 p1;
    public float birthTime;

    private void Start()
    {
        //Choose the random point on the left side of the screen
        p0 = Vector3.zero;
        p0.x = -bndCheck.camWidth - bndCheck.radius;
        p0.y = Random.Range(-bndCheck.camHeight, bndCheck.camHeight);

        //Choose the random point on the rigth side of the screen
        p1 = Vector3.zero;
        p1.x = bndCheck.camWidth + bndCheck.camWidth;
        p1.y = Random.Range(-bndCheck.camHeight, bndCheck.camHeight);

        //Swap points randomly
        if (Random.value > 0.5f)
        {
            p0.x *= -1;
            p1.x *= -1;
        }

        //set birth time as a current time
        birthTime = Time.time;
    }

    public override void Move()
    {
        float u = (Time.time - birthTime) / lifeTime;

        //if u > 1, meant that our spaceship lives more than lifeTime
        if (u > 1)
        {
            Destroy(this.gameObject);
            return;
        }

        //Fix the trajectory of our spaceship to make  move via sinusoid
        u = u + sinEccentricity * (Mathf.Sin(u * Mathf.PI * 2));

        //Use linear interpolation to make our spaceship move 
        pos = (1 - u) * p0 + u * p1;
    }
}
