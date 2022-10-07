using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
  [SerializeField] public GameObject[] connectedProps;

  public bool interactable = true;

  private void OnTriggerEnter2D(Collider2D other){
    if(other.gameObject.layer == LayerMask.NameToLayer("Player")){
      this.gameObject.SetActive(false);
      if (connectedProps != null) {
          foreach (GameObject connection in connectedProps) {
              connection.GetComponent<ILevelProp>().SwitchOperate();
          }
      }
    }
  }
}
