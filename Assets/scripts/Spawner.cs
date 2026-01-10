using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform[] spawnPoint;
    float timer;

    void Update()
    {
        // 타이머 작동
        timer += Time.deltaTime;

        // 소환 주기 결정
        if (timer > 0.5f) {
            timer = 0;
            Spawn();
        }
    }
    void Spawn()
    {
        // PoolManager에서 몬스터 꺼내오기
        GameObject enemy = GameManager.instance.pool.Get(Random.Range(0, GameManager.instance.pool.prefabs.Length));

        // 미리 만들어둔 소환 지점 중 랜덤하게 한 곳
        enemy.transform.position = spawnPoint[Random.Range(0, spawnPoint.Length)].position;
    }
}
