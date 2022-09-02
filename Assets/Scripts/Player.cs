using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool isTouchTop;
    public bool isTouchBottom;
    public bool isTouchLeft;
    public bool isTouchRight;
    
    public int life;
    public int score;
    public float speed;
    public float power;
    public float maxShotDelay;
    public float curShotDelay;

    public GameObject bulletObjA;
    public GameObject bulletObjB;

    public GameManager manager;
    public bool isHit;

    public bool isRespawnTime = true;

    Animator anim;
    SpriteRenderer spriteRenderer;

    void Awake() {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        Invincible();
        Invoke("Invincible", 3);
    }

    void Invincible()
    {
        isRespawnTime = !isRespawnTime;
        if (isRespawnTime) {
            spriteRenderer.color = new Color(1, 1, 1, 0.5f);
        }
        else    {
            spriteRenderer.color = new Color(1, 1, 1, 1);
        }
    }

    void Update()
    {
        Move();
        Fire();
        Reload();
    }

    void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        if ((isTouchRight && h == 1) || (isTouchLeft && h == -1))
            h = 0;

        float v = Input.GetAxisRaw("Vertical");
        if ((isTouchTop && v == 1) || (isTouchBottom && v == -1))
            v = 0;

        Vector3 curPos = transform.position;
        Vector3 nextPos = new Vector3(h, v, 0) * speed * Time.deltaTime;

        transform.position = curPos + nextPos;

        if (Input.GetButtonDown("Horizontal") || Input.GetButtonUp("Horizontal"))
        {
            anim.SetInteger("Input", (int)h);
        }
    }

    void Fire()
    {
        if(!Input.GetButton("Fire1"))
            return;

        if(curShotDelay < maxShotDelay)
            return;

        switch (power)
        {
            case 1:
                GameObject bullet = Instantiate(bulletObjA, transform.position, transform.rotation);
                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                rigid.AddForce(Vector2.up*10, ForceMode2D.Impulse);
                break;
            case 2:
                GameObject bulletL = Instantiate(bulletObjA, transform.position + Vector3.left*0.1f, transform.rotation);
                GameObject bulletR = Instantiate(bulletObjA, transform.position + Vector3.right*0.1f, transform.rotation);
                Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
                rigidL.AddForce(Vector2.up*10, ForceMode2D.Impulse);
                rigidR.AddForce(Vector2.up*10, ForceMode2D.Impulse);
                break;
            case 3:
                GameObject bulletLL = Instantiate(bulletObjA, transform.position + Vector3.left*0.35f, transform.rotation);
                GameObject bulletCC = Instantiate(bulletObjB, transform.position, transform.rotation);
                GameObject bulletRR = Instantiate(bulletObjA, transform.position + Vector3.right*0.35f, transform.rotation);
                Rigidbody2D rigidLL = bulletLL.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidCC = bulletCC.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidRR = bulletRR.GetComponent<Rigidbody2D>();
                rigidLL.AddForce(Vector2.up*10, ForceMode2D.Impulse);
                rigidCC.AddForce(Vector2.up*10, ForceMode2D.Impulse);
                rigidRR.AddForce(Vector2.up*10, ForceMode2D.Impulse);
                break;
            default:
                break;
        }
        
        curShotDelay = 0;
    }

    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Border")
        {
            switch(collision.gameObject.name)
            {
                case "Top":
                    isTouchTop = true;
                    break;
                case "Bottom":
                    isTouchBottom = true;
                    break;
                case "Left":
                    isTouchLeft = true;
                    break;
                case "Right":
                    isTouchRight = true;
                    break;
            }
        }
        else if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "EnemyBullet")  {
            if (isRespawnTime)
                return;
            
            if (isHit)
                return;

            isHit = true;            
            life--;
            manager.UpdateLifeIcon(life);

            if (life == 0)  {
                manager.GameOver();
            }
            else    {
                manager.RespawnPlayer();
            }

            gameObject.SetActive(false);
            Destroy(collision.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Border")
        {
            switch(collision.gameObject.name)
            {
                case "Top":
                    isTouchTop = false;
                    break;
                case "Bottom":
                    isTouchBottom = false;
                    break;
                case "Left":
                    isTouchLeft = false;
                    break;
                case "Right":
                    isTouchRight = false;
                    break;
            }
        }
    }
}
