using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public const int EVENT_BRICKBREAK = 0;
    public const int EVENT_DEAD = 1;

    public float timer=0;
    private bool speedUp= false;
    public float speed;

    public GameObject Player;
    public delegate void BallEvent(int eventId);
    public BallEvent ballCallBack;

    private Vector2 currentVelocity;
    private Rigidbody2D rb;
    private bool isStart = false;

    const float DEG_LOW_LIMIT = 0.4f;
    const float DEG_HIGH_LIMIT = Mathf.PI / 2.0f - DEG_LOW_LIMIT;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isStart)
        {
            timer += Time.deltaTime;
        }

// 타이머가 10초가 지나면 속도가 50씩 증가 한다 단 최대속도는 350이다.
// 400 넘어가면 맵 밖으로 나가는 버그가 있어서 350으로 제한 했다.
        if (timer >= 10&&!speedUp&&speed<350)
        {
            GetspeedUp();
        }

        // 스피드가 200을 넘어가면 바의 크기가 줄어든다.
        if (speed >= 200)
        {
            Player.transform.localScale = new Vector3(1,1,1);
        }

        if (speedUp && timer >= 10)
        {
            speedUp = false;
        }
    }

    void GetspeedUp()
    {
        speed += 50;
        speedUp = true;
        timer = 0;
    }

    void SetVelocity(Vector2 vel)
    {
        rb.velocity = vel;
        currentVelocity = vel;
    }

    void SetVelocityFromTh(float th)
    {
        Vector2 vel = new Vector2(speed * Mathf.Cos(th),
            speed * Mathf.Sin(th));
        SetVelocity(vel);
    }

    void BounceBar(Transform bar)
    {
        
        // 공이 막대에 맞고 튕기는 경우
        // 막대의 좌우측에 따라 튕기는 방향이 달라진다.
        Vector2 vel = rb.velocity;
        if (bar.position.x > transform.position.x)
        {
            // 공은 왼쪽으로 날아감
            vel.x = -Mathf.Abs(rb.velocity.x);
        }
        else
        {
            // 공은 오른쪽으로 날아감
            vel.x = Mathf.Abs(rb.velocity.x);
        }
        SetVelocity(vel);
        AdjustAngle();
    }

    void AdjustAngle()
    {
        float th = Mathf.Atan2(rb.velocity.y, rb.velocity.x);
        float newTh;
        if (th >= 0)
        {
            // 0 ~ 180도 사이의 처리
            if (th < Mathf.PI / 2.0f)
            {
                // 90 ~ 180도 사이의 처리
                newTh = Mathf.Clamp(th, 
                    DEG_LOW_LIMIT, DEG_HIGH_LIMIT);
                SetVelocityFromTh(newTh);
            }
            else
            {
                newTh = Mathf.Clamp(th, Mathf.PI/2.0f + DEG_LOW_LIMIT, 
                    Mathf.PI/2.0f + DEG_HIGH_LIMIT);
                SetVelocityFromTh(newTh);
            }
        }
        else 
        {
            // 0 ~ -180도 사이의 처리
            if (th > -Mathf.PI / 2.0f)
            {
                // 0 ~ -90도 사이의 처리
                newTh = Mathf.Clamp(th, -DEG_HIGH_LIMIT, -DEG_LOW_LIMIT);
                SetVelocityFromTh(newTh);
            }
            else
            {
                // -90 ~ -180도 사이의 처리
                newTh = Mathf.Clamp(th, -Mathf.PI / 2.0f - DEG_HIGH_LIMIT,
                    -Mathf.PI/2.0f - DEG_LOW_LIMIT );
                SetVelocityFromTh(newTh);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("brick"))
        {
            AudioManager.instance.PlaySfx(AudioManager.Sfx.s);
            // 반사처리
            Destroy(collision.gameObject);
            ballCallBack(EVENT_BRICKBREAK);
            Debug.Log("brick");
            AdjustAngle();
        }
        else if (collision.transform.CompareTag("bar"))
        {
            AudioManager.instance.PlaySfx(AudioManager.Sfx.s);
            // 반사처리
            BounceBar(collision.transform);
            Debug.Log("bar");
        }
        else if (collision.transform.CompareTag("wall"))
        {
            
            AudioManager.instance.PlaySfx(AudioManager.Sfx.s);
            // 반사처리
        }
        else if (collision.transform.CompareTag("dead"))
        {
            timer = 0;
            speed = 100;
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Dead);
            // 사망
            ballCallBack(EVENT_DEAD);
            StopMove();
        }
    }

    public void StartMove()
    {
        if (!isStart)
        {
            // 시작각도로 볼을 발사한다.
            SetVelocityFromTh(Mathf.PI / 4.0f);
            isStart = true;
        }
    }

    public void StopMove()
    {
        currentVelocity =  rb.velocity = Vector2.zero;
        isStart = false;
    }

    
    // 공의 위치를 초기화 
    // 막대의 크기도 초기화 해준다
    public void ResetBall(Vector3 pos)
    {
        Player.transform.localScale = new Vector3(2,1,1);
        rb.position = pos - new Vector3(0, -0.6f, 0);
    }
}