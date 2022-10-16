using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{

  public GameObject PauseMenu;
  public bool isPaused;


  void start()
  {
    PauseMenu.SetActive(false);
    
  }


  public void PauseGame()
  {

    PauseMenu.SetActive(true);

    Time.timeScale = 0f;
    isPaused = true;
  }

  public void Resume()
  {
    PauseMenu.SetActive(false);

    Time.timeScale = 1f;
    isPaused = false;
  }

  void Update()
  {
    if(Input.GetKeyDown(KeyCode.Escape))
    {
      if(isPaused==true){
        Resume();
      }
      else{
        PauseGame();
      }
    }
  }

}
