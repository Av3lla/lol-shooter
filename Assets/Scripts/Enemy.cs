using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public string enemyName;
    public int enemyScore;
    public float speed;
    public int health;
    public Sprite[] sprites;

    public float maxShotDelay;
    public float curShotDelay;

    public GameObject bulletObjA;
    public GameObject bulletObjB;
    public GameObject player;

    SpriteRenderer spriteRenderer;
    Rigidbody2D rigid;

    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();  
    }

    void Update() {
        Fire();
        Reload();
    }

    void Fire()
    {
        if(curShotDelay < maxShotDelay)
            return;

        if (enemyName=="S") {
            GameObject bullet = Instantiate(bulletObjA, transform.position, transform.rotation);
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Vector3 dirVec = player.transform.position - transform.position;
            rigid.AddForce(dirVec.normalized*3, ForceMode2D.Impulse);
        }
        else if (enemyName=="L")    {
            GameObject bulletL = Instantiate(bulletObjB, transform.position+Vector3.left*0.3f, transform.rotation);
            GameObject bulletR = Instantiate(bulletObjB, transform.position+Vector3.right*0.3f, transform.rotation);

            Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
            Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();


            Vector3 dirVecL = player.transform.position - transform.position;
            Vector3 dirVecR = player.transform.position - transform.position;

            rigidL.AddForce(dirVecL.normalized*4, ForceMode2D.Impulse);
            rigidR.AddForce(dirVecR.normalized*4, ForceMode2D.Impulse);
        }

        curShotDelay = 0;
    }

    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }

    void OnHit(int dmg)
    {
        health -= dmg;
        spriteRenderer.sprite = sprites[1];
        Invoke("ReturnSprite", 0.1f);
        Player playerLogic = player.GetComponent<Player>();
        if (health <= 0)
            Destroy(gameObject);
            playerLogic.score += enemyScore;
    }

    void ReturnSprite()
    {
        spriteRenderer.sprite = sprites[0];
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "BulletBorder")
            Destroy(gameObject);
        else if (collision.gameObject.tag == "PlayerBullet")    {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            OnHit(bullet.dmg);

            Destroy(collision.gameObject);
        }
    }
}