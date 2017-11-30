using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraControl : MonoBehaviour {

    public GameObject P1;
    public GameObject P2;
    public float movspeed = 20;
    public float maxMov = 50;
	
	// Update is called once per frame
	void Update () {
        float x = (P1.transform.position.x + P2.transform.position.x) / 2 - transform.position.x;
        float y = (P1.transform.position.y + P2.transform.position.y) / 2 - transform.position.y;

        Vector3 mov = new Vector3(x, y, 0) * movspeed * Time.deltaTime;
        if (mov.x > maxMov)
            mov.x = maxMov;
        if (mov.x < -maxMov)
            mov.x = -maxMov;
        if (mov.y > maxMov)
            mov.y = maxMov;
        if (mov.y < -maxMov)
            mov.y = -maxMov;

        transform.Translate(mov);
        float dist = Mathf.Abs(P1.transform.position.x - P2.transform.position.x);
        float siz = (dist / 30) * 10 + 1;
        if (siz < 5)
            siz = 5;
        if (siz > 15)
            siz = 15;
        GetComponent<Camera>().orthographicSize = siz;
    }
}
