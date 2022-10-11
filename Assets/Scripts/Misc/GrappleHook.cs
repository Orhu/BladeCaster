using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleHook : MonoBehaviour {
    public bool hooked = false; // if the hook has reached it's destination.

    private Vector3 start;
    private Vector3 destination;

    [SerializeField] Sprite[] dirSprites; // 0 = up -> 7 = up left (go around clockwise)

    private SpriteRenderer _sprite;

    void Start() {
        _sprite = GetComponent<SpriteRenderer>();
        gameObject.SetActive(false);
    }

    public void SetUpHook(Vector3 st, Vector3 dest, int dir) {
        gameObject.SetActive(true);
        start = st;
        destination = dest;
        _sprite.sprite = dirSprites[dir];

        StartCoroutine(FireHook());
    }

    private IEnumerator FireHook() {
        float t = 0.0f;
        while (transform.position != destination) {
            transform.position = new Vector3(Mathf.Lerp(start.x, destination.x, t), Mathf.Lerp(start.y, destination.y, t), 0f); // interpolate between start and destination
            
            t = 4f * Time.deltaTime; // update t

            yield return null;
        }
        hooked = true;
    }
}
