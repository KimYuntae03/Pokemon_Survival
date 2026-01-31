using Unity.VisualScripting;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int id; //무기 자체의 종류
    public int prefabId;
    public float damage;
    public int count; //무기 개수
    public float speed;
    float timer;
    public int level; //무기레벨 저장
    public LayerMask targetLayer; //Enemy레이어를 체크할 변수
    Player_Controller player; //플레이어 입력 방향 가져오는 변수

    void Awake()
    {
        player = GetComponentInParent<Player_Controller>();
    }

    void Update()
    {
        switch (id)
        {
            case 0:
                transform.Rotate(Vector3.forward * speed * Time.deltaTime);
                break;
            case 1: // 진공파 (일정 시간마다 발사)
                timer += Time.deltaTime; // 발사 간격 타이머
                if (timer > speed) {
                    timer = 0f;
                    Fire(); //원거리 무기 발사함수
                }
                break;
            case 2:
                timer += Time.deltaTime;
                if(timer > speed)
                {
                    timer = 0f;
                    Fire();
                }
                break;
            case 4:
                timer += Time.deltaTime;
                if (timer > speed) { // 여기서 speed는 공격 쿨타임
                    timer = 0f;
                    Fire();
                }
                break;
        }
    }

    public void LevelUp(float addDamage, int addCount)
    {
        this.damage += 5;
        switch(id){
            case 0: //화염자동차
                this.count += 1;
                Batch();
                break;
            case 1: //파동탄
                this.count += 1;
                break;
            case 2:
                this.speed = Mathf.Max(this.speed - 0.5f, 0.2f); //쿨타임 감소
                break;
        }
    }

    public void Init(ItemData data)
    {
        id = data.itemId;
        damage = data.baseDamage;
        count = data.baseCount;
        level = 0; // 초기 레벨 설정
        this.speed = data.baseSpeed;
        transform.localPosition = Vector3.zero;

        if(id == 0) Batch();
    }

    void Fire()
    {   
        //생구 먹었을때 무기에 데미지 증가를 적용하기 위해 추가
        float finalDamage = damage * GameManager.instance.damageBuff;

        if(id == 2)
        {   
            Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, 3f, LayerMask.GetMask("Enemy"));
            if (targets.Length == 0) return; //범위내에 없으면 발동ㄴ

            if (GameManager.instance.scratchSfx != null) {
                GameManager.instance.scratchSfx.PlayOneShot(GameManager.instance.scratchSfx.clip);
            }

            // 적들 중 랜덤으로 하나 선택
            Transform target = targets[Random.Range(0, targets.Length)].transform;
            Vector3 targetPos = target.position;

            // 풀에서 할퀴기 이펙트(Bullet_Scratch)를 가져옴
            Transform bullet = GameManager.instance.pool.Get(prefabId).transform;
            bullet.position = targetPos; // 선택된 적의 위치에 생성
            
            bullet.GetComponent<Bullet>().Init(finalDamage, 0, Vector3.zero,id);
        }
        else if(id == 1){
            // 풀 매니저에서 진공파 프리팹을 가져온다
            Transform bullet = GameManager.instance.pool.Get(prefabId).transform;
            bullet.position = transform.position; // 플레이어 위치에서 발사

            // 플레이어가 이동 중인 방향(inputVec)을 가져옴 (없으면 위쪽으로)
            Vector3 dir = player.inputVec == Vector2.zero ? (Vector3)player.lastVec : (Vector3)player.inputVec.normalized;

            // Bullet 스크립트의 Init 호출 (damage, 관통력, 발사방향)
            bullet.GetComponent<Bullet>().Init(finalDamage, count, dir,id);
        }
        else if(id == 4)
        {
            Transform scaryFace = GameManager.instance.pool.Get(prefabId).transform;
            scaryFace.parent = player.transform;// 플레이어를 부모로 설정
            scaryFace.localPosition = Vector3.zero; //위치 초기화
            scaryFace.localRotation = Quaternion.identity;

            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemyObj in enemies) {
                if (!enemyObj.activeSelf) continue;

                Enemy enemy = enemyObj.GetComponent<Enemy>();
                if (enemy != null) {
                    enemy.ApplySlow(3.0f, 0.1f); // 3초간 10% 감소
                }
            }
        }
    }

    void Batch()
    {
        for(int index = 0; index < count; index++)
        {
            Transform bullet;
            //PoolManager에 등록했던 prefabId를 사용해서 무기를 꺼내옴
            
            if(index < transform.childCount) 
            {
                bullet = transform.GetChild(index);         
            }
            else
            {
                bullet = GameManager.instance.pool.Get(prefabId).transform;
                bullet.parent = transform;
            }

            // transform은 이 스크립트를 가지고 있는 오브젝트를 뜻함.
            // bullet은 pool에서 꺼내온 무기인거고 Weapon오브젝트에 이 스크립트가 들어있으므로
            // 결과적으로 꺼내온 무기의 부모는 Weapon오브젝트가 됨.
            
            bullet.localPosition = Vector3.zero;
            bullet.localRotation = Quaternion.identity;
            //풀에서 꺼낸 무기 위치 초기화

            Vector3 rotVec = Vector3.forward * 360 * index / count;
            bullet.Rotate(rotVec);
            bullet.Translate(bullet.up * 1.5f, Space.World);
            //무기가 추가됐을때 무기 간격 설정 코드

            bullet.GetComponent<Bullet>().Init(damage, -1,Vector3.zero,id);
            //근접 무기는 무조건 관통하므로 per값을 -1로 고정
        }
    }
}
