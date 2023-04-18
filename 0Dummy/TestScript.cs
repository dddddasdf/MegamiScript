using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ĳ�� ���� Ȯ�ο� ���� ��ũ��Ʈ

public class TestScript : MonoBehaviour
{
    public static TestScript Instance;

    [SerializeField] private SkillDatabaseManager SkillDBCaching = new SkillDatabaseManager();

    private void Awake()
    {
        if (null == Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SkillDBCaching.InitSkillDatabaseManager();


            return;
        }
        Destroy(gameObject);

    }

    public void TestFN()
    {
        Debug.Log("��ī����");

        SkillDBCaching.tkffuwnj();
    }


    public SkillDatabaseManager ReturnClass()
    {
        return SkillDBCaching;
    }
}
