using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public float speed = 1.0f;
    public int jumps = 1;
    public int jumpForce = 500;
    public int pushdownForce = 50;

    private int jumpsleft = 1;
    private Collider2D ignoring = null;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        if (Input.GetButtonDown("Jump") && jumpsleft > 0)
        {
            jumpsleft--;

            if (ignoring != null)
            {
                Physics2D.IgnoreCollision(ignoring.GetComponent<Collider2D>(), GetComponent<Collider2D>(), false);
                ignoring = null;
            }

            GetComponent<Rigidbody2D>().velocity = new Vector2(0,0);
            GetComponent<Rigidbody2D>().AddForce(Vector2.up * jumpForce);
        }
        if (Input.GetButtonDown("SlideDown"))
        {
            Collider2D temp = Physics2D.OverlapCircle(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 0.55f), 0.20f);
            if(temp.gameObject.tag == "Platform")
            {
                ignoring = temp;
                Physics2D.IgnoreCollision(ignoring.GetComponent<Collider2D>(), GetComponent<Collider2D>());
                GetComponent<Rigidbody2D>().AddForce(Vector2.down * pushdownForce);
            }
        }


        var move = new Vector3(Input.GetAxis("Horizontal") * speed, 0, 0);
        transform.position += move * Time.deltaTime;
	}

    private void OnCollisionEnter2D (Collision2D other)
    {
        if (other.gameObject.layer == 8)
        {
            if (ignoring != null)
            {
                Physics2D.IgnoreCollision(ignoring.GetComponent<Collider2D>(), GetComponent<Collider2D>(), false);
                ignoring = null;
            }
            if (Physics2D.OverlapCircle(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 0.55f), 0.10f).gameObject.layer == 8)
                jumpsleft = jumps;
        }
    }
}
