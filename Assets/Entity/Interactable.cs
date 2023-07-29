using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public virtual bool ShouldInteract(Interaction player) {
        return true;
    }

    public abstract void Interact(Interaction player);
    
    public virtual KeyCode Key(Interaction player) {
        return KeyCode.E;
    }

    public virtual string Action(Interaction player) {
        return "Interact";
    }
}
