using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleHook : MonoBehaviour {
    public bool hooked = false; // if the hook has reached it's destination.

    private Vector3 start;
    private Vector3 destination;

    [SerializeField] Sprite[] dirSprites; // 0 = up -> 7 = up left (go around clockwise)

    public void SetUpHook(Vector3 st, Vector3 dest, int dir) {
        start = st;
        destination = dest;
        GetComponent<SpriteRenderer>().sprite = dirSprites[dir];

        StartCoroutine(FireHook());
    }

    private IEnumerator FireHook() {
        float t = 0.0f;
        while (t != 1f) {
            transform.position = new Vector3(Mathf.Lerp(start.x, destination.x, t), Mathf.Lerp(start.y, destination.y, t), 0f); // interpolate between start and destination
            
            t += 8f * Time.deltaTime; // update t
            if (t > 1f) {
                t = 1f;
            }

            yield return null;
        }
        hooked = true;
    }
}
