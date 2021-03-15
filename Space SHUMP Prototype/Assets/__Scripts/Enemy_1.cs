using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_1 : Enemy
{
    [Header("Set in Inspector: Enemy_1")]
    float waveFrequency = 2;//seconds to do a full sinusoid
    float waveWidth = 4;
    float waveRotY = 45;

    private float x0;//the start point of the X coordinate
    private float birthTime;

    private void Start()
    {
        x0 = pos.x;

        //set birth time as a current time
        birthTime = Time.time;
    }

    public override void Move()
    {
        Vector3 temPos = pos;

        //thera will change with time so our sin will change too
        float age = Time.time - birthTime;
        float theta = Mathf.PI * 2 * age / waveFrequency;
        float sin = Mathf.Sin(theta);
        temPos.x = x0 + waveWidth * sin;
        pos = temPos;

        //y-axis rotation
        Vector3 rot = new Vector3(0, sin * waveRotY, 0);
        this.transform.rotation = Quaternion.Euler(rot);

        //downward movement on the y-axis from parent class
        base.Move();

       //print(bndCheck.isOnscreen);
    }
}
