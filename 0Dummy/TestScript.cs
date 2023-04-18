using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//캐싱 동작 확인용 더미 스크립트

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
        Debug.Log("과카몰리");

        SkillDBCaching.tkffuwnj();
    }


    public SkillDatabaseManager ReturnClass()
    {
        return SkillDBCaching;
    }
}
