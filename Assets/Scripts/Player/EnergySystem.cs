using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class EnergySystem : MonoBehaviour
{

  public Image Bar;
  /*
  public TextMesh EText;
  */
  public float energy, maxEnergy = 10;

  private void Update()
  {
    BarFill();
    if(Input.GetKeyDown("x"))
    {
      EnergyReduce();

    }
  }


  private void BarFill()
  {
    Bar.fillAmount = energy / maxEnergy;
  }

  public void EnergyReduce()
  {
    energy -= 1;
  }

}
