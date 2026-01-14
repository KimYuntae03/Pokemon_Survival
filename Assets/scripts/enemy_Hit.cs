using System.Collections;
using UnityEngine;

public class enemy_Hit : MonoBehaviour
{
    SpriteRenderer spr;
    Color originColor;
    Rigidbody2D rigid; //넉백 효과를 위한 변수
    public bool isKnockback = false; //넉백중인지 확인하는 변수
    public AudioClip hitSound; //인스펙터에서 넣을 효과음 파일
    AudioSource audioSource; //효과음 재생할 컴포넌트

    void Awake()
    {
        spr = GetComponent<SpriteRenderer>();
        originColor = spr.color;
        rigid = GetComponent<Rigidbody2D>(); 
        audioSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        // 몬스터가 풀에서 다시 나올 때(스폰될 때) 색상을 원래대로 강제 초기화합니다.
        if (spr != null) {
            spr.color = originColor;
        }
        isKnockback = false;
        StopAllCoroutines();
    }

    public void OnHit()// 외부(Bullet 등)에서 데미지를 입었을 때 호출할 함수
    {
        StopCoroutine("FlashRoutine"); // 이미 번쩍이는 중이면 멈추고 새로 시작
        StartCoroutine("FlashRoutine");

        StopCoroutine("KnockbackRoutine");
        StartCoroutine("KnockbackRoutine");
        if (audioSource != null && hitSound != null) {
            audioSource.PlayOneShot(hitSound); // 소리를 한 번 재생
        }
    }

    IEnumerator KnockbackRoutine()
    {
        isKnockback = true; //피격당해서 밀려나는 중임을 확인

        Vector2 playerPos = GameManager.instance.player.transform.position;
        Vector2 dirVec = (Vector2)transform.position - playerPos;

        rigid.linearVelocity = Vector2.zero; // 속도 초기화
        
        rigid.AddForce(dirVec.normalized * 1.2f, ForceMode2D.Impulse); 

        // 0.2초 동안 뒤로 밀려나는 시간
        yield return new WaitForSeconds(0.2f); 

        isKnockback = false; // 넉백 끝. 다시 player추적.
    }

    IEnumerator FlashRoutine()
    {
        // 1. 색상을 아주 밝게 변경 (번쩍이는 효과)
        spr.color = new Color(5f, 5f, 5f, 1f); 

        // 2. 0.1초 동안 유지
        yield return new WaitForSeconds(0.1f);

        // 3. 원래 저장해두었던 색상으로 복구
        spr.color = originColor;
    }
}