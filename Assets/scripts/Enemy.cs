using Unity.Android.Gradle.Manifest;
using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public float speed;
    float defaultSpeed; //원래속도 기억할 변수
    public float health;
    public float maxHealth;
    public RuntimeAnimatorController[] animCon;
    public Rigidbody2D target; //enemy들이 추적할 타겟
    bool isLive;
    public int expPrefabId = 2;
    Rigidbody2D rigid;
    SpriteRenderer spriter;
    Animator anim;

    enemy_Hit hitScript; //enemy_Hit참조(피격시 넉백 적용하려고)
    
    void Awake() // Awake는 객체가 생성될때 실행됨
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        hitScript = GetComponent<enemy_Hit>();
        defaultSpeed = speed;
    }
    

    void FixedUpdate()
    {
        if (target == null) {
            GameObject player = GameObject.FindWithTag("Player");
        if (player != null) target = player.GetComponent<Rigidbody2D>();
        }

        if (!isLive || (hitScript != null && hitScript.isKnockback)) return;

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
            if (hitScript != null) { 
                hitScript.OnHit();
            }
        }
        else 
        {   
            isLive = false;
            Dead();
        }
    }

    void Dead()
    {   
        GameManager.instance.GetKill(); //killcount 상승
        GameObject exp = GameManager.instance.pool.Get(expPrefabId);//풀 메니저에서 Exp가져옴
        exp.transform.position = transform.position;//보석 위치를 현재 죽은 몬스터의 위치로 설정
        gameObject.SetActive(false); //죽은 enemy오브젝트 비활성화
        
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        //충돌한 적이 Player인지 확인
        if (collision.gameObject.CompareTag("Player")) {

            //플레이어 컨트롤러 컴포넌트를 가져옴
            Player_Controller player = collision.gameObject.GetComponent<Player_Controller>();
            
            if (player != null) {
                player.TakeDamage(20f * Time.deltaTime); //초당 20의 데미지를 줌
            }
        }
    }

    public void ApplySlow(float duration, float rate) { //겁나는 얼굴
        StopCoroutine("SlowRoutine"); // 이미 느려진 상태라면 초기화 후 재시작
        StartCoroutine(SlowRoutine(duration, rate));
    }

    IEnumerator SlowRoutine(float duration, float rate) {
        speed = defaultSpeed * (1f - rate); // 20% 감소면 rate = 0.2f
        yield return new WaitForSeconds(duration);
        speed = defaultSpeed; // 원래대로 복구
    }
}
