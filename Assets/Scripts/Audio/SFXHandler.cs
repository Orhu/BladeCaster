using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXHandler : MonoBehaviour {
    private AudioSource _voice;

    void Awake() {
        _voice = GetComponent<AudioSource>();
    }

    public void PlaySFX(string path) {
        Debug.Log($"Loading AudioClip at {path}");
        AudioClip clip = Resources.Load(path) as AudioClip;
        _voice.pitch = 1f;
        _voice.PlayOneShot(clip);
    }
    
    public void PlaySFXPitch(string path, float newPitch = 1f) { // range = [-3f, 3f]
        Debug.Log($"Loading AudioClip at {path}");
        AudioClip clip = Resources.Load(path) as AudioClip;
        _voice.pitch = newPitch;
        _voice.PlayOneShot(clip);
    }

    public void PlaySFXLoop(string path) {
        Debug.Log($"Loading AudioClip at {path}");
        AudioClip clip = Resources.Load(path) as AudioClip;
        _voice.loop = true;
        _voice.pitch = 1;
        _voice.clip = clip;
        _voice.Play();
    }

    public void StopLoop() {
        _voice.Stop();
        _voice.clip = null;
        _voice.loop = false;
    }
}
