using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;
    public int per;

    public void Init(float damage, int per) //변수 초기화 함수
    {
        this.damage = damage;
        this.per = per;
    }
    public void LateUpdate()
    {
        if (per == -1) { //근접 회전 무기인 경우
            transform.rotation = Quaternion.identity; //무기 각도를 0으로 고정
        }
    }
}
