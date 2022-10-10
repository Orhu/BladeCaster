using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplePoint : MonoBehaviour, ILevelProp, IGrappleTarget {
    public bool rooted {get; private set;} = true;
    public float weight {get; private set;} = 0f;

    [SerializeField] bool activated = true;

    private bool targeted = false;

    private BoxCollider2D _box;
    private Animator _anim;

    void Start() {
        _box = GetComponent<BoxCollider2D>();
        _anim = GetComponent<Animator>();

        _anim.SetBool("activated", activated);
    }

    void Update() {
        _anim.SetBool("targeted", targeted);
    }
    
    public void Interact() {

    }

    public void SwitchOperate() {
        activated = !activated;
        _anim.SetBool("activated", activated);
    }

    public void Target() {

    }
}
