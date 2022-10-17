using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTrigger : MonoBehaviour {
    [SerializeField] SpearBossPhase1 _boss;
    [SerializeField] Door _door;
    [SerializeField] BoxCollider2D _safetySquare;
    [SerializeField] AudioSource _voice;

    public int type;

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            if (type == 0) {
                _door.SwitchOperate();
                _voice.Stop();
            } else {
                _boss.BeginFight();
                _voice.clip = Resources.Load("Sounds/Music/boss") as AudioClip;
                _voice.Play();
                _safetySquare.enabled = true;
                _safetySquare.GetComponent<Rigidbody2D>().simulated = true;
            }
            Destroy(this.gameObject);
        }
    }
}
