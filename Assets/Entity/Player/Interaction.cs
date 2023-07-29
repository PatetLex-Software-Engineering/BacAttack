using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TMPro;
using UnityEngine.SceneManagement;

public class Interaction : MonoBehaviour
{
    [Header("Components")]
    public GameObject interactionOverlay;
    public TextMeshProUGUI keyLabel;
    public TextMeshProUGUI actionLabel;

    public LayerMask layer;
    public CinemachineVirtualCameraBase cinemachineCamera;


    [Header("Values")]
    public float reach;
    public float maxAngle;

    private Hoistable hoist;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        Collider[] collidingObjects = Physics.OverlapSphere(this.transform.position, reach, layer);
        float closestAngle = float.MaxValue;
        GameObject closestObject = null;
        for (int i = 0; i < collidingObjects.Length; i++) {
            Collider collider = collidingObjects[i];
            Interactable interactable = collider.gameObject.GetComponent<Interactable>();
            if (interactable != null && interactable.ShouldInteract(this)) {
                float angle = Vector3.Angle(cinemachineCamera.State.CorrectedOrientation * Vector3.forward, collider.gameObject.transform.position - cinemachineCamera.transform.position);
                if (Mathf.Abs(angle) < maxAngle) {
                    float distance = Vector3.Distance(this.transform.position, collider.gameObject.transform.position);
                    if (angle < closestAngle) {
                        closestAngle = angle;
                        closestObject = collider.gameObject;
                    }
                }
            }
        }
        if (closestObject != null) {
            Interactable interactable = closestObject.GetComponent<Interactable>();
            interactionOverlay.transform.position = Camera.main.WorldToScreenPoint(new Vector3(closestObject.transform.position.x, closestObject.transform.position.y + closestObject.transform.localScale.y + 0.2F, closestObject.transform.position.z));
            keyLabel.text = interactable.Key(this).ToString().ToUpper();
            actionLabel.text = interactable.Action(this);
            interactionOverlay.SetActive(true);
            if (Input.GetKey(interactable.Key(this))) {
                interactable.Interact(this);
            }
        } else {
            interactionOverlay.SetActive(false);
        }
    }

    void FixedUpdate() {
        if (hoist != null) {
            Statistics statistics = GetComponent<Statistics>();
            Spawnable spawnable = hoist.gameObject.GetComponent<Spawnable>();
            statistics.Speed().AddModifier(new Statistics.Modifier(hoist.speedRatio * spawnable.Speed(), Statistics.Modifier.Operator.SET));

            AI ai = hoist.gameObject.GetComponent<AI>();
            Rigidbody rigidbody = hoist.gameObject.GetComponent<Rigidbody>();
            Collider collider = hoist.gameObject.GetComponent<Collider>();
            ai.enabled = false;
            collider.enabled = false;
            rigidbody.velocity = GetComponent<Rigidbody>().velocity;
            hoist.gameObject.transform.position = transform.position - new Vector3(0, 0.1F, 0);

            if (Input.GetKey(KeyCode.LeftShift)) {
                UnHoist();
            }
            if (statistics.ATP().value == 0) {
                UnHoist();
            }
        }
    }

    public void Damage(float damage) {
        Statistics statistics = gameObject.GetComponent<Statistics>();
        float preHealth = statistics.Health().value - damage;
        statistics.Health().value -= damage;
        if (preHealth <= 0) {
            GUI gui = gameObject.GetComponent<GUI>();
            gui.SetMenu(gui.death);
        }
    }

    public void Respawn() {
        Statistics statistics = gameObject.GetComponent<Statistics>();
        GUI gui = gameObject.GetComponent<GUI>();

        UnHoist();
        statistics.Health().value = statistics.Health().maxValue;
        statistics.ATP().value = statistics.ATP().maxValue;
        gameObject.transform.position = new Vector3(12, 12, 12);

        gui.Close();
    }

    public void Leave() {
        SceneManager.LoadSceneAsync("Menu");
    }

    public Hoistable Hoist() {
        return hoist;
    }

    public void SetHoist(Hoistable hoist) {
        UnHoist();
        this.hoist = hoist;
        GetComponent<Movement>().SetMovementController(Movement.MovementController.VECTOR);
        cinemachineCamera.gameObject.GetComponent<CinemachineFreeLook>().m_Lens.FieldOfView = 60;
    }

    public void UnHoist() {
        if (this.hoist != null) {
            cinemachineCamera.gameObject.GetComponent<CinemachineFreeLook>().m_Lens.FieldOfView = 50;
            GetComponent<Movement>().SetMovementController(Movement.MovementController.CLASSIC);
            AI ai = hoist.gameObject.GetComponent<AI>();
            Collider collider = hoist.gameObject.GetComponent<Collider>();
            ai.enabled = true;
            collider.enabled = true;
            hoist = null;
        }   
    }
}
