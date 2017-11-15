using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public float speed = 1.0f; //Velocidad de movimento del jugador
    public int jumps = 1;   //Numero de saltos máximos sin tocar el suelo
    public int jumpForce = 500; //Fuerza  que recibe el jugador al saltar
    public int dashForce = 500; //Fuerza  que recibe el jugador al hacer el dash
    public int pushdownForce = 50; //Fuerza  que recibe el jugador al bajar de una plataforma

    private int jumpsleft = 1;  //Saltos que le quedan al jugador
    private bool dash = true;   //Si ha realizado el dash
    private Collider2D ignoring = null; //Plataforma que podemos atravesar
    private Animator anim;  //Animator del player
    private int ground; //Máscara de capa de la capa "terrain", necesaria para algunas cosillas
    private Rigidbody2D body;   //Componente Rigidbody2d del player
    private bool facingRight = false;   //Si esta hacia la izquierda o la derecha

    private int points = 0;

	// Use this for initialization
	void Start () {
        //Inicializamos ciertas variables

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


        //Se mueve en función del valor del eje lateral de movimiento. Luego se gira hacia la izquierda o la derecha y actualiza la condición del animador
        var move = new Vector3(Input.GetAxis("Horizontal") * speed, 0, 0);
        walk(move);

        if (Input.GetButtonDown("Dash") && dash)
        {
            makeDash();
        }
        if (Input.GetButton("Attack"))
        {
            anim.SetTrigger("Attack");
        }

        //Se mueve y comprueba el moviento vertical
        anim.SetFloat("verticalMovement", body.velocity.y);
    }

    //Función cuando choca con algo
    private void OnCollisionEnter2D (Collision2D other)
    {
        if (other.gameObject.layer == 8) //la capa 8 es la del suelo
        {
            hitGround(other);
        }
    }

    //funcion de hacer daño con la espada
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Enemy" && anim.GetCurrentAnimatorStateInfo(0).IsTag("attack"))
        {
            points++;
            print(points);
        }
    }

    //Función para hacer descender la plataforma en la que está.
    private void descendPlatform()
    {
        //Si hay una plataforma cerca de sus pies, la ignora a la hora de hacer colisiones. A demás se aplica una fuerza hacia abajo

        Collider2D temp = Physics2D.OverlapCircle(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 1.6f), 0.5f, ground);
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

        //Si no le quedan saltos, no salta
        if (jumpsleft <= 0)
            return;

        //Actualiza las variables y deja de ignorar la plataforma que estuviera ignorando. Luego aplica una fuerza hacia arriba y actualiza el animador

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
        //Si está ignorando algo, deja de inorarlo, luego comprueba si está sobre el suelo y actualiza las variables.

        if (ignoring != null)
        {
            Physics2D.IgnoreCollision(ignoring.GetComponent<Collider2D>(), GetComponent<Collider2D>(), false);
            ignoring = null;
        }
        Collider2D temp = Physics2D.OverlapCircle(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 1.6f), 0.2f, ground);
        if (temp != null) //De esta forma evitamos que se reseteen lo salto al chocar de frente contra las paredes en el aire.
        {
            anim.SetBool("onGround", true);
            jumpsleft = jumps;
            dash = true;
            anim.SetFloat("verticalMovement", body.velocity.y);
        }
    }
    
    //Esta hay que mejorarla
    private void makeDash()
    {
        body.AddForce(new Vector2(Input.GetAxis("Horizontal"), 0) * dashForce);
    }

    private  void walk(Vector3 movement)
    {
        if (movement.x > 0.1f && !facingRight)
        {
            transform.Rotate(0, 180, 0);
            facingRight = true;
        }
        if (movement.x < -0.1f && facingRight)
        {
            transform.Rotate(0, 180, 0);
            facingRight = false;
        }

        Vector3 temp = transform.position + movement * Time.deltaTime;
        float xdist = 0.21f;
        float yhead = 0.4f;
        float yfoot = -1.1f;
        Collider2D head;
        Collider2D body;
        Collider2D foot;
        if (facingRight) {
            head = Physics2D.OverlapCircle(new Vector2(gameObject.transform.position.x + xdist, gameObject.transform.position.y + yhead), 0.1f, ground);
            body = Physics2D.OverlapCircle(new Vector2(gameObject.transform.position.x + xdist, gameObject.transform.position.y), 0.1f, ground);
            foot = Physics2D.OverlapCircle(new Vector2(gameObject.transform.position.x + xdist, gameObject.transform.position.y + yfoot), 0.1f, ground);
        }
        else
        {
            head = Physics2D.OverlapCircle(new Vector2(gameObject.transform.position.x - xdist, gameObject.transform.position.y + yhead), 0.1f, ground);
            body = Physics2D.OverlapCircle(new Vector2(gameObject.transform.position.x - xdist, gameObject.transform.position.y), 0.1f, ground);
            foot = Physics2D.OverlapCircle(new Vector2(gameObject.transform.position.x - xdist, gameObject.transform.position.y + yfoot), 0.1f, ground);
        }
        if ((foot == null || foot.tag != "Ground") && (head == null || head.tag != "Ground") && (body == null || body.tag != "Ground"))
            transform.position += movement * Time.deltaTime;


        anim.SetFloat("lateralMovement", Mathf.Abs(movement.x));
    }
}
