using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnable : MonoBehaviour
{
    [Header("Values")]
    public Vector2 scaleRange;
    public Vector2 speedRange;
    public Vector2 atpRange;

    private float scale;
    private float speed;
    private float atp;
    
    void Start()
    {
        this.scale = scaleRange.x;
        this.speed = speedRange.x;
        this.atp = atpRange.x;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float Speed() {
        return speed;
    }

    public float Scale() {
        return scale;
    }

    public float ATP() {
        return atp;
    }

    public void Instance(float scale, float speed, float atp) {
        this.scale = scale;
        this.speed = speed;
        this.atp = atp;
    }
}
