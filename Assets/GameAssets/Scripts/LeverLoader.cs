using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeverLoader : MonoBehaviour {

    public float time = 0;
    public GameObject canvas;
    
    public void LoadByIndex(int index)
    {
        LevelController.time = this.time;
        AudioSource audio = canvas.GetComponent<AudioSource>();
        audio.Play();
        SceneManager.LoadScene(index);
    }

    public void Exit()
    {
        Application.Quit();
        AudioSource audio = canvas.GetComponent<AudioSource>();
        audio.Play();
    }

}
