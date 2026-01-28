using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ItemUI : MonoBehaviour, ISelectHandler
{
    public Image icon;       // 아이템 아이콘 연결용
    public TMP_Text textName;    // 아이템 이름 텍스트 연결용
    // public TMP_Text textDesc;    // 아이템 설명 텍스트 연결용
    public RectTransform pointer;


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
        Debug.Log(gameObject.name + " 선택됨");
        // LevelUp 스크립트에 있는 '공용 설명창'에 현재 아이템 설명을 전달
        GetComponentInParent<LevelUp>().UpdateDescription(data.itemDesc);

        //화살표 아이콘 이동
        LevelUp levelUpScript = GetComponentInParent<LevelUp>();

        if (levelUpScript.selectionPointer != null)
            {
                GameObject pointer = levelUpScript.selectionPointer;
                pointer.SetActive(true);

                pointer.transform.SetParent(this.transform);
                RectTransform pointerRect = pointer.GetComponent<RectTransform>();
                
                pointerRect.anchoredPosition = new Vector2(-70f, 20f);
                pointerRect.localScale = Vector3.one;
                pointerRect.localRotation = Quaternion.identity;
            }
    }

    // 버튼이 클릭되었을 때 실행될 함수
    public void OnClick()
    {
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
                    if (weapon.level < data.damages.Length && weapon.level < data.counts.Length) {
                        weapon.LevelUp(data.damages[weapon.level], data.counts[weapon.level]);
                }   else {
                        // 만렙일 경우에 대한 예외 처리
                        Debug.LogWarning($"{data.itemName}은 이미 최대 레벨({weapon.level})입니다!");
                    }
                } 
                else { //스킬이 첫 선택일 경우
                    weapon.gameObject.SetActive(true);
                    weapon.transform.localPosition = Vector3.zero;
                    weapon.Init(data); // 기본 데미지, 개수 등 초기화
                }
                break;
            }
        }

        if (!isExist) {//무기를 처음 선택할 때
            if (data.itemPrefab != null){ //프리팹이 있는지 체크
                    //프리팹 생성 및 플레이어의 자식으로 설정
                    GameObject newWeapon = Instantiate(data.itemPrefab, player.transform);
                    newWeapon.transform.localPosition = Vector3.zero;
                    Weapon weaponScript = newWeapon.GetComponent<Weapon>();
                
                    if (weaponScript != null) {
                        weaponScript.Init(data); //데이터 전달해서 초기화
                    }
            }          
        }
    }

    void HandleBuffItem(Player_Controller player)
    {
        switch (data.itemId) {
            case 2: // 알카로이드 (이동속도 증가)
                player.moveSpeed += 0.5f; // 혹은 data.baseDamage 수치를 활용
                break;
            case 3: // 행복의 알 (경험치 획등량 증가)
                GameManager.instance.expMultiplier += 0.2f;
                Debug.Log($"<color=yellow>행복의 알 획득!</color> 현재 배율: {GameManager.instance.expMultiplier}");
                break;
            case 5: // [추가] 생명의 구슬 (피해량 10% 증가)
                GameManager.instance.ApplyDamageBuff(0.2f);
                Debug.Log($"<color=red>생명의 구슬 획득!</color> 현재 공격력 버프: {GameManager.instance.damageBuff}");
                break;
        }
    }
    public string GetItemDescription()
    {
        return data != null ? data.itemDesc : "";
    }
}
