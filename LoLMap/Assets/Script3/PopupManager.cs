using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupManager : MonoBehaviour
{

    // �˾�â 
    public PopupUI _ShopPopup;
    public PopupUI _SystemPopup;

    //����Ű 
    [Space]
    public KeyCode _escapeKey = KeyCode.Escape;
    public KeyCode _ShopKey = KeyCode.P;

    //�ǽð� �˾� ���� ��ũ�� ����Ʈ
    private LinkedList<PopupUI> _activePopupList;

    //��ü �˾� ���
    private List<PopupUI> _allPopupList;



    private void Awake()
    {
        _activePopupList = new LinkedList<PopupUI>();
        Init();
        InitCloseAll();
    }

    private void Update()
    {
        //esc Ű�ٿ� ����Ʈ�� ù��° �ݱ� 
        if(Input.GetKeyDown(_escapeKey))
        {
            if(_activePopupList.Count > 0 )
            {
                ClosePopup(_activePopupList.First.Value);
            }
        }
        //����Ű ����
        ToggleKeyDownAction(_ShopKey, _ShopPopup); //P ������ ���� Ȱ��ȭ 
    }

    private void Init()
    {
        //����Ʈ �ʱ�ȭ
        _allPopupList = new List<PopupUI>()
        {
            _ShopPopup
        };

        //�̺�Ʈ ���
        foreach (var popup in _allPopupList)
        {
            //��� ��Ŀ�� �̺�Ʈ
            popup.OnFocus += () =>
            {
                _activePopupList.Remove(popup);
                _activePopupList.AddFirst(popup);
                RefreshAllPopupDepth();
            };

            //�ݱ� ��ư �̺�Ʈ
            popup._closeButton.onClick.AddListener(() => ClosePopup(popup));
        }
    }


    //���۽� ��� �˾� �ݱ�
    private void InitCloseAll()
    {
        foreach (var popup in _allPopupList)
        {
            ClosePopup(popup);
        }
    }

    //����Ű �Է¿� ���� �˾� ���ų� �ݱ�
    private void ToggleKeyDownAction(in KeyCode key , PopupUI popup)
    {
        if (Input.GetKeyDown(key))
            ToggleOpenClosePopup(popup);
    }



    //�˾��� ���¿� ���� ���ų� �ݱ�
    private void ToggleOpenClosePopup(PopupUI popup)
    {
        if (!popup.gameObject.activeSelf) OpenPopup(popup);
        else ClosePopup(popup);
    }

    //�˾��� ���� ��ũ�帮��Ʈ�� ��ܿ� �߰�
    private void OpenPopup(PopupUI popup)
    {
        _activePopupList.AddFirst(popup);
        popup.gameObject.SetActive(true);
        RefreshAllPopupDepth();
    }

    //�˾��� �ݰ� ��ũ�帮��Ʈ���� ����
    private void ClosePopup(PopupUI popup)
    {
        _activePopupList.Remove(popup);
        popup.gameObject.SetActive(false);
        RefreshAllPopupDepth();
    }

    //��ũ�帮��Ʈ �� ��� �˾��� �ڽ� ���� ���ġ
    private void RefreshAllPopupDepth()
    {
       foreach(var popup in _activePopupList)
        {
            popup.transform.SetAsFirstSibling();
        }
    }
}
