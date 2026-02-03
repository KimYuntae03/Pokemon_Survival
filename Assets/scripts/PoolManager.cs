using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public GameObject[] enemyPrefabs;  // 적 프리팹
    public GameObject[] weaponPrefabs; // 무기 프리팹
    
    //프리팹에 있는 요소를 담을 리스트 주머니(꼬렛,주벳 등등)
    List<GameObject>[] enemyPools;
    List<GameObject>[] weaponPools;
    
    void Awake()
    {
        //enemy 풀 초기화
        enemyPools = new List<GameObject>[enemyPrefabs.Length];
        for (int i = 0; i < enemyPools.Length; i++) enemyPools[i] = new List<GameObject>();
        //weapon 풀 초기화
        weaponPools = new List<GameObject>[weaponPrefabs.Length];
        for (int i = 0; i < weaponPools.Length; i++) weaponPools[i] = new List<GameObject>();
    }

    public GameObject GetEnemy(int index) {
        return GetFromPool(enemyPools, enemyPrefabs, index);
    }

    public GameObject GetWeapon(int index) {
        return GetFromPool(weaponPools, weaponPrefabs, index);
    }

    GameObject GetFromPool(List<GameObject>[] pools, GameObject[] prefabs, int index) {
        GameObject select = null;
        foreach (GameObject item in pools[index]) {
            if (!item.activeSelf) {
                select = item;
                select.SetActive(true);
                break;
            }
        }
        if (!select) {
            select = Instantiate(prefabs[index], transform);
            pools[index].Add(select);
        }
        return select;
    }

}
