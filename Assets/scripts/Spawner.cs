using UnityEngine;

public class Spawner : MonoBehaviour
{
    public SpawnData[] spawnData;
    public Transform[] spawnPoint;
    float timer;
    int level;
    bool isBossSpawned = false;
    public BossHealthBar bossHealthBar;

    void Update()
    {   
        if (!GameManager.instance.isPlayerLive) return;
        // 타이머 작동

        if (!isBossSpawned && GameManager.instance.gameTime >= 20f) { 
            isBossSpawned = true; 
            SpawnBoss(8); 
        }

        timer += Time.deltaTime;
        level = Mathf.Min(Mathf.FloorToInt(GameManager.instance.gameTime / 40f), spawnData.Length - 1);
        //ㄴ>Mathf 클래스의 FloorToInt로 정수변환 40초마다 1레벨 증가로 인식
        
        //level증가할 수록 생성주기가 짧아짐. ? 참일때 값 : 거짓일때 값
        if (timer > spawnData[level].spawnTime) {
            timer = 0;
            Spawn();
        }
    }

    void SpawnBoss(int index)
    {
        isBossSpawned = true;
        GameManager.instance.PlayBossBgm();
        GameObject enemy = GameManager.instance.pool.GetEnemy(index);

        enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;

        if (bossHealthBar != null) {
            bossHealthBar.gameObject.SetActive(true);
            enemy.GetComponent<Enemy>().bossHealthBar = bossHealthBar; 
            bossHealthBar.InitBossBar(enemy.transform, spawnData[index].health);
        }

        if (index < spawnData.Length) {
            enemy.GetComponent<Enemy>().Init(spawnData[index]);
        }
    }

    void Spawn()
    {   
        if (isBossSpawned) return;//보스몹 생성 후에는 생정 중지

        // PoolManager에서 몬스터 꺼내오기
        GameObject enemy = GameManager.instance.pool.GetEnemy(0);

        int selectedIndex = Random.Range(0, Mathf.Min(level + 1, 8));
        
        if (selectedIndex < level && Random.value > 0.5f) {
            selectedIndex = Mathf.Min(level, 7);
        }
        // 미리 만들어둔 소환 지점 중 랜덤하게 한 곳
        enemy.transform.position = spawnPoint[Random.Range(0, spawnPoint.Length)].position;
        enemy.GetComponent<Enemy>().Init(spawnData[selectedIndex]);
    }
}

//직렬화(Serialization) []를 앞에쓰면 배열이 아니라 속성의 의미도 됨. 
//유니티의 인스펙터 상에서 아래의 수치들을 바꾸기 위해 직렬화
[System.Serializable]
public class SpawnData
{
    public int spriteType;
    public float spawnTime;
    public int health;
    public float speed;
}
