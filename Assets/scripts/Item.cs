using UnityEngine;

public class Item : MonoBehaviour
{
    public float speed = 5f; // 보석이 날아오는 속도
    public float magnetRange = 2.0f;
    bool isChasing = false; //보석이 플레이어를 추적중인지 확인하는 변수
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
        isChasing = false; //EXP보석의 추적상태 초기화 
    }

    void FixedUpdate()
    {
        float distance = Vector2.Distance(transform.position, target.position);

        if (isChasing || distance < magnetRange) 
        {//ㄴ>Exp보석이 플레이어를 추적 시작했거나 거리가 범위 안이라면
            isChasing = true; // 추적 상태를 true로 고정

            Vector2 nextVec = (target.position - transform.position).normalized;
            rig.MovePosition(rig.position + nextVec * speed * Time.fixedDeltaTime);
            
            // 플레이어와 충분히 가까워지면 비활성화 
            if (distance < 0.2f) 
            {
                gameObject.SetActive(false);
            }
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
