using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpPosition : MonoBehaviour
{
    [Header("Values")]
    public AnimationCurve curve;
    public Vector3 start;
    public Vector3 end;
    public float time;

    private float t;

    void Start() {
        time *= 50;
    }

    void FixedUpdate() {
        t++;
        float r = t / time;
        float p = r - Mathf.FloorToInt(r);

        p = curve.Evaluate(p);
        
        gameObject.transform.position = new Vector3(Mathf.Lerp(start.x, end.x, p), Mathf.Lerp(start.y, end.y, p), Mathf.Lerp(start.z, end.z, p));
    }
}
