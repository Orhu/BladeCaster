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
  [SerializeField] float interval;
  [SerializeField] float turnOff;

  private bool started = false;

  void Start(){
    textBox.text = string.Empty;
  }

  IEnumerator TextBoxUpdate(){
    int line = 0;
    while(line < text.Length){
      char[] chars = text[line].ToCharArray();
      int index = 0;
      while(index < chars.Length){
        textBox.text += chars[index];
        speech.Play();
        index++;
        yield return new WaitForSeconds(interval);
      }
      line++;
      yield return new WaitForSeconds(turnOff);
      textBox.text = string.Empty;
    }
    yield return new WaitForSeconds(turnOff);
    textBox.gameObject.SetActive(false);
    started = false;
    textBox.text = string.Empty;
  }

  void OnTriggerEnter2D(Collider2D other){
    Debug.Log("Trigger");
    if (other.tag == "Player" && !started){
      started = true;
      textBox.gameObject.SetActive(true);
      StartCoroutine(TextBoxUpdate());
    }
  }
}
