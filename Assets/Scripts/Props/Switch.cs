using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour, ILevelProp {
    [SerializeField] public GameObject[] connectedProps;

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
                foreach (GameObject connection in connectedProps) {
                    connection.GetComponent<ILevelProp>().SwitchOperate();
                }
            }
        }
    }

    public void SwitchOperate() {
        interactable = !interactable;
    }
}
