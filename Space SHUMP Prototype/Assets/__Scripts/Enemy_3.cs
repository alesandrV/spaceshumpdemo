using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//determine movement using linear interpolation of more than 2 points via Bezier curve
public class Enemy_3 : Enemy
{
    [Header("Set in  Inspector: Enemy_3")]
    public float lifeTime = 5;

    [Header("Set Dynamically: Enemy_3")]
    public Vector3[] points;//array for all our points
    public float birthTime;

    private void Start()
    {
        points = new Vector3[3];

        points[0] = pos;

        //set random x coordinates 
        float xMin = -bndCheck.camWidth + bndCheck.radius;
        float xMax = bndCheck.camWidth - bndCheck.radius;

        //choose a random point below the middle of the screen
        Vector3 v;
        v = Vector3.zero;
        v.x = Random.Range(xMin, xMax);
        v.y = -bndCheck.camHeight * Random.Range(2.75f, 2);
        points[1] = v;

        //choose a random point above the middle of the screen
        v = Vector3.zero;
        v.y = pos.y;
        v.x = Random.Range(xMin, xMax);
        points[2] = v;

        //set birth time as a current time
        birthTime = Time.time;
    }

    public override void Move()
    {
        float u = (Time.time - birthTime) / lifeTime;

        //destroy "too old" spaceships
        if (u > 1)
        {
            Destroy(this.gameObject);
            return;
        }

        //interpolate Bezier curve by three points
        Vector3 p01, p12;
        u = u - 0.2f * Mathf.Sin(u * Mathf.PI * 2);//easing function to move evenly 
        p01 = (1 - u) * points[0] + u * points[1];
        p12 = (1 - u) * points[1] + u * points[2];
        pos = (1 - u) * p01 + u * p12;
    }
}
