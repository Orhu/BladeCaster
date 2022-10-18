using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTrigger : MonoBehaviour {
    [SerializeField] SpearBossPhase1 _boss;
    [SerializeField] Door _door;
    [SerializeField] BoxCollider2D _safetySquare;
    [SerializeField] SpearBossMain _bossManager;

    public int type;

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            if (type == 0) {
                _door.SwitchOperate();
                _bossManager.Anticipation();
            } else {
                _boss.BeginFight();
                _bossManager.StartFight();
                _safetySquare.enabled = true;
                _safetySquare.GetComponent<Rigidbody2D>().simulated = true;
            }
            Destroy(this.gameObject);
        }
    }
}
