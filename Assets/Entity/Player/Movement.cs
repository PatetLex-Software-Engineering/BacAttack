using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Movement : MonoBehaviour
{

    [Header("Components")]
    public CinemachineVirtualCameraBase cinemachineCamera;

    private Rigidbody rb;

    private MovementController controller;

    private float zInput;
    private float xInput;
    private float yInput;

    void Start() {
        rb = GetComponent<Rigidbody>();
        controller = MovementController.CLASSIC;
    }

    void Update() {
        UpdateInput();
    }

    void FixedUpdate() {
        Move();
    }

    void UpdateInput() {
        xInput = Input.GetAxisRaw("Horizontal");
        zInput = Input.GetAxisRaw("Vertical");
        yInput = Input.GetKey(KeyCode.Space) ? 1F : (Input.GetKey(KeyCode.LeftShift) ? -1F : 0);
    }

    void Move() {
        Interaction interaction = GetComponent<Interaction>();
        Statistics statistics = GetComponent<Statistics>();
        statistics.ATP().regeneration.AddModifier(new Statistics.Modifier(-(interaction.Hoist() != null ? interaction.Hoist().gameObject.GetComponent<Spawnable>().ATP() * Mathf.Abs(zInput) : Mathf.Abs(zInput) * 0.01F), Statistics.Modifier.Operator.ADDITION));
        if (statistics.ATP().value == 0) {
            statistics.Speed().AddModifier(new Statistics.Modifier(0.2F, Statistics.Modifier.Operator.MULTIPLY));
        }

        float calcSpeed = statistics.Speed().CalculatedValue();
        if (controller == MovementController.CLASSIC) {
            Vector3 move = cinemachineCamera.State.CorrectedOrientation * Vector3.forward * zInput + cinemachineCamera.State.CorrectedOrientation * Vector3.right * xInput;
            move.y = 0;
            rb.AddForce(move.normalized * calcSpeed * 10F, ForceMode.Force);
            rb.AddForce(new Vector3(0, yInput, 0) * calcSpeed * 10F, ForceMode.Force);
        } else if (controller == MovementController.VECTOR) {
            Vector3 move = cinemachineCamera.State.CorrectedOrientation * Vector3.forward * zInput + cinemachineCamera.State.CorrectedOrientation * Vector3.right * xInput;
            rb.AddForce(move.normalized * calcSpeed * 10F, ForceMode.Force);
        }

        if (rb.velocity.magnitude > calcSpeed) {
            Vector3 lV = rb.velocity.normalized * calcSpeed;
            rb.velocity = new Vector3(lV.x, lV.y, lV.z);
        }
    }

    public void SetMovementController(MovementController controller) {
        this.controller = controller;
    }

    public enum MovementController {
        CLASSIC, VECTOR
    }
}
