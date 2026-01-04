using UnityEngine;

public class Mapmanager : MonoBehaviour
{
    public GameObject[] mapPrefabs;
    public Transform player;       
    private GameObject[] spawnedMaps = new GameObject[9];

    public float tileSize = 20f;

    void Start()
    {
        int index = 0;
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector3 spawnPos = new Vector3(x * tileSize, y * tileSize, 0);
                spawnedMaps[index] = Instantiate(mapPrefabs[0], spawnPos, Quaternion.identity);
                index++;
            }
        }
    }

    void Update()
    {
        foreach (GameObject map in spawnedMaps)
        {
            Reposition(map);
        }
    }
    void Reposition(GameObject map)
    {
        //플레이어와 맵 타일 사이의 거리 차이 계산
        Vector3 dist = player.position - map.transform.position;

        //X축 거리 체크: 플레이어가 맵 범위를 벗어나면 반대편으로 이동
        if (Mathf.Abs(dist.x) > tileSize * 1.5f)
        {
            float moveDirX = dist.x > 0 ? 1 : -1;
            map.transform.position += Vector3.right * moveDirX * tileSize * 3f;
        }

        //Y축 거리 체크
        if (Mathf.Abs(dist.y) > tileSize * 1.5f)
        {
            float moveDirY = dist.y > 0 ? 1 : -1;
            map.transform.position += Vector3.up * moveDirY * tileSize * 3f;
        }
    }
}
