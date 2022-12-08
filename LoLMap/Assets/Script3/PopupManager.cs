using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupManager : MonoBehaviour
{

    // 팝업창 
    public PopupUI _ShopPopup;
    public PopupUI _SystemPopup;

    //단축키 
    [Space]
    public KeyCode _escapeKey = KeyCode.Escape;
    public KeyCode _ShopKey = KeyCode.P;

    //실시간 팝업 관리 링크드 리스트
    private LinkedList<PopupUI> _activePopupList;

    //전체 팝업 목록
    private List<PopupUI> _allPopupList;



    private void Awake()
    {
        _activePopupList = new LinkedList<PopupUI>();
        Init();
        InitCloseAll();
    }

    private void Update()
    {
        //esc 키다운 리스트의 첫번째 닫기 
        if(Input.GetKeyDown(_escapeKey))
        {
            if(_activePopupList.Count > 0 )
            {
                ClosePopup(_activePopupList.First.Value);
            }
        }
        //단축키 조작
        ToggleKeyDownAction(_ShopKey, _ShopPopup); //P 누를시 상점 활성화 
    }

    private void Init()
    {
        //리스트 초기화
        _allPopupList = new List<PopupUI>()
        {
            _ShopPopup
        };

        //이벤트 등록
        foreach (var popup in _allPopupList)
        {
            //헤드 포커스 이벤트
            popup.OnFocus += () =>
            {
                _activePopupList.Remove(popup);
                _activePopupList.AddFirst(popup);
                RefreshAllPopupDepth();
            };

            //닫기 버튼 이벤트
            popup._closeButton.onClick.AddListener(() => ClosePopup(popup));
        }
    }


    //시작시 모든 팝업 닫기
    private void InitCloseAll()
    {
        foreach (var popup in _allPopupList)
        {
            ClosePopup(popup);
        }
    }

    //단축키 입력에 따라 팝업 열거나 닫기
    private void ToggleKeyDownAction(in KeyCode key , PopupUI popup)
    {
        if (Input.GetKeyDown(key))
            ToggleOpenClosePopup(popup);
    }



    //팝업의 상태에 따라 열거나 닫기
    private void ToggleOpenClosePopup(PopupUI popup)
    {
        if (!popup.gameObject.activeSelf) OpenPopup(popup);
        else ClosePopup(popup);
    }

    //팝업을 열고 링크드리스트의 상단에 추가
    private void OpenPopup(PopupUI popup)
    {
        _activePopupList.AddFirst(popup);
        popup.gameObject.SetActive(true);
        RefreshAllPopupDepth();
    }

    //팝업을 닫고 링크드리스트에서 제거
    private void ClosePopup(PopupUI popup)
    {
        _activePopupList.Remove(popup);
        popup.gameObject.SetActive(false);
        RefreshAllPopupDepth();
    }

    //링크드리스트 내 모든 팝업의 자식 순서 재배치
    private void RefreshAllPopupDepth()
    {
       foreach(var popup in _activePopupList)
        {
            popup.transform.SetAsFirstSibling();
        }
    }
}
