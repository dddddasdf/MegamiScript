using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gpm.Ui;

/// <summary>
/// 소모템 구조체
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
        Recovery,   //회복
        Combat, //공격&방어
        Incense //버프
    }
}
