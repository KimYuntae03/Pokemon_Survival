using UnityEngine;

public class Item : MonoBehaviour
{
    public float speed = 5f; // 보석이 날아오는 속도
    public float magnetRange = 2.0f;
    Rigidbody2D rig;
    Transform target; // 플레이어의 위치를 담을 변수

    void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        // 보석이 생성될 때마다 타겟(플레이어)을 초기화
        target = GameManager.instance.player.transform;
    }

    void FixedUpdate()
    {
        float distance = Vector2.Distance(transform.position, target.position);

        if (distance < magnetRange) // 거리가 magnetRange보다 작을때만 자석효과 적용
        {
            Vector2 nextVec = (target.position - transform.position).normalized;
            rig.MovePosition(rig.position + nextVec * speed * Time.fixedDeltaTime);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어와 부딪히면 획득 처리
        if (collision.CompareTag("Player"))
        {
            gameObject.SetActive(false); //아이템 비활성화
        }
    }
}
