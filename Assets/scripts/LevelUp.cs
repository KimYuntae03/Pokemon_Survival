using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;

public class LevelUp : MonoBehaviour
{
    public ItemData[] allItems; // 모든 아이템 데이터를 담는 배열
    public ItemUI[] itemSlots;  // 버튼 3개에 붙어있는 UI 업데이트용 스크립트
    public Player_Controller player;
    public TMP_Text descriptionText;
    public GameObject contentPanel;
    public GameObject selectionPointer;
    public RectTransform uiJoystick;
    public GameObject pauseButton;

    void Awake()
    {
        // 처음엔 비활성화
        if (contentPanel != null)
            contentPanel.SetActive(false);
    }

    public void Show()
    {
        if (uiJoystick != null) {
            uiJoystick.localScale = Vector3.zero;
        }
        if (pauseButton != null) pauseButton.SetActive(false);
        Time.timeScale = 0f; // 게임 일시정지
        if (contentPanel != null)
            contentPanel.SetActive(true);
        
        List<ItemData> availableItems = new List<ItemData>();
        Weapon[] currentWeapons = player.GetComponentsInChildren<Weapon>(true);

        foreach (ItemData item in allItems) {
            Weapon targetWeapon = currentWeapons.FirstOrDefault(w => w.id == item.itemId);
            
            if (targetWeapon == null || targetWeapon.level < 5) {
                availableItems.Add(item);
            }
        }
        List<int> ranList = new List<int>();
        int count = Mathf.Min(itemSlots.Length, availableItems.Count);

        // 랜덤하게 3개 뽑아서 버튼들에 데이터 전달
        for (int i = 0; i < itemSlots.Length; i++){
                // 일단 모든 슬롯을 끄고 시작 (아이템이 부족할 때 대비)
                itemSlots[i].gameObject.SetActive(false);
            }

        for (int i = 0; i < count; i++)
        {
            int ran;
            // 중복되지 않는 인덱스가 나올 때까지 반복
            do {
                ran = Random.Range(0, allItems.Length);
            } while (ranList.Contains(ran));
                
            ranList.Add(ran);
            itemSlots[i].Set(allItems[ran]);
            itemSlots[i].gameObject.SetActive(true); 
        }

        if (count > 0)
        {
            EventSystem.current.SetSelectedGameObject(itemSlots[0].gameObject);
            itemSlots[0].isDescriptionActive = true;
            
            if (selectionPointer != null)
            {
                selectionPointer.SetActive(true);
                
                // 포인터의 부모를 첫 번째 슬롯으로 변경
                selectionPointer.transform.SetParent(itemSlots[0].transform);

                RectTransform pointerRect = selectionPointer.GetComponent<RectTransform>();
                pointerRect.anchoredPosition = new Vector2(-70f, 20f); 
                pointerRect.localScale = Vector3.one;
                pointerRect.localRotation = Quaternion.identity;
            }
            UpdateDescription(itemSlots[0].GetItemDescription());
        }

        itemSlots[0].GetComponent<Button>().Select();
    }

    public void UpdateDescription(string desc)
    {
        if (descriptionText != null)
            descriptionText.text = desc;
    }

    public void Hide()
    {   
        if (uiJoystick != null) {
            uiJoystick.localScale = new Vector3(0.8f, 0.8f, 1f);
        }
        if (pauseButton != null) pauseButton.SetActive(true);
        Time.timeScale = 1f; // 게임 다시 시작
        if (player != null) {
            player.ResetInput(); 
        }
        if (selectionPointer != null)
            selectionPointer.SetActive(false);
        if (contentPanel != null)
            contentPanel.SetActive(false);
    }

}
