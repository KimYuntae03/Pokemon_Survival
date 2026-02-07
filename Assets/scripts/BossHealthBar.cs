using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHealthBar : MonoBehaviour
{
    public Image fillImage;         
    public Transform targetBoss;
    public Vector3 offset = new Vector3(0, -100f, 0); // 머리 위 높이 조절

    // 보스가 나타날 때 UI 초기화
    public void InitBossBar(Transform bossTransform,float maxHp)
    {   
        targetBoss = bossTransform; // 대상 설정
        gameObject.SetActive(true);
        UpdateHealthBar(maxHp, maxHp);
    }

    void LateUpdate()
    {
        // 보스가 살아있고 활성화되어 있다면 위치를 갱신
        if (targetBoss != null && targetBoss.gameObject.activeSelf) {
            transform.position = targetBoss.position + offset;
        }
    }
    // 실시간 체력 업데이트
    public void UpdateHealthBar(float currentHp, float maxHp)
    {
        if (fillImage != null) fillImage.fillAmount = currentHp / maxHp;

    }
    public void Hide()
    {
        targetBoss = null;
        gameObject.SetActive(false); // 다시 비활성화
    }
}
