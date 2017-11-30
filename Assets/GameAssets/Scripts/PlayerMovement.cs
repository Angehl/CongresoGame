using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public float speed = 1.0f; //Velocidad de movimento del jugador
    public int jumps = 1;   //Numero de saltos máximos sin tocar el suelo
    public int jumpForce = 500; //Fuerza  que recibe el jugador al saltar
    public float dashSpeed = -1f; //Fuerza  que recibe el jugador al hacer el dash
    public int pushdownForce = 50; //Fuerza  que recibe el jugador al bajar de una plataforma

    private int damage = 10;
    private int type = 1;
    public string horizontalAxis;
    public string verticalAxis;
    public string jumpButton;
    public string attackButton;
    public string dashkButton;
    public GameObject other;

    private bool attacking = false; //Indica si está atacando
    private bool locked = false;    //Indica si está bloqueado en la animación. Si está bloqueado no recibe inputs
    private bool inmune = false;    //Indica si es inmune al daño
    private bool dashing = false;
    private bool midair = false;    //Midair indica si está suspendido en el aire

    public AudioSource hit1;
    public AudioSource hit2;
    public AudioSource hit3;
    public AudioSource swordhit;
    private AudioSource[] hits = new AudioSource[3];

    private Collider2D enemyHitted = null;

    private int jumpsleft = 1;  //Saltos que le quedan al jugador
    private bool dash = true;   //Si ha realizado el dash
    private Collider2D ignoring = null; //Plataforma que podemos atravesar
    public Animator anim;  //Animator del player
    private int ground; //Máscara de capa de la capa "terrain", necesaria para algunas cosillas
    private Rigidbody2D body;   //Componente Rigidbody2d del player
    public bool facingRight = false;   //Si esta hacia la izquierda o la derecha

    private int combo = 0;
    public int points = 0;

    // Use this for initialization
    void Start() {
        //Inicializamos ciertas variables

        hits[0] = hit1;
        hits[1] = hit2;
        hits[2] = hit3;
        body = GetComponent<Rigidbody2D>();

        ground = 1 << LayerMask.NameToLayer("Terrain");
        Physics2D.IgnoreCollision(other.GetComponent<Collider2D>(), this.GetComponent<Collider2D>());
    }



    // Update is called once per frame
    void Update() {

        if (Input.GetButtonDown(jumpButton) && !locked)
        {
            jump();
        }

        if (midair)
        {
            body.velocity = Vector2.zero;
        }

        if (onGround())
        {
            anim.SetBool("onGround", true);
            dash = true;
            jumpsleft = jumps;
        }
        else
            anim.SetBool("onGround", false);

        //Se mueve en función del valor del eje lateral de movimiento. Luego se gira hacia la izquierda o la derecha y actualiza la condición del animador
        if (!locked)
        {
            var move = new Vector3(Input.GetAxis(horizontalAxis) * speed, body.velocity.y, 0);
            walk(move);
        }

        if (dashing)
        {
            var move = new Vector3(dashSpeed, body.velocity.y, 0);
            walk(move);
        }

        if (Input.GetButtonDown(dashkButton) && dash && !locked)
            {
                dash = false;
                anim.SetTrigger("Dash");
            }

        if (Input.GetButtonDown(attackButton))
        {
            anim.SetTrigger("Attack");
        }

        //Se mueve y comprueba el moviento vertical
        anim.SetFloat("verticalMovement", body.velocity.y);

        anim.SetFloat("Xaxis", Input.GetAxis(horizontalAxis));
        anim.SetFloat("Yaxis", Input.GetAxis(verticalAxis));
    }



    //Función cuando choca con algo
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == 8) //la capa 8 es la del suelo
        {
            if (onGround())
            {
                anim.SetBool("onGround", true);
                jumpsleft = jumps;
                dash = true;
                anim.SetFloat("verticalMovement", body.velocity.y);
            }
        }
    }


    //funcion de hacer daño con la espada
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 9 && other.gameObject.tag != gameObject.tag && attacking)
        {
            if (other.gameObject.name == "Sword")
                return;
            points += other.gameObject.GetComponent<PlayerMovement>().hit(damage, type) * (1 + combo);
            combo++;
            enemyHitted = other;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == 9 && other.gameObject.tag != gameObject.tag)
        {
            if (other.gameObject.name == "Sword")
                return;
            enemyHitted = null;
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
        }
    }


    private void jump() {

        if (Input.GetAxis(verticalAxis) < -0.1)
        {
            descendPlatform();
            return;
        }

        //Si no le quedan saltos, no salta
        if (jumpsleft <= 0)
            return;

        //Actualiza las variables y deja de ignorar la plataforma que estuviera ignorando. Luego aplica una fuerza hacia arriba y actualiza el animador

        anim.SetTrigger("Jump");
        jumpsleft--;

        if (ignoring != null)
        {
            Physics2D.IgnoreCollision(ignoring.GetComponent<Collider2D>(), GetComponent<Collider2D>(), false);
            ignoring = null;
        }

        body.velocity = Vector2.zero;
        body.AddForce(Vector2.up * jumpForce);
    }

    //Devuelve si está o no tocando el suelo
    private bool onGround() {

        Collider2D temp = Physics2D.OverlapCircle(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 1.6f), 0.2f, ground);
        if (temp != null) //De esta forma evitamos que se reseteen lo salto al chocar de frente contra las paredes en el aire.
        {
            return true;
        }
        return false;
    }


    //Comienza el dash
    private void makeDash()
    {
        locked = true;
        midair = true;
    }

    private void dashMovement()
    {
       if ((dashSpeed < 0 && facingRight) || (dashSpeed > 0 && !facingRight))
            dashSpeed *= -1;

        dashing = !dashing;
        inmune = !inmune;

        if (dashing == false)
            body.velocity = Vector2.zero;
    }

    private void endDash()
    {
        locked = false;
        midair = false;
        dash = false;
        dashing = false;
        inmune = false;
    }

    private void walk(Vector3 movement)
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
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("RunAttack"))
            attacking = true;
        else if (attacking == true)
            attacking = false;
        /* float xdist = 0.21f;
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
         if ((foot == null || foot.tag != "Ground") && (head == null || head.tag != "Ground") && (body == null || body.tag != "Ground"))*/
        body.velocity = movement;
        anim.SetFloat("lateralMovement", Mathf.Abs(movement.x));
    }


    public int hit(int damage, int type)
    {
        if (inmune)
            return 0;

        dashing = false;
        attacking = false;
        midair = false;
        int i = UnityEngine.Random.Range(0,2);
        hits[i].Play();

        anim.SetTrigger("Damaged");
        combo = 0;

        if (attacking)
            damage *= 2;

        if (type == 2)
            damage *= 2;

        return damage;
    }

    private void attack(int t)
    {
        type = t;
        attacking = !attacking;
        if(t > 0)
            locked = !locked;
        if(enemyHitted != null && locked)
            points += enemyHitted.gameObject.GetComponent<PlayerMovement>().hit(damage, type) * (1 + combo);
        if (!locked && type == 2)
            anim.ResetTrigger("Attack");
        if (attacking == true)
            swordhit.Play();
    }

    public void locks()
    {
        locked = true;
    }

    public void unlocks()
    {
        locked = false;
    }

    private void suspend()
    {
        midair = true;
    }

    private void unsuspend()
    {
        midair = false;
    }
}