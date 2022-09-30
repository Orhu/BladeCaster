using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeapon : MonoBehaviour
{
  public float energyConsump = 1.0f;

  public bool Call()
  {
    if(GetComponent<Player>().UpdateEnergy(energyConsump))
    {
      return false;
    }
    return true;
  }
}
