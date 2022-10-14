using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeSpawner : MonoBehaviour, ILevelProp {
    [SerializeField] private GameObject slimePrefab;
    private GameObject _enemy;

    [SerializeField] bool active = true;
    private bool spawning = false;
    private bool aliveCheck = true;

    private Animator _anim;

    [SerializeField] float respawnDelayTime = 2f;

    void Start() {
        _anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() {
        if (_enemy == null && aliveCheck) {
            aliveCheck = false;
            StartCoroutine(RespawnDelay());
        } 

        if (!gameObject.activeSelf || !active) {
            return;
        }

        if (_enemy == null && !spawning) {
            spawning = true;
            aliveCheck = true;
            _anim.SetTrigger("spawn");
        }
    }

    void OnDisable() {
        if (_enemy != null) {
            Destroy(_enemy);
            _enemy = null;
        }
    }

    public void Spawn() {
        _enemy = Instantiate(slimePrefab) as GameObject;
        _enemy.transform.position = new Vector3(transform.position.x, transform.position.y - 0.12f, 0f);
        spawning = false;
        aliveCheck = true;
    }

    public void Interact() {
        // unused
    }

    public void SwitchOperate() {
        active = !active;
    }

    private IEnumerator RespawnDelay() {
        spawning = true;
        yield return new WaitForSeconds(respawnDelayTime);
        spawning = false;
    }
}
