using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityDespawn : MonoBehaviour
{
    [Header("Values")]
    public float threshold;

    private Rigidbody rb;

    void Start()
    {
        this.rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (this.rb.velocity.magnitude > 0 && this.rb.velocity.magnitude <= threshold) {
            Destroy(this.gameObject);
        }
    }
}
