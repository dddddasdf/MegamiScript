using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ĳ�� ���� Ȯ�ο� ���� ��ũ��Ʈ

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