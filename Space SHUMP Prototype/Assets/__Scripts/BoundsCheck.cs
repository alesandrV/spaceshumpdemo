using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundsCheck : MonoBehaviour
{
    /// <summary>
    /// Prevents the game object from going off-screen.
    /// Works only with orthographic camera at [0, 0, 0].
    /// </summary>

    [Header("Set in Inspector")]
    public float radius = 1f;
    public bool keepOnScreen = true;//helps us to know is it allowed to go out of screen

    [Header("Set Dynamically")]
    public bool isOnscreen = true;//false when game object is out of the screen
    public float camWidth;
    public float camHeight;

    [HideInInspector]
    public bool offRight, offLeft, offUp, offDown; //it will help us to check which screen side was crossed

    private void Awake()
    {
        //find the distance from the center to the top/bottom camera borders
        camHeight = Camera.main.orthographicSize;
        //find the distance from the center to the left/right camera borders
        camWidth = camHeight * Camera.main.aspect;
    }

    private void LateUpdate() //LateUpdate method called after method Update in each frame
    {
        Vector3 pos = transform.position;
        isOnscreen = true;
        offRight = offLeft = offDown = offUp = false;

        if(pos.x > camWidth - radius)
        {
            pos.x = camWidth - radius;
            offRight = true;
        }

        if (pos.x < -camWidth + radius)
        {
            pos.x = -camWidth + radius;
            offLeft = true;
        }

        if (pos.y > camWidth - radius)
        {
            pos.y = camWidth - radius;
            offUp = true;
        }

        if (pos.y < -camWidth + radius)
        {
            pos.y = -camWidth + radius;
            offDown = true;
        }

        isOnscreen = !(offRight || offLeft || offUp || offDown);
        //our game object is out of the screen we have to get it back
        if (keepOnScreen && !isOnscreen)
        {
            transform.position = pos;
            isOnscreen = true;
            offRight = offLeft = offUp = offDown = false;
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Vector3 boundSize = new Vector3(camWidth * 2, camHeight * 2, 0.1f);
        Gizmos.DrawWireCube(Vector3.zero, boundSize);
    }
}
