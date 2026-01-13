using System.Collections; // 코루틴을 위해 필요합니다
using UnityEngine;

public class enemy_Hit : MonoBehaviour
{
    SpriteRenderer spr;
    Color originColor;

    void Awake()
    {
        spr = GetComponent<SpriteRenderer>();
        originColor = spr.color;
    }

    void OnEnable()
    {
        // 몬스터가 풀에서 다시 나올 때(스폰될 때) 색상을 원래대로 강제 초기화합니다.
        if (spr != null) {
            spr.color = originColor;
        }
    }

    public void OnHit()// 외부(Bullet 등)에서 데미지를 입었을 때 호출할 함수
    {
        StopCoroutine("FlashRoutine"); // 이미 번쩍이는 중이면 멈추고 새로 시작
        StartCoroutine("FlashRoutine");
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