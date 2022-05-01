using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


public class SaveFiles : MonoBehaviour
{
    public static SaveFiles instance;
    [HideInInspector]
    public int chosenSlot = 0;
    public Vector3 playerPos;
    public int playerHealth;
    public bool playerHasSpecial;
    public string currentSpecial = "";
    public float specialMeter;
 
    public int enemiesKilled;
    public int currentLevel;
    public int lastEncounter;

    private SaveSlotDisplay currDisplay;
    void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
        }

        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += RunOnSceneChange;

            if (CheckDataInSlot(0)) DeleteData(0);
        }
    }
    private void Start()
    {
        UpdateAllSlots();
        
    }
    private void RunOnSceneChange(Scene s, LoadSceneMode mode)
    {
        UpdateAllSlots();
    }
    

    public void SaveGame(PlayerMain p, Vector3 pos, int slotNum, int killCount, int levelNum, int lastEncounter)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "SaveData" + slotNum + ".dat");
        SaveData data = new SaveData();
        data.playerPositionX = pos.x;
        data.playerPositionY = pos.y;
        data.playerHealth = p.health;
        data.specialMeterCharge = p.specialMeter;
        data.currPlayerSpecial = p.currSpecial;
        data.playerHasSpecial = p.hasSpecial;
        data.killCountEnemies = killCount;
        data.currentLevel = levelNum;
        data.slotNumber = slotNum;
        data.prevEncounter = lastEncounter;
        print(p.currSpecial);
        bf.Serialize(file, data);
        file.Close();

    }

    public void LoadSlotDisplay(int slotNum)
    {
        if (CheckDataInSlot(slotNum))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "SaveData" + slotNum + ".dat", FileMode.Open);
            SaveData data = (SaveData)bf.Deserialize(file);
            

            // Figure out what to load here
            currentLevel = data.currentLevel;
            currentSpecial = data.currPlayerSpecial;
            enemiesKilled = data.killCountEnemies;
            playerHasSpecial = data.playerHasSpecial;
            file.Close();
            UpdateSlotData(slotNum, true);
        }
        else
        {
            currentLevel = 0;
            enemiesKilled = 0;
            UpdateSlotData(slotNum, false);
        }
        
    }
    public void LoadGame(int slotNum)
    {
        if (CheckDataInSlot(slotNum) && slotNum != 0)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "SaveData" + slotNum + ".dat", FileMode.Open);
            SaveData data = (SaveData)bf.Deserialize(file);
            file.Close();
            playerPos = new Vector3(data.playerPositionX, data.playerPositionY);
            playerHealth = data.playerHealth;
            playerHasSpecial = data.playerHasSpecial;
            currentSpecial = data.currPlayerSpecial;
            specialMeter = data.specialMeterCharge;

            chosenSlot = data.slotNumber;
            enemiesKilled = data.killCountEnemies;
            currentLevel = data.currentLevel;
            lastEncounter = data.prevEncounter;
        }
        else
        {
            playerPos = new Vector3(-4.9f, -.3f);
            playerHealth = 15;
            playerHasSpecial = false;
            currentSpecial = "";
            specialMeter = 0;

            chosenSlot = slotNum;
            enemiesKilled = 0;
            currentLevel = 0;
            lastEncounter = 0;
            //if (GameManager.instance) GameManager.instance.ResetGameStats();
        }
        
    }

    public void DeleteData(int slotNum)
    {
        if (CheckDataInSlot(slotNum))
        {
            File.Delete(Application.persistentDataPath + "SaveData" + slotNum + ".dat");
        }
        UpdateAllSlots();
    }

    public bool CheckDataInSlot(int slotNum)
    {
        return File.Exists(Application.persistentDataPath + "SaveData" + slotNum + ".dat");
    }
    public void UpdateAllSlots()
    {
        SaveSlotDisplay[] temp = FindObjectsOfType<SaveSlotDisplay>();
        if (temp.Length <= 0) return;
        foreach (SaveSlotDisplay s in temp)
        {
            LoadSlotDisplay(s.slot);
            //print(CheckDataInSlot(s.slot));
            s.UpdateVisuals(!CheckDataInSlot(s.slot));
        }
    }

    private void UpdateSlotData(int slotNum, bool isCleared)
    {
        FindSlot(slotNum);
        
        currDisplay.currSpecial = currentSpecial;
        currDisplay.enemiesKilled = enemiesKilled;
        currDisplay.currLevel = currentLevel;
        currDisplay.UpdateVisuals(isCleared);
    }
    private void FindSlot(int slotNum)
    {
        SaveSlotDisplay[] temp = FindObjectsOfType<SaveSlotDisplay>();
        foreach (SaveSlotDisplay s in temp)
        {
            if (s.slot == slotNum) currDisplay = s;
        }
    }

    

}


[Serializable]
class SaveData
{
    public int slotNumber;

    public float playerPositionX;
    public float playerPositionY;
    public float specialMeterCharge;
    public bool playerHasSpecial;
    public string currPlayerSpecial;
    public int playerHealth;

    public int killCountEnemies;
    public int currentLevel;
    public int prevEncounter;
}
