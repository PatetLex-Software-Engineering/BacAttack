using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    [Header("Values")]
    public float damage;

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.GetComponent<Statistics>() != null) {
            Destroy(gameObject);
            Interaction interaction = collision.gameObject.GetComponent<Interaction>();
            interaction.Damage(damage);
        }
    }
}
