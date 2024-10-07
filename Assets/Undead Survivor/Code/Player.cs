using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public class Player : MonoBehaviour
{
    public Vector2 inputVec;
    public float speed;
    public Scanner scanner;
    public Hand[] hands;
    public RuntimeAnimatorController[] animCon;

    Rigidbody2D rigid;
    SpriteRenderer spriter;
    Animator anim;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        scanner = GetComponent<Scanner>();
        hands = GetComponentsInChildren<Hand>(true);
    }

    void OnEnable()
    {   
        speed *= Character.Speed;
        anim.runtimeAnimatorController = animCon[GameManager.Instance.playerId];
    }

    private void Update()
    {
        if (!GameManager.Instance.isLive)
            return;
        inputVec.x = Input.GetAxisRaw("Horizontal");
        inputVec.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        // 1. 힘을 준다
        // rigid.AddForce(inputVec);

        // 2. 속도 제어
        // rigid.velocity = inputVec;

       if (!GameManager.Instance.isLive)
            return;

        Vector2 nextVec = inputVec.normalized * speed * Time.fixedDeltaTime;
       // 3. 위치 이동
       rigid.MovePosition(rigid.position + nextVec);
    }

    private void LateUpdate()
    {
        if (!GameManager.Instance.isLive)
            return;

        anim.SetFloat("Speed", inputVec.magnitude);

        if (inputVec.x != 0)
        {
            spriter.flipX = inputVec.x < 0;
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (!GameManager.Instance.isLive)
            return;

        GameManager.Instance.health -= Time.deltaTime * 10;

        if (GameManager.Instance.health < 0) {
            for (int index = 2; index < transform.childCount; index++) { 
                transform.GetChild(index).gameObject.SetActive(false);
            }

            anim.SetTrigger("Dead");
            GameManager.Instance.GameOver();
        }
    }
}
