using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpPosition : MonoBehaviour
{
    [Header("Values")]
    public Vector2 start;
    public Vector2 end;
    public float time;

    private float t;

    void Start()
    {
        time *= 50;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        t++;
        float r = t / time;
        float p = r - Mathf.FloorToInt(r);
        
        gameObject.transform.position = new Vector3(Mathf.Lerp(start.x, end.x, p), Mathf.Lerp(start.y, end.y, p), gameObject.transform.position.z);
    }
}
