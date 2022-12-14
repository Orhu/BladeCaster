using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour, ILevelProp {
    [SerializeField] public GameObject[] connectedProps;

    public bool active = false;
    public bool interactable = true;
    public bool selfDisable = false;

    public bool timeOff = false;
    public float delayTime = 0.05f;

    private Animator _anim;
    private SFXHandler _voice;

    void Start() {
        _anim = GetComponent<Animator>();
        _voice = GetComponent<SFXHandler>();

        _anim.SetBool("active", active);
    }

    public void Interact() {
        if (interactable) {
            active = !active;
            if (active) {
                _voice.PlaySFX("Sounds/SFX/switchOn");
            } else {
                _voice.PlaySFX("Sounds/SFX/switchOff");
            }
            _anim.SetBool("active", active);
            if (connectedProps != null) {
                foreach (GameObject connection in connectedProps) {
                    connection.GetComponent<ILevelProp>().SwitchOperate();
                }
            }
            StartCoroutine(Delay());
        }
    }

    public void SwitchOperate() {
        if (!selfDisable) {
            active = !active;
            _anim.SetBool("active", active);
        } else {
            interactable = false;
        }
    }

    private IEnumerator Delay() {
        if (!selfDisable) {
            interactable = false;
            yield return new WaitForSeconds(delayTime);
            interactable = true;
        }
    }

    public void MakeInteractable() {
        interactable = true;
    }
}
