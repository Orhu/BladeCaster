using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTrigger : MonoBehaviour {
    [SerializeField] SpearBossPhase1 _boss;
    [SerializeField] Door _door;

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            _door.SwitchOperate();
            _boss.BeginFight();
            Destroy(this.gameObject);
        }
    }
}
