using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public GameObject[] prefabs;
    
    //프리팹에 있는 요소를 담을 리스트 주머니(꼬렛,주벳 등등)
    List<GameObject>[] pools;
    
    void Awake()
    {
        // 프리팹 종류만큼 리스트 배열 초기화
        pools = new List<GameObject>[prefabs.Length];

        //enemy종류 수 만큼 돌면서 enemy를 담을 리스트 추가
        for (int i = 0; i < pools.Length; i++) {
            pools[i] = new List<GameObject>();
        }
    }

    public GameObject Get(int index)
    {
        GameObject select = null;

        // 선택한 풀의 리스트
        foreach (GameObject item in pools[index]) {
            if (!item.activeSelf) { //죽어서 pools[index]에 들어간 몬스터가 있으면
                select = item; 
                select.SetActive(true); //select에 담고 화면에 생성
                break;
            }
        }

        
        if (!select) { //재활용할 몬스터가 없으면
            select = Instantiate(prefabs[index], transform); //새로 하나 들어서 생성
            pools[index].Add(select); //새로 만든 애가 죽으면 다시 사용할 수 있도록 pools[index]에 등록
        }

        return select;
    }

}
