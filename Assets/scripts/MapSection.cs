using UnityEngine;

public class MapSection : MonoBehaviour
{
    public GameObject[] decoPrefabs; // 잔디, 꽃 등등
    public int decoCount = 20;

    [Header("Item Settings")]
    [Range(0, 100)] public float berrySpawnChance = 10f;
    [Range(0, 100)] public float magnetSpawnChance = 5f;

    void OnEnable() 
    {
        // 기존 장식 삭제 
        foreach (Transform child in transform) {
            if (child.CompareTag("Decoration")) Destroy(child.gameObject);
            if (child.CompareTag("Item")) child.gameObject.SetActive(false);
        }
        SpawnDecorations();
        SpawnBerry();
        SpawnMagnet();
    }

    void SpawnDecorations()
    {
        for (int i = 0; i < decoCount; i++)
        {
            //맵 크기맞춰서 장식물 핸덤 생성
            Vector3 spawnPos = new Vector3(Random.Range(-9.5f, 9.5f), Random.Range(-9.5f, 9.5f), 0);
            int randomIndex = Random.Range(0, decoPrefabs.Length);
            GameObject deco = Instantiate(decoPrefabs[randomIndex], transform);
            deco.transform.localPosition = spawnPos;
            deco.tag = "Decoration"; // 태그 설정
        }
    }

    void SpawnBerry()
    {
        if (Random.Range(0f, 100f) > berrySpawnChance) return;

        GameObject berry = GameManager.instance.pool.GetWeapon(3);
        
        berry.transform.SetParent(transform);

        Vector3 spawnPos = new Vector3(Random.Range(-9.5f, 9.5f), Random.Range(-9.5f, 9.5f), 0);
        berry.transform.localPosition = spawnPos;
    }

    void SpawnMagnet()
    {
        if (Random.Range(0f, 100f) > magnetSpawnChance) return;

        GameObject magnet = GameManager.instance.pool.GetWeapon(4); 
        
        magnet.transform.SetParent(transform);

        Vector3 spawnPos = new Vector3(Random.Range(-9.5f, 9.5f), Random.Range(-9.5f, 9.5f), 0);
        magnet.transform.localPosition = spawnPos;
    }
}
