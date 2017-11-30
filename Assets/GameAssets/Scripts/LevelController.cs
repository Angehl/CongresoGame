using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour {

    public Text Anouncements;
    public Text PlayerPoints;
    public Text EnemyPoints;
    public Text Timer;
    public GameObject Player;
    public GameObject Enemy;
    static public float time = 30f;

    private int PPoints = 0;
    private int EPoints = 0;
    public float timer = 10f;

    private bool on = true;
    private bool ended = false;
    public AudioSource whistle;
    public AudioSource bip;
    public AudioSource music;

    void Start () {
        on = false;
        Player.gameObject.GetComponent<PlayerMovement>().locks();
        Enemy.gameObject.GetComponent<PlayerMovement>().locks();
        Timer.text = (int)time + "";
    }
	
	// Update is called once per frame
	void Update () {

        if (on)
        {
            updateTexts();
            if(time <= 0)
            {
                gameOver();
            }
        }
        else if (ended == false)
        {
            timer -= Time.deltaTime;
            if(time >= 0)
            {
                if (timer >= 4.0f)
                    Anouncements.text = "Get\nReady";
                else if (timer >= 3.0f)
                {
                    if (Anouncements.text != "3") {
                        bip.Play();
                        music.Play();
                    }
                    Anouncements.text = "3";
                }
                else if (timer >= 2.0f)
                {
                    if(Anouncements.text != "2")
                        bip.Play();
                    Anouncements.text = "2";
                }
                else if (timer >= 1.0f)
                {
                    if (Anouncements.text != "1")
                        bip.Play();
                    Anouncements.text = "1";
                }
                else if (timer >= 0.0f)
                {
                    if (Anouncements.text != "GO!")
                        whistle.Play();
                    Anouncements.text = "GO!";
                }
                else
                {
                    Anouncements.text = "";
                    Player.gameObject.GetComponent<PlayerMovement>().unlocks();
                    Enemy.gameObject.GetComponent<PlayerMovement>().unlocks();
                    on = true;
                }
            }
        }
        else
        {
            timer -= Time.deltaTime;
            if(timer <= 0)
            {
                SceneManager.LoadScene(0);
            }
            Player.gameObject.GetComponent<PlayerMovement>().locks();
            Enemy.gameObject.GetComponent<PlayerMovement>().locks();
        }
    }
    
    private void updateTexts()
    {
        PPoints = Player.gameObject.GetComponent<PlayerMovement>().points;
        EPoints = Enemy.gameObject.GetComponent<PlayerMovement>().points;
        PlayerPoints.text = PPoints + "";
        EnemyPoints.text = EPoints + "";

        time -= Time.deltaTime;
        Timer.text = (int)time + "";
        if (time <= 0)
            Timer.text = "0";
    }

    private void gameOver()
    {
        music.Stop();
        whistle.volume = 1;
        whistle.Play();
        timer = 5;
        ended = true;
        if (EPoints > PPoints)
        {
            Anouncements.color = new Color(0, 255, 0);
            Anouncements.text = "GREEN\nWINS";
        }
        else if (PPoints > EPoints)
        {
            Anouncements.color = new Color(255, 0, 0);
            Anouncements.text = "RED\nWINS";
        }
        else
            Anouncements.text = "TIE";
        on = false;
    }
}
