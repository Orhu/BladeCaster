using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueController : MonoBehaviour
{
  [Header("The Dialogue")]
  [SerializeField] string text;
  [SerializeField] TMP_Text textBox;

  [Header("Dialogue Speed")]
  [SerializeField] float interval;
  [SerializeField] float turnOff;

  void Start(){
    textBox.text = string.Empty;
  }

  IEnumerator TextBoxUpdate(){
    char[] chars = text.ToCharArray();
    int index = 0;
    while(index < chars.Length){
      textBox.text += chars[index];
      index++;
      yield return new WaitForSeconds(interval);
    }
    yield return new WaitForSeconds(turnOff);
    textBox.gameObject.SetActive(false);
  }

  void OnTriggerEnter2D(Collider2D other){
    Debug.Log("Trigger");
    if (other.tag == "Player"){
      textBox.gameObject.SetActive(true);
      StartCoroutine(TextBoxUpdate());
    }
  }
}
