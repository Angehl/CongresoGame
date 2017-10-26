using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public float speed = 1.0f;
    public int jumps = 1;
    public int jumpForce = 500;
    public int dashForce = 500;
    public int pushdownForce = 50;

    private int jumpsleft = 1;
    private bool dash = true;
    private Collider2D ignoring = null;
    private Animator anim;
    private int ground;
    private Rigidbody2D body;
    private bool facingRight = false;

	// Use this for initialization
	void Start () {
        body = GetComponent<Rigidbody2D>();
        ground = 1 << LayerMask.NameToLayer("Terrain");
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() {

        if (Input.GetButtonDown("Jump"))
        {
            jump();
        }
        if (Input.GetButtonDown("SlideDown"))
        {
            descendPlatform();
        }


        var move = new Vector3(Input.GetAxis("Horizontal") * speed, 0, 0);
        if (move.x > 0.1f && !facingRight) {
            transform.Rotate(0, 180, 0);
            facingRight = true;
        }
        if (move.x < -0.1f && facingRight)
        {
            transform.Rotate(0, 180, 0);
            facingRight = false;
        }

        anim.SetFloat("lateralMovement", Mathf.Abs( move.x));

        if (Input.GetButtonDown("Dash") && dash)
        {
           body.AddForce(new Vector2(Input.GetAxis("Horizontal"), 0) * dashForce);
        }

        transform.position += move * Time.deltaTime;
        anim.SetFloat("verticalMovement", body.velocity.y);
    }

    private void OnCollisionEnter2D (Collision2D other)
    {
        if (other.gameObject.layer == 8)
        {
            hitGround(other);
        }
    }

    private void descendPlatform()
    {
        Collider2D temp = Physics2D.OverlapCircle(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 2f), 0.5f, ground);
        print(temp);
        if (temp != null && temp.gameObject.tag == "Platform")
        {
            anim.SetBool("onGround", false);
            ignoring = temp;
            Physics2D.IgnoreCollision(ignoring.GetComponent<Collider2D>(), GetComponent<Collider2D>());
            body.AddForce(Vector2.down * pushdownForce);
            anim.SetFloat("verticalMovement", body.velocity.y);
        }
    }

    private void jump() {
        if (jumpsleft <= 0)
            return;

        anim.SetBool("onGround", false);
        jumpsleft--;

        if (ignoring != null)
        {
            Physics2D.IgnoreCollision(ignoring.GetComponent<Collider2D>(), GetComponent<Collider2D>(), false);
            ignoring = null;
        }

        body.velocity = new Vector2(0, 0);
        body.AddForce(Vector2.up * jumpForce);
        anim.SetFloat("verticalMovement", body.velocity.y);
    }

    private void hitGround(Collision2D other) {
        if (ignoring != null)
        {
            Physics2D.IgnoreCollision(ignoring.GetComponent<Collider2D>(), GetComponent<Collider2D>(), false);
            ignoring = null;
        }
        Collider2D temp = Physics2D.OverlapCircle(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 2f), 0.5f, ground);
        if (temp != null)
        {
            anim.SetBool("onGround", true);
            jumpsleft = jumps;
            dash = true;
            anim.SetFloat("verticalMovement", body.velocity.y);
        }
    }

}
