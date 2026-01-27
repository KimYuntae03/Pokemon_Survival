using UnityEngine;

public class SelfDisable : MonoBehaviour
{   
    [SerializeField] private float disableTime = 3f;

    void OnEnable()
    {
        // 0.8초 뒤에 자동으로 꺼짐
        Invoke("Disable", disableTime);
    }

    void Disable()
    {
        // 꺼지기 직전에 부모 관계를 먼저 끊어줌
        transform.parent = null; 
        
        // 그 다음 오브젝트를 비활성화
        gameObject.SetActive(false);
    }

    void OnDisable()
    {
        // OnDisable에서는 예약 취소만 담당
        CancelInvoke();
    }
}