using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using LitJson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class CharacterClassManager : MonoBehaviour
{
    public class characterClass
    {
        public CharacterClass Class;
        public bool Unlock;

        public void setClass(CharacterClass characterClass)
        {
            this.Class = characterClass;
        }
    }


    public class characterCalssData
    {
        public List<characterClass> characterClasses = new List<characterClass>();

        public int ArcherUnlock_SkeletonArcherKillCount = 20;
        public int killedSkeletonArcher = 0;

        public bool checkClassUnlock(CharacterClass characterClass)
        {
            foreach (characterClass characterClass1 in characterClasses)
            {
                if (characterClass1.Class == characterClass)
                {
                    return characterClass1.Unlock;
                }
            }

            return false;
        }

        public void CheckUnlockCondition()
        {
            if (killedSkeletonArcher >= ArcherUnlock_SkeletonArcherKillCount)
            {
                foreach (characterClass characterClass in characterClasses)
                {
                    if (characterClass.Class == CharacterClass.Archer && characterClass.Unlock == false)
                    {
                        characterClass.Unlock = true;

                        Debug.LogError("아처 클래스 언락됨");
                    }
                }
            }
        }

        public void UnlockArcher()
        {
            foreach (characterClass characterClass in characterClasses)
            {
                if (characterClass.Class == CharacterClass.Archer && characterClass.Unlock == false)
                {
                    characterClass.Unlock = true;

                    Debug.LogError("아처 클래스 언락됨");
                }
            }
        }

        public void UnlockRouge()
        {
            foreach (characterClass characterClass in characterClasses)
            {
                if (characterClass.Class == CharacterClass.Rogue && characterClass.Unlock == false)
                {
                    characterClass.Unlock = true;

                    Debug.LogError("로그 클래스 언락됨");
                }
            }
        }

        public void UnlockMage()
        {
            foreach (characterClass characterClass in characterClasses)
            {
                if (characterClass.Class == CharacterClass.Wizard && characterClass.Unlock == false)
                {
                    characterClass.Unlock = true;

                    Debug.LogError("메이지 클래스 연락됨");
                }
            }
        }

        public void LockAllClasses()
        {
            foreach (characterClass characterClass in characterClasses)
            {
                if(characterClass.Class != CharacterClass.Warrior)
                    characterClass.Unlock = false;
            }
        }

    }

    public characterCalssData classData = new characterCalssData();

    private static bool Initialized = false;

    public static CharacterClassManager instance;

    void Awake()
    {
        if(!Initialized)
        {
            DontDestroyOnLoad(gameObject);
            Initialized = true;
        }

        instance = this;

        LoadCharacterClassUnlock();
    }

    public void UnlockAllClass()
    {
        Debug.Log("치트 : 모든 클래스 언락됨");

        foreach(characterClass characterClass in classData.characterClasses)
        {
            characterClass.Unlock = true;
        }

        string SaveData = ObjectToJson(classData);

        File.WriteAllText(Application.dataPath + "/Resources/CharacterClassUnlock.json", SaveData);
    }

    public void LockAllClasses()
    {
        var data = LoadCharacterClassUnlockData();

        data.LockAllClasses();
        SaveCharacterClassUnlockData(data);
    }

    public static void AddSkeletonArcherKillCount()
    {
        var data = LoadCharacterClassUnlockData();
        data.killedSkeletonArcher++;
        //Debug.LogError("SkeletonArcherKillCount : "  + data.killedSkeletonArcher);

        data.CheckUnlockCondition();

        SaveCharacterClassUnlockData(data);
    }

    public static int GetSkeletonArcherKillCount()
    {
        var data = LoadCharacterClassUnlockData();

        return data.killedSkeletonArcher;
    }

    public static void UnlockArcher()
    {
        var data = LoadCharacterClassUnlockData();

        data.UnlockArcher();

        SaveCharacterClassUnlockData(data);
    }

    public static void ClearStage3()
    {
        var data = LoadCharacterClassUnlockData();

        data.UnlockRouge();

        //Debug.LogError("ClearStage3");

        data.CheckUnlockCondition();

        SaveCharacterClassUnlockData(data);
    }

    public static void KillTheLich()
    {
        var data = LoadCharacterClassUnlockData();

        data.UnlockMage();

        //Debug.LogError("KillTheLich");

        data.CheckUnlockCondition();

        SaveCharacterClassUnlockData(data);
    }

    public void SaveCharacterClassUnlock()
    {
        Debug.Log("캐릭터 클래스 언락데이터 저장 ");

        if (classData.characterClasses.Count == 0)
        {
            for (int i = 0; i < System.Enum.GetNames(typeof(CharacterClass)).Length; i++)
            {
                CharacterClass characterClassEnum = (CharacterClass)System.Enum.ToObject(typeof(CharacterClass), i);

                characterClass characterClass = new characterClass();

                characterClass.setClass(characterClassEnum);

                if(characterClass.Class == CharacterClass.Adventurer)
                {
                    characterClass.Unlock = true;
                }

                if (characterClass.Class == CharacterClass.Warrior)
                {
                    characterClass.Unlock = true;
                }

                Debug.Log(characterClass.Class + " " + characterClass.Unlock);

                classData.characterClasses.Add(characterClass);
            }

        }

        string SaveData = ObjectToJson(classData);

        File.WriteAllText(Application.dataPath + "/Resources/CharacterClassUnlock.json", SaveData);
    }

    public void LoadCharacterClassUnlock()
    {
        Debug.Log("캐릭터 클래스 언락데이터 로드");

        string JsonString = null;

        try
        {
            JsonString = File.ReadAllText(Application.dataPath + "/Resources/CharacterClassUnlock.json");
        }
        catch (FileNotFoundException fe)
        {
            print(fe + "로드파일이 없어서 새로 만듭니다.");
            SaveCharacterClassUnlock();
        }

        characterCalssData loadData = JsonToOject<characterCalssData>(JsonString);

        //Debug.LogError(JsonString);

        classData = null;

        classData = loadData;
    }

    public static void SaveCharacterClassUnlockData(characterCalssData data)
    {
        string SaveData = ObjectToJson(data);

        File.WriteAllText(Application.dataPath + "/Resources/CharacterClassUnlock.json", SaveData);
    }

    public static characterCalssData LoadCharacterClassUnlockData()
    {
        string JsonString = null;

        try
        {
            JsonString = File.ReadAllText(Application.dataPath + "/Resources/CharacterClassUnlock.json");
        }
        catch (FileNotFoundException fe)
        {
            print(fe + "로드할 캐릭터 데이터가 없습니다.");
            return new characterCalssData();
        }

        characterCalssData loadData = JsonToOject<characterCalssData>(JsonString);

        return loadData;
    }

    static string ObjectToJson(object obj)
    {
        return JsonConvert.SerializeObject(obj);
    }

    static T JsonToOject<T>(string jsonData)
    {
        return JsonConvert.DeserializeObject<T>(jsonData);
    }
}
