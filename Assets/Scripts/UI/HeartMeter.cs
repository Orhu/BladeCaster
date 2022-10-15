using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartMeter : MonoBehaviour {
    [SerializeField] Image[] hearts; // should be 6 of these in order left to right
    [SerializeField] Sprite[] heartSprites; // 0 = empty, 1 = half, 2 = whole

    private int[] heartStates = {2, 2, 2, -1, -1, -1}; // -1 = hidden, 0 = empty, 1 = half, 2 = whole

    // track current and max health locally to only update what's necessary
    private int mHealth = 6;

    public void Refresh(int curHealth, int maxHealth) {
        if (maxHealth != mHealth) {
            mHealth = maxHealth;
            switch (maxHealth) {
                case 8:
                    heartStates[3] = 0;
                    break;
                case 10:
                    heartStates[4] = 0;
                    break;
                case 12:
                    heartStates[5] = 0;
                    break;
            }
        }

        if (curHealth <= 0) {
            for (int i = 0; i < 6; i++) {
                if (heartStates[i] != -1) {
                    heartStates[i] = 0;
                }
            }
        } else if (curHealth >= maxHealth) {
            for (int i = 0; i < 6; i++) {
                if (heartStates[i] != -1) {
                    heartStates[i] = 2;
                }
            }
        } else { // fill up like one of those assembly line thingys
            for (int i = 0; i < 6; i++) {
                if (heartStates[i] == -1) {
                    break;
                }
                heartStates[i] = 0;
                if (curHealth != 0) {
                    for (int j = 0; j < 2; j++) {
                        Debug.Log($"adding 1 to heartStates[{i}]. {curHealth}, {j}");
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
        for (int i = 0; i < 6; i++) {
            Debug.Log(i);
            if (heartStates[i] == -1) {
                hearts[i].gameObject.SetActive(false);
            } else {
                hearts[i].gameObject.SetActive(true);
                Debug.Log(heartStates[i]);
                hearts[i].sprite = heartSprites[heartStates[i]];
            }
        }
    }
}
