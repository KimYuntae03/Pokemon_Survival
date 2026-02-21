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
    public BossHealthBar bossHealthBar;
    public Transform shadow;
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
            if (GameManager.instance != null && GameManager.instance.player != null) {
                target = GameManager.instance.player.GetComponent<Rigidbody2D>();
            }
            if (target == null) return; // 여전히 없으면 리턴
        }

        if (!isLive || (hitScript != null && hitScript.isKnockback)) return;

        // float dist = Vector2.Distance(target.position, rigid.position);
        // if (dist < 0.01f) return;
        
         Vector2 dirVec = target.position - rigid.position;
        //방향은 (타켓위치 - 나(적)의위치)를 정규화 한것임
        // 플레이어는 계속 방향키를 누를거고 target.position값은 계속 바뀜

        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        //프레임의 영향으로 결과가 달라지지 않도록 fixedDeltaTime사용

         rigid.MovePosition(rigid.position + nextVec);
        // MovePosition는 적이 겹치지 않고 모여있는 느낌과 물리적으로 이동하는 느낌을 줌

    }

    void LateUpdate()
    {
        if(!isLive) return;
        //에니메이션 방향 전환 
        bool isGiratina = anim.runtimeAnimatorController == animCon[8];

        if (isGiratina) //기라티나 특별 애니메이션 처리
        {
            Vector2 dir = (target.position - rigid.position).normalized;
            
            anim.SetFloat("DirX", dir.x);
            anim.SetFloat("DirY", dir.y);

            spriter.flipX = false; 
        }
        else //일반몹
        {
            spriter.flipX = target.position.x > rigid.position.x;
        }
        //Vector3 사용 이유는 오브젝트의 위치 호환성과 에러 방지를 위함
        Vector3 dist = (Vector3)target.position - transform.position;
        
        //플레이어가 화면 밖으로 벗어나 거리가 15f보다 멀어지면 리스폰
        if (!isGiratina && (Mathf.Abs(dist.x) > 22f || Mathf.Abs(dist.y) > 18f))
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
        
    }

    void OnEnable() // 풀에서 꺼내져서 활성화될 때마다 실행됨
    {
        if (GameManager.instance != null && GameManager.instance.player != null) {
            target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        }

        anim.transform.localPosition = Vector3.zero; //전에 처치했던 기라티나 위치 초기화
        isLive = true;
        health = maxHealth; //죽어서 리스폰된 enemy의 체력을 max로 초기화
        rigid.simulated = true;//전에 처치했던 기라티나 물리엔진 켜기
        GetComponent<Collider2D>().enabled = true;

        Transform mask = transform.Find("GiraMask");

        if (mask != null) mask.gameObject.SetActive(false);

        SpriteRenderer spr = GetComponent<SpriteRenderer>();
        if (spr != null) spr.maskInteraction = SpriteMaskInteraction.None;

        anim.transform.localPosition = Vector3.zero;
    }
    
    public void Init(SpawnData data)
    {
        anim.runtimeAnimatorController = animCon[data.spriteType];
        speed = data.speed;
        maxHealth = data.health;
        health = data.health;

        bool isGiratina = data.spriteType == 8;
        if (bossHealthBar != null) {
            bossHealthBar.gameObject.SetActive(isGiratina);
            if (isGiratina) {
                bossHealthBar.InitBossBar(transform,maxHealth);
            }
        }

        if (shadow != null)
        {
            // 기본값 초기화
            shadow.localPosition = new Vector3(0, -0.45f, 0); 
            shadow.localScale = Vector3.one;

            switch (data.spriteType)
            {
                case 2: //아보
                    shadow.localPosition = new Vector3(0, -0.6f, 0); 
                    shadow.localScale = new Vector3(1.3f, 1.2f, 1f); 
                    break;

                case 4: //레트라
                    shadow.localPosition = new Vector3(0, -0.45f, 0);
                    shadow.localScale = new Vector3(1.3f, 1.2f, 1f); 
                    break;

                case 5: // 동탁군 
                    shadow.localPosition = new Vector3(0, -0.75f, 0); 
                    shadow.localScale = new Vector3(1.5f, 1.0f, 1f); 
                    break;
                case 6: //골벳
                    shadow.localPosition = new Vector3(0, -0.75f, 0); 
                    shadow.localScale = new Vector3(1.5f, 1.0f, 1f); 
                    break;
                case 7: //헬가
                    shadow.localPosition = new Vector3(0, -0.75f, 0); 
                    shadow.localScale = new Vector3(1.5f, 1.0f, 1f); 
                    break;
                case 8: // 기라티나
                    shadow.localPosition = new Vector3(0, -1.2f, 0); 
                    shadow.localScale = new Vector3(3.0f, 2.2f, 1f);
                    break;

                default: // 기타 기본 몹 
                    shadow.localPosition = new Vector3(0, -0.45f, 0);
                    shadow.localScale = new Vector3(1f, 1.2f, 1f);
                    break;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        //Bullet 태그를 단 오브젝트와 충동하지 않으면 그냥 return(enemy와 Bullet의 충돌여부 확인)
        if(!collision.CompareTag("Bullet"))
            return;

        if (!isLive) return;//이미 죽은 적이면 피격처리 X
        
        //설정한 Bullet의 damage만큼을 enemy의 health에서 빼서 남은 hp계산
        float dmg = collision.GetComponent<Bullet>().damage;
        health -= dmg;
        bool isGiratina = anim.runtimeAnimatorController == animCon[8];
        if (isGiratina && bossHealthBar != null) {
            bossHealthBar.UpdateHealthBar(health, maxHealth);
        }

        if (hitScript != null)  hitScript.OnHit(dmg);
               
        if(health <= 0)
        {   
            isLive = false;
            if (isGiratina && bossHealthBar != null) {
                bossHealthBar.Hide(); 
            }
            Dead();
        }
    }

    void OnDisable() {
        bool isGiratina = anim.runtimeAnimatorController == animCon[8];
    
        if (bossHealthBar != null && !isGiratina) {
            bossHealthBar.gameObject.SetActive(false);
        }
    }

    void Dead()
    {   
        isLive = false;
        GameManager.instance.GetKill(); //killcount 상승

        rigid.simulated = false;
        GetComponent<Collider2D>().enabled = false; //물리 효과 차단

        bool isGiratina = anim.runtimeAnimatorController == animCon[8];

        if (isGiratina) 
        {
            if (shadow != null) shadow.gameObject.SetActive(false);
            StartCoroutine(SinkingRoutine());
        }
        else
        {
            GameObject exp = GameManager.instance.pool.GetWeapon(expPrefabId);//풀 메니저에서 Exp가져옴
            exp.transform.position = transform.position;
            gameObject.SetActive(false); //죽은 enemy오브젝트 비활성화
        }
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

    IEnumerator SinkingRoutine()//기라티나 기절 코루틴
    {   
        GameManager.instance.PlayBossDeathSfx();
        
        float duration = 1.5f;
        float timer = 0f;
        
        Transform visualTransform = anim.transform; 
        Vector3 startPos = visualTransform.localPosition;

        Transform mask = transform.Find("GiraMask");
        if (mask != null) {
            mask.gameObject.SetActive(true);
            mask.parent = null;
        }
        SpriteRenderer spr = GetComponent<SpriteRenderer>();
        if (spr != null) spr.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = timer / duration;
            
            visualTransform.localPosition = startPos + Vector3.down * (progress * 4.0f);

            yield return null;
        }
        if (mask != null) {
            mask.parent = transform; //다음 소환을 위해 다시 mask의 부모로 설정
            mask.gameObject.SetActive(false);
        }

        GameManager.instance.GameClear();
        gameObject.SetActive(false);
    }
}
