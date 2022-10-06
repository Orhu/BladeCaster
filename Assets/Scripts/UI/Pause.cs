using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{

  public GameObject PauseMenu;
  public void PausePress()
  {

    PauseMenu.SetActive(true);

    Time.timeScale = 0f;
  }

  public void Resume()
  {
    PauseMenu.SetActive(false);

    Time.timeScale = 1f;
  }

  void Update()
  {
    if(Input.GetKeyDown(KeyCode.Escape))
    {
      PausePress();
    }
  }




}
