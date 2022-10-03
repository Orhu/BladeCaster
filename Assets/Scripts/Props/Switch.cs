using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour, ILevelProp {
    public ILevelProp[] connectedProps;
    [SerializeField] Sprite[] switchStates;

    public bool active = false;
    public bool interactable = true;

    private Animator _anim;

    void Start() {
        _anim = GetComponent<Animator>();
    }

    public void Interact() {
        if (interactable) {
            active = !active;
            _anim.SetBool("active", active);
            if (connectedProps != null) {
                foreach (ILevelProp connection in connectedProps) {
                    connection.SwitchToggle();
                }
            }
        }
    }

    public void SwitchToggle() {
        interactable = !interactable;
    }
}
