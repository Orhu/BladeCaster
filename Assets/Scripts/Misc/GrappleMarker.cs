using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleMarker : MonoBehaviour {
    public void SetUpMarker(GameObject parentObj, int direction) {
        GetComponent<Animator>().SetInteger("direction", direction);
        GetComponent<Animator>().SetTrigger("update");
        gameObject.transform.position = parentObj.transform.position + new Vector3(0f,0f,-0.1f);
        gameObject.transform.parent = parentObj.transform;
    }

    public void UpdateMarker(int direction) {
        if (GetComponent<Animator>().GetInteger("direction") != direction) {
            GetComponent<Animator>().SetInteger("direction", direction);
            GetComponent<Animator>().SetTrigger("update");
        }
    }
}
