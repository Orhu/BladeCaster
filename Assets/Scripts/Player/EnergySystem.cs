using UnityEngine.UI;
using UnityEngine;



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
