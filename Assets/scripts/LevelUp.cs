using UnityEngine;

public class LevelUp : MonoBehaviour
{
    public ItemData[] allItems; // 모든 아이템 데이터를 담는 배열
    public ItemUI[] itemSlots;  // 버튼 3개에 붙어있는 UI 업데이트용 스크립트
    public Player_Controller player;
    
    void Awake()
    {
        // 처음엔 비활성화
        gameObject.SetActive(false);
    }

    public void Show()
    {
        Time.timeScale = 0f; // 게임 일시정지
        gameObject.SetActive(true);

        // 랜덤하게 3개 뽑아서 버튼들에 데이터 전달
        for (int i = 0; i < itemSlots.Length; i++)
        {
            int ran = Random.Range(0, allItems.Length);
            itemSlots[i].Set(allItems[ran]);
        }
    }

    public void Hide()
    {
        Time.timeScale = 1f; // 게임 다시 시작
        gameObject.SetActive(false);
    }
}
