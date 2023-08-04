using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animation : MonoBehaviour
{

    [Header("Components")]
    public Transform model;

    [Header("Method")]
    public Method method;
    public List<Axis> axis;
    public float multiplier = 1F;

    private Rigidbody rb;
    private float rotation;

    void Start() {
        this.rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate() {
        if (method == Method.ROTATE_WITH_SPEED) {
            rotation += rb.velocity.magnitude * multiplier;
        } else if (method == Method.CONSTANT) {
            rotation += multiplier;
        }
        model.rotation = Quaternion.Euler(axis.Contains(Axis.X) ? rotation : 0, axis.Contains(Axis.Y) ? rotation : 0, axis.Contains(Axis.Z) ? rotation : 0);
    }

    public enum Method {
        ROTATE_WITH_SPEED, CONSTANT
    }

    public enum Axis {
        X,Y,Z
    }
}
