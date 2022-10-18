using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class EndGameEvent : MonoBehaviour, ILevelProp {
    [SerializeField] GameObject player;
    [SerializeField] Image blackscreen;
    [SerializeField] AudioSource _voice;
    [SerializeField] TMP_Text thankYouText;

    [Header("Dialogue")]
    [SerializeField] string[] text;
    [SerializeField] TMP_Text textBox;
    [SerializeField] AudioSource speech;

    [Header("Dialogue Speed")]
    [SerializeField] float startWait = 1.5f;
    [SerializeField] float interval;
    [SerializeField] float periodPauseTime = 0.75f;
    [SerializeField] float turnOff;
    
    void Start() {
        textBox.text = string.Empty;
        thankYouText.gameObject.SetActive(false);
        blackscreen.gameObject.SetActive(false);
    }

    public void Interact() {
        // nothing
    }

    public void SwitchOperate() {
        _voice.Stop();
        StartCoroutine(GameEnd());
    }

    private IEnumerator GameEnd() {
        yield return new WaitForSeconds(3f);

        
        blackscreen.gameObject.SetActive(true);
        player.SetActive(false);

        StartCoroutine(TextBoxUpdate());
    }

    private IEnumerator TextBoxUpdate(){
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
        yield return new WaitForSeconds(turnOff);
        textBox.gameObject.SetActive(false);
        textBox.text = string.Empty;
        StartCoroutine(EndGame());
    }

    private IEnumerator EndGame() {
        thankYouText.gameObject.SetActive(true);
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene(0); // back to menu
    }


}
