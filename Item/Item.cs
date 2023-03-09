using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gpm.Ui;

/// <summary>
/// �Ҹ��� ����ü
/// </summary>
[System.Serializable]
public class UsableItemInformaiton : InfiniteScrollData
{
    public string Name = string.Empty;
    public int NumberOfNowPossess = 0;
    public int NumberOfMax = 0;
}

[System.Serializable]
public class Item : MonoBehaviour
{
    
    public enum UseType
    {
        Recovery,   //ȸ��
        Combat, //����&���
        Incense //����
    }
}
