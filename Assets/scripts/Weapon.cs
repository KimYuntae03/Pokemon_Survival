using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int id; //무기 자체의 종류
    public int prefabId;
    public float damage;
    public int count; //무기 개수
    public float speed;

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
            default:
                break;
        }
        if(Input.GetButtonDown("Jump"))
            LevelUp();
    }

    public void LevelUp()
    {
        this.damage += 5;
        this.count += 1;
        if(id == 0)
        {
            Batch();
        }
    }

    public void Init()
    {
        switch (id)
        {
            case 0:
                Batch();
                break;
            default:
                break;
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

            bullet.GetComponent<Bullet>().Init(damage, -1);
            //근접 무기는 무조건 관통하므로 per값을 -1로 고정
        }
    }
}
