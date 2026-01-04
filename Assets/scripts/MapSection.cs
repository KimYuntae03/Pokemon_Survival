using UnityEngine;

public class MapSection : MonoBehaviour
{
    public GameObject[] decoPrefabs; // 잔디, 꽃 등등
    public int decoCount = 20;
    void Start()
    {

    }

    void Update()
    {
        
    }
    void OnEnable() 
    {
        // 기존 장식 삭제 
        foreach (Transform child in transform) {
            if (child.CompareTag("Decoration")) Destroy(child.gameObject);
        }
        SpawnDecorations();
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
}
