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
    Player_Controller player; //플레이어 입력 방향 가져오는 변수

    void Awake()
    {
        player = GetComponentInParent<Player_Controller>();
    }

    void Start()
    {
        Init();
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
        }
        if(Input.GetButtonDown("Jump")) //스페이스바를 눌렀을때 레벨업이 되도록 일단 테스트 용
            LevelUp();
    }

    public void LevelUp()
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
       }
    }

    public void Init()
    {
        if(id == 0) Batch();
    }

    void Fire()
    {
        // 풀 매니저에서 진공파 프리팹을 가져온다
        Transform bullet = GameManager.instance.pool.Get(prefabId).transform;
        bullet.position = transform.position; // 플레이어 위치에서 발사

        // 플레이어가 이동 중인 방향(inputVec)을 가져옴 (없으면 위쪽으로)
        Vector3 dir = player.inputVec == Vector2.zero ? Vector3.up : (Vector3)player.inputVec.normalized;

        // Bullet 스크립트의 Init 호출 (damage, 관통력, 발사방향)
        bullet.GetComponent<Bullet>().Init(damage, count, dir);
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

            bullet.GetComponent<Bullet>().Init(damage, -1,Vector3.zero);
            //근접 무기는 무조건 관통하므로 per값을 -1로 고정
        }
    }
}
