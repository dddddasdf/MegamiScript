using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuScript : MonoBehaviour
{
    #region SetVariables

    [SerializeField] private Canvas MenuCanvas;
    [SerializeField] private List<Button> ButtonList;
    private int NowSelectedButtonIndex;  //���� ���� ���� ��ų ��ư �ε���: Ű���� �Է¿� �� �����

    //���õ� ��ư ���� ����
    private Button NowSelectedButton;   //���õ� ��ư �����
    private Color SelectedButtonColor; //���õ� ��ų ��ư ����+����
    private Color UnselectedButtonColor;   //���õ��� ���� ��ų ��ư ����+����

    #endregion

    public void ShowMenuCanvas()
    {
        MenuCanvas.enabled = true;
    }

    public void HideMenuCanvas()
    {
        MenuCanvas.enabled = false;
    }
    
}
