using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ItemUI : MonoBehaviour, ISelectHandler
{
    public Image icon;       // 아이템 아이콘 연결용
    public TMP_Text textName;    // 아이템 이름 텍스트 연결용
    // public TMP_Text textDesc;    // 아이템 설명 텍스트 연결용

    ItemData data;           // 현재 슬롯이 배정받은 데이터
    int level;

    // LevelUp 스크립트에서 호출하여 슬롯 정보를 업데이트하는 함수
    public void Set(ItemData data)
    {
        this.data = data;

        icon.sprite = data.itemIcon;
        textName.text = data.itemName;
        // textDesc.text = data.itemDesc;
    }

    public void OnSelect(BaseEventData eventData)
    {
        // LevelUp 스크립트에 있는 '공용 설명창'에 현재 아이템 설명을 전달
        GetComponentInParent<LevelUp>().UpdateDescription(data.itemDesc);
    }

    // 버튼이 클릭되었을 때 실행될 함수
    public void OnClick()
    {
        Debug.Log("버튼이 클릭되었습니다!");

        //플레이어 참조를 가져옴
        Player_Controller player = GetComponentInParent<LevelUp>().player;

        switch (data.itemType) {
            case ItemData.ItemType.Melee: // 근거리 무기
            case ItemData.ItemType.Range: // 원거리 무기
                HandleWeaponUpgrade(player);
                break;

            case ItemData.ItemType.buff:
                HandleBuffItem(player);
                break;

            case ItemData.ItemType.Heal:
                // 추후 추가
                break;
        }
        GetComponentInParent<LevelUp>().Hide(); //레벨업 창 닫기
    }

    //무기강화 함수
    void HandleWeaponUpgrade(Player_Controller player)
    {
        Weapon[] weapons = player.GetComponentsInChildren<Weapon>(true);
        bool isExist = false;

        foreach (Weapon weapon in weapons) {
            if (weapon.id == data.itemId) {
                isExist = true;
                if (weapon.gameObject.activeSelf) {
                    weapon.LevelUp(data.damages[weapon.level], data.counts[weapon.level]);
                    weapon.level++;
                } 
                else { //스킬이 첫 선택일 경우
                    weapon.gameObject.SetActive(true);
                    weapon.Init(data); // 기본 데미지, 개수 등 초기화
                    weapon.level++;
                }
                break;
            }
        }

        if (!isExist) {
            // [새 무기 생성 코드...]
        }
    }

    void HandleBuffItem(Player_Controller player)
    {
        switch (data.itemId) {
            case 2: // 알카로이드 (이동속도 증가)
                player.moveSpeed += 0.5f; // 혹은 data.baseDamage 수치를 활용
                Debug.Log("이동속도가 증가했습니다!");
                break;
            case 3: // 공격력 증가 포션 등
                // player.power += 1; 
                break;
        }
    }
    public string GetItemDescription()
    {
        return data != null ? data.itemDesc : "";
    }
}
