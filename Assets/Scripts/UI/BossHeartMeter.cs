using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHeartMeter : MonoBehaviour {
    [SerializeField] Image[] hearts; // should be 6 of these in order left to right
    [SerializeField] Sprite[] heartSprites; // 0 = empty, 1 = half, 2 = whole

    private int[] heartStates = {2, 2, 2, 2, 2, 2, 2, 2, 2, 2}; // 0 = empty, 1 = half, 2 = whole

    void Start() {
        foreach (Image heart in hearts) {
            heart.gameObject.SetActive(false);
        }
    }

    public void StartFight() {
        foreach (Image heart in hearts) {
            heart.gameObject.SetActive(true);
        }
    }

    public void Refresh(int curHealth) {
        if (curHealth <= 0) {
            for (int i = 0; i < 10; i++) {
                if (heartStates[i] != -1) {
                    heartStates[i] = 0;
                }
            }
        } else if (curHealth == 20) {
            for (int i = 0; i < 10; i++) {
                if (heartStates[i] != -1) {
                    heartStates[i] = 2;
                }
            }
        } else { // fill up like one of those assembly line thingys
            for (int i = 0; i < 10; i++) {
                if (heartStates[i] == -1) {
                    break;
                }
                heartStates[i] = 0;
                if (curHealth != 0) {
                    for (int j = 0; j < 2; j++) {
                        heartStates[i] += 1;
                        curHealth -= 1;
                        if (curHealth == 0) {
                            break;
                        }
                    }
                }
                
            }
        }    

        ShowRefresh();
    }

    private void ShowRefresh() {
        for (int i = 0; i < 10; i++) {
            if (heartStates[i] == -1) {
                hearts[i].gameObject.SetActive(false);
            } else {
                hearts[i].gameObject.SetActive(true);
                hearts[i].sprite = heartSprites[heartStates[i]];
            }
        }
    }

    public void EndFight() {
        Onboarder.SPEAR_BOSS_DEFEATED = true;
        foreach (Image heart in hearts) {
            heart.gameObject.SetActive(false);
        }
    }
}
