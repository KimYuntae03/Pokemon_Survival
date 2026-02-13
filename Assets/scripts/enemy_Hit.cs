using System.Collections;
using UnityEngine;

public class enemy_Hit : MonoBehaviour
{
    SpriteRenderer spr;
    Color originColor;
    Rigidbody2D rigid; //넉백 효과를 위한 변수
    public bool isKnockback = false; //넉백중인지 확인하는 변수
    public GameObject damageTextPrefab;//DamageText프리팹 연결할 변수

    void Awake()
    {
        spr = GetComponent<SpriteRenderer>();
        originColor = spr.color;
        rigid = GetComponent<Rigidbody2D>(); 
    }

    void OnEnable()
    {
        if (spr != null) {
            spr.color = originColor;
        }
        isKnockback = false;
        StopAllCoroutines();
    }

    public void OnHit(float damage)
    {
        StopCoroutine("FlashRoutine"); // 이미 번쩍이는 중이면 멈추고 새로 시작
        StartCoroutine("FlashRoutine");
        StopCoroutine("KnockbackRoutine");
        StartCoroutine("KnockbackRoutine");

        if (damageTextPrefab != null) {
            GameObject hudText = Instantiate(damageTextPrefab, transform.position, Quaternion.identity);
            
            // 자식 오브젝트에 있는 TextMesh 컴포넌트를 찾아 데미지 기입
            TextMesh tm = hudText.GetComponentInChildren<TextMesh>();
            if (tm != null) {
                tm.text = Mathf.RoundToInt(damage).ToString();
            }
        }
    }

    IEnumerator KnockbackRoutine()
    {
        isKnockback = true; //피격당해서 밀려나는 중임을 확인

        Vector2 playerPos = GameManager.instance.player.transform.position;
        Vector2 dirVec = (Vector2)transform.position - playerPos;

        rigid.linearVelocity = Vector2.zero; // 속도 초기화
        
        rigid.AddForce(dirVec.normalized * 0.9f, ForceMode2D.Impulse); 

        // 0.2초 동안 뒤로 밀려나는 시간
        yield return new WaitForSeconds(0.2f); 

        isKnockback = false; // 넉백 끝. 다시 player추적.
    }

    IEnumerator FlashRoutine()
    {
        //색상변경,유지,복구
        spr.color = new Color(5f, 5f, 5f, 1f); 
        yield return new WaitForSeconds(0.1f);
        spr.color = originColor;
    }
}