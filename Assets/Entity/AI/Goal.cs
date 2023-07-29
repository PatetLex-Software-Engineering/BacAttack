using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour {

    protected string title;

    public virtual bool ShouldEngage(AI ai) {
        return false;
    }

    public virtual void Engage(AI ai) {

    }

    public string Title() {
        return title;
    }
}
