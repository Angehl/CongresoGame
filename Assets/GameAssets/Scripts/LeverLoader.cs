using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeverLoader : MonoBehaviour {

    public float time = 0;
    
    public void LoadByIndex(int index)
    {
        LevelController.time = this.time;
        SceneManager.LoadScene(index);
        GetComponent<AudioSource>().Play();
    }

    public void Exit()
    {
        GetComponent<AudioSource>().Play();
        Application.Quit();
    }

}
