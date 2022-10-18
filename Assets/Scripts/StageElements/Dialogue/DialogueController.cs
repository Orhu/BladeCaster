using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueController : MonoBehaviour
{
  [Header("Dialogue")]
  [SerializeField] string[] text;
  [SerializeField] TMP_Text textBox;
  [SerializeField] AudioSource speech;

  [Header("Dialogue Speed")]
  [SerializeField] float startWait = 1.5f;
  [SerializeField] float interval;
  [SerializeField] float periodPauseTime = 0.75f;
  [SerializeField] float turnOff;

  public bool isEventTrigger = false;
  public int eventNum = 0;

  //private bool started = false;
  public bool completed = false;

  private IEnumerator textCR;

  void Start(){
    textBox.text = string.Empty;
  }

  IEnumerator TextBoxUpdate(){
    yield return new WaitForSeconds(startWait);
    int line = 0;
    while(line < text.Length){
      char[] chars = text[line].ToCharArray();
      int index = 0;
      while(index < chars.Length){
        textBox.text += chars[index];
        speech.Play();
        if (chars[index] == '.' || chars[index] == '?' || chars[index] == '!') {
          yield return new WaitForSeconds(periodPauseTime);
        } else {
          yield return new WaitForSeconds(interval);
        }
        index++;
      }
      line++;
      yield return new WaitForSeconds(turnOff);
      textBox.text = string.Empty;
    }
    textBox.gameObject.SetActive(false);
    //started = false;
    completed = true;
    textBox.text = string.Empty;
    textCR = null;
    if (isEventTrigger) {
      DoEvent();
    }
  }

  void OnTriggerEnter2D(Collider2D other){
    Debug.Log("Trigger");
    if (other.tag == "Player"){
      textBox.gameObject.SetActive(true);
      if (!completed) {
        textCR = TextBoxUpdate();
        StartCoroutine(textCR);
      }
    }
  }

  void OnTriggerExit2D(Collider2D other) {
    if (other.tag == "Player") {
      if (textCR != null) {
        StopCoroutine(textCR);
      }
      textBox.text = string.Empty;
    }
  }

  public void DoEvent() {
    switch (eventNum) {
      case 1:
        transform.parent.GetComponentInChildren<SDSwitchEvent>().PlayAnimation();
        return;
      case 2:
        transform.parent.GetComponentInChildren<Switch>().MakeInteractable();
        return;
      // there could probably be others if we make a bigger game.
    }
  }
}
