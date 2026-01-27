using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;
    public int per ;
    public int weaponId; //어떤 무기인지 확인용 변수

    Rigidbody2D rigid;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    public void Init(float damage, int per,Vector3 dir, int id) //변수 초기화 함수
    {
        this.damage = damage;
        this.per = per;
        this.weaponId = id;

        if (per > -1) { //원거리 무기 일때만 물리속도 적용
            rigid.linearVelocity = dir * 15f;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Enemy와 부딪혔는지 확인
        // 혹은 이미 관통력이 다한 무기인지 확인
        if (!collision.CompareTag("Enemy") || per == -1)
            return;

        // 관통력 1 감소
        per--;

        // 관통력을 다 썼다면 비활성화
        if (per < 0) {
            rigid.linearVelocity = Vector2.zero;
            if (weaponId != 2) { 
                gameObject.SetActive(false); 
            }
        }
    }

    public void LateUpdate()
    {
        if (per == -1) { //근접 회전 무기인 경우
            transform.rotation = Quaternion.identity; //무기 각도를 0으로 고정
        }
    }
    void OnBecameInvisible()
    {
        // 원거리무기만 화면밖으로 나갔을 때를 취급하므로 근접무기는 제외 
        if (per == -1)
            return;

        // 화면 밖으로 나가면 물리 속도를 멈추고 풀로 돌려보냄
        rigid.linearVelocity = Vector2.zero;
        gameObject.SetActive(false); //비활성화
    }
}
