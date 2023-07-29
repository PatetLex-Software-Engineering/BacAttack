using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : Goal
{
    [Header("Components")]
    public GameObject bullet;

    [Header("Values")]
    public float shootDistance;
    public float breakDistance;
    public float bulletSpeed;
    public float interval;
    public List<float> stamps;

    void Start()
    {
        this.title = "Shoot";
    }

    public override bool ShouldEngage(AI ai) {
        GameObject entityToFollow = Closest(ai, ai.Players());
        float dx = ai.transform.position.x - entityToFollow.transform.position.x;
        float dy = ai.transform.position.y - entityToFollow.transform.position.y;
        float dz = ai.transform.position.z - entityToFollow.transform.position.z;
        float d = Mathf.Sqrt(dx * dx + dy * dy + dz * dz);
        if (d <= shootDistance) {
            return true;
        }
        return false;
    }

    private float lastReg;
    public override void Engage(AI ai) {
        base.Engage(ai);
        GameObject entityToShoot = Closest(ai, ai.Players());
        float dx = ai.transform.position.x - entityToShoot.transform.position.x;
        float dy = ai.transform.position.y - entityToShoot.transform.position.y;
        float dz = ai.transform.position.z - entityToShoot.transform.position.z;
        float d = Mathf.Sqrt(dx * dx + dy * dy + dz * dz);
        if (d > breakDistance) {
            ai.Break();
            return;
        }
        float reg = (ai.Time() / interval) - Mathf.FloorToInt(ai.Time() / interval);
        foreach (float f in stamps) {
            if (f > lastReg && f <= reg) {
                GameObject b = Instantiate<GameObject>(bullet);
                Rigidbody rb = b.GetComponent<Rigidbody>();
                b.SetActive(true);
                float tt = Vector3.Distance(entityToShoot.transform.position, ai.transform.position) / bulletSpeed;
                Vector3 direction = ((entityToShoot.transform.position + (rb.velocity * tt)) - ai.transform.position).normalized;
                b.transform.position = ai.transform.position;
                rb.AddForce(direction * bulletSpeed, ForceMode.Impulse);
            }
        }
        lastReg = reg;
    }

    public GameObject Closest(AI ai, List<GameObject> gameObjects) {
        float distance = float.MaxValue;
        GameObject gameObject = gameObjects[0];
        foreach (GameObject go in gameObjects) {
            float dx = ai.transform.position.x - go.transform.position.x;
            float dy = ai.transform.position.y - go.transform.position.y;
            float dz = ai.transform.position.z - go.transform.position.z;
            float d = Mathf.Sqrt(dx * dx + dy * dy + dz * dz);
            if (d < distance) {
                distance = d;
                gameObject = go;
            }
        }
        return gameObject;
    }
}
