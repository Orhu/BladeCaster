using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummySpawner : MonoBehaviour
{
  [SerializeField] private GameObject Slime;
  private GameObject _enemy;

  void Update(){
    if (_enemy == null){
      _enemy = Instantiate(Slime) as GameObject;
      _enemy.transform.position = new Vector3(-0.801f, -0.819f, 0f);
    }
  }
}
