using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplePoint : MonoBehaviour, ILevelProp, IGrappleTarget {
    public bool active {get; set;} = true; // i guess for now they're always going to be true?

    //private Animator _anim;

    /*void Start() {
        _anim = GetComponent<Animator>();
        _anim.SetBool("activated", active);
    }*/

    public void Interact() {
        // unused
    }

    public void SwitchOperate() {
        active = !active;
        //_anim.SetBool("activated", active);
    }
}
