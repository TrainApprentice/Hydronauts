using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    public string currentSpecial;
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
        }
    }
    private void Start()
    {
        UpdateAllSlots();
    }

    public void SaveGame(PlayerMain p, Vector3 pos, int slotNum, int killCount, int levelNum, int lastEncounter)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "SaveData" + slotNum + ".dat");
        SaveData data = new SaveData();
        data.playerPosition = pos;
        data.playerHealth = p.health;
        data.specialMeterCharge = p.specialMeter;
        data.currPlayerSpecial = p.currSpecial;
        data.playerHasSpecial = p.hasSpecial;
        data.killCountEnemies = killCount;
        data.currentLevel = levelNum;
        data.slotNumber = slotNum;
        data.prevEncounter = lastEncounter;
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
            file.Close();

            // Figure out what to load here
            currentLevel = data.currentLevel;
            currentSpecial = data.currPlayerSpecial;
            enemiesKilled = data.killCountEnemies;
            UpdateSlotData(slotNum);
        }
    }
    public void LoadGame(int slotNum)
    {
        if (CheckDataInSlot(slotNum))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "SaveData" + slotNum + ".dat", FileMode.Open);
            SaveData data = (SaveData)bf.Deserialize(file);
            file.Close();
            playerPos = data.playerPosition;
            playerHealth = data.playerHealth;
            playerHasSpecial = data.playerHasSpecial;
            currentSpecial = data.currPlayerSpecial;
            specialMeter = data.specialMeterCharge;

            chosenSlot = data.slotNumber;
            enemiesKilled = data.killCountEnemies;
            currentLevel = data.currentLevel;
            lastEncounter = data.prevEncounter;
        }
        
    }

    public void DeleteData(int slotNum)
    {
        if (CheckDataInSlot(slotNum))
        {
            File.Delete(Application.persistentDataPath + "SaveData" + slotNum + ".dat");
        }
        UpdateSlotData(slotNum);
    }

    public bool CheckDataInSlot(int slotNum)
    {
        return File.Exists(Application.persistentDataPath + "SaveData" + slotNum + ".dat");
    }
    public void UpdateAllSlots()
    {
        SaveSlotDisplay[] temp = FindObjectsOfType<SaveSlotDisplay>();
        foreach (SaveSlotDisplay s in temp)
        {
            LoadSlotDisplay(s.slot);
            s.UpdateVisuals();
        }
    }

    private void UpdateSlotData(int slotNum)
    {
        FindSlot(slotNum);
        currDisplay.currSpecial = currentSpecial;
        currDisplay.enemiesKilled = enemiesKilled;
        currDisplay.currLevel = currentLevel;
        currDisplay.UpdateVisuals();
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

    public Vector3 playerPosition;
    public float specialMeterCharge;
    public bool playerHasSpecial;
    public string currPlayerSpecial;
    public int playerHealth;

    public int killCountEnemies;
    public int currentLevel;
    public int prevEncounter;
}
