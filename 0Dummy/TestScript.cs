using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//캐싱 동작 확인용 더미 스크립트

public class TestScript : MonoBehaviour
{
    public static TestScript Instance { get; private set; }

    public SkillDatabaseManager SkillDBCaching;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            SkillDBCaching = new SkillDatabaseManager();

            return;
        }
        Destroy(gameObject);
    }

    
}
