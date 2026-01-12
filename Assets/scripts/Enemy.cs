using Unity.Android.Gradle.Manifest;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    public float health;
    public float maxHealth;
    public RuntimeAnimatorController[] animCon;
    public Rigidbody2D target; //enemy들이 추적할 타겟
    bool isLive;
    Rigidbody2D rigid;
    SpriteRenderer spriter;
    Animator anim;

    void Awake() // Awake는 객체가 생성될때 실행됨
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void FixedUpdate() //물리적 이동이 아닌 
    {

        if (!isLive) return;

         Vector2 dirVec = target.position - rigid.position;
        //방향은 (타켓위치 - 나(적)의위치)를 정규화 한것임
        // 플레이어는 계속 방향키를 누를거고 target.position값은 계속 바뀜

        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        //프레임의 영향으로 결과가 달라지지 않도록 fixedDeltaTime사용

         rigid.MovePosition(rigid.position + nextVec);
        // MovePosition는 적이 겹치지 않고 모여있는 느낌과 물리적으로 이동하는 느낌을 줌

        rigid.linearVelocity = Vector2.zero;
    }

    void LateUpdate()
    {
        if(!isLive) return;
        //에니메이션 방향 전환 
        spriter .flipX = target.position.x > rigid.position.x;

        //Vector3 사용 이유는 오브젝트의 위치 호환성과 에러 방지를 위함
        Vector3 dist = (Vector3)target.position - transform.position;
        
        //플레이어가 화면 밖으로 벗어나 거리가 15f보다 멀어지면 리스폰
        if (Mathf.Abs(dist.x) > 15f || Mathf.Abs(dist.y) > 15f)
        {
            RepositionEnemy(dist);
        }
    }

    void RepositionEnemy(Vector3 dist)
    {
        // 플레이어가 가고있는 방향으로 적 생성
        float moveX = dist.x > 0 ? 40f : -40f; 
        float moveY = dist.y > 0 ? 40f : -40f;

        // 플레이어가 가로로 더 멀리 도망갔으면 그 방향으로 재배치
        if (Mathf.Abs(dist.x) > Mathf.Abs(dist.y)) {
            transform.position += Vector3.right * moveX;
        } else {
            transform.position += Vector3.up * moveY;
        }
        
        // 이동 후 튕김 방지
        rigid.linearVelocity = Vector2.zero;
    }

    void OnEnable() // 풀에서 꺼내져서 활성화될 때마다 실행됨
    {
        GameObject player = GameObject.FindWithTag("Player"); 
        if (player != null) {
            target = player.GetComponent<Rigidbody2D>();
        }
        isLive = true;
        health = maxHealth; //죽어서 리스폰된 enemy의 체력을 max로 초기화
        
    }
    
    public void Init(SpawnData data)
    {
        anim.runtimeAnimatorController = animCon[data.spriteType];
        speed = data.speed;
        maxHealth = data.health;
        health = data.health;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        //Bullet 태그를 단 오브젝트와 충동하지 않으면 그냥 return(enemy와 Bullet의 충돌여부 확인)
        if(!collision.CompareTag("Bullet"))
            return;
        //설정한 Bullet의 damage만큼을 enemy의 health에서 빼서 남은 hp계산
        health -= collision.GetComponent<Bullet>().damage;

        if(health > 0)
        {
            
        }
        else 
        {
            Dead();
        }
    }

    void Dead()
    {
        gameObject.SetActive(false); //죽은 enemy오브젝트 비활성화
    }
}
