//using System;
//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject pauseMenu;
    public GameObject powerupBase;
    public Transform bossSpawn;
    public int killCount = 0;
    public BossUI bossHealthBar;
    public EncounterMarker[] encounterPos = new EncounterMarker[4];

    [HideInInspector]
    public PlayerMain player;
    [HideInInspector]
    public int maxEncounter = 0;
    [HideInInspector]
    public int totalKills = 0;
    [HideInInspector]
    public bool hasWon = false;

    private CameraFollow cam;
    private GameObject lWall, rWall;
    private GameObject baseMeleeEnemy, baseRangedEnemy, baseBoss, baseWall;
    private BossAI currBoss;
    private List<EnemyMain> enemies = new List<EnemyMain>();

    private TutorialMove movementTut, combatTut, specialTut, dousingTut;
    private PowerPos[] powerSpawn = new PowerPos[2];
    private EncounterBox encounter1, encounter2, encounter3;

    private bool seenMoveTut, seenCombatTut, seenSpecialTut, seenDousingTut;
    private bool inEncounter = false;
    private int currEncounter = 0;
    private int killGoal = 0;
    private int currEnemies = 0;
    private float enemySpawnCooldown = .3f;
    private float cutsceneTimer = 0;

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
            ResetGameStats();
            
        }
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Level1") && !player) ResetGameStats();
        if (Input.GetKeyDown("p") || Input.GetKeyDown("escape"))
        {
            if (Time.timeScale == 0) ResumeGame();
            else PauseGame();
        }
        if(!inEncounter && player) cam.SetNewTarget(player.transform.position, true);
        if (player.isDead && SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Level1"))
        {
            GoToGameOver();
            player.Reset();
            
        }
        if (player.isInCutscene) cutsceneTimer += Time.deltaTime;
        if(cutsceneTimer > 4f && SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Level1"))
        {
            GoToCredits();
            player.Reset();
        }
        
    }

    private void FixedUpdate()
    {
        
        UpdateEnemies();
        if (inEncounter && currEnemies < killGoal)
        {
            if (enemySpawnCooldown > 0) enemySpawnCooldown -= Time.fixedDeltaTime;
            else
            {
                var randSpawn = new Vector3(Random.Range(lWall.transform.position.x + 1f, rWall.transform.position.x - 1f), Random.Range(lWall.transform.position.y + 3f, lWall.transform.position.y - 2f), 0f);
                GameObject newEnemy;

                switch(currEncounter)
                {
                    case 1:
                        newEnemy = Instantiate(baseMeleeEnemy, randSpawn, Quaternion.identity);
                        enemies.Add(newEnemy.GetComponent<EnemyMain>());
                        break;
                    case 2:
                        newEnemy = (currEnemies % 4 == 0) ? Instantiate(baseRangedEnemy, randSpawn, Quaternion.identity) : Instantiate(baseMeleeEnemy, randSpawn, Quaternion.identity);
                        enemies.Add(newEnemy.GetComponent<EnemyMain>());
                        break;
                    case 3:
                        newEnemy = (currEnemies % 2 == 0) ? Instantiate(baseRangedEnemy, randSpawn, Quaternion.identity) : Instantiate(baseMeleeEnemy, randSpawn, Quaternion.identity);
                        enemies.Add(newEnemy.GetComponent<EnemyMain>());
                        break;

                    case 4:
                        newEnemy = Instantiate(baseBoss, bossSpawn.position, Quaternion.identity);
                        currBoss = newEnemy.GetComponent<BossAI>();
                        break;
                }

                
                currEnemies++;
                enemySpawnCooldown = .3f;
            }
        }
        
        

        if (killCount >= killGoal && inEncounter) EndEncounter();
    }


    void PauseGame()
    {
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
    }
    public void ResumeGame()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
    }
    public void StartEndCutscene()
    {
        if (player.isInCutscene) return;
        player.isInCutscene = true;
        player.SetMovementInCutscene("left");
    }
    private void GoToGameOver()
    {
        SceneManager.LoadScene("GameOver");
    }
    private void GoToCredits()
    {
        MusicManager.instance.SwitchTrack("Win");
        SceneManager.LoadScene("EndScene");
        
    }
    private void UpdateEnemies()
    {
        for(int i = 0; i < enemies.Count; i++)
        {
            if(enemies[i].isDead)
            {
                Destroy(enemies[i].gameObject);
                enemies.Remove(enemies[i]);

                if (inEncounter)
                {
                    killCount++;
                    totalKills++;
                }
            }
        }
    }

    public void StartEncounter(int num)
    {
        currEncounter = num;
        switch(num)
        {
            case 1:
                killGoal = 3;
                movementTut.SwitchOnOff(false);
                if (!seenCombatTut)
                {
                    combatTut.SwitchOnOff(true);
                    seenCombatTut = true;
                }
                break;
            case 2:
                killGoal = 8;
                break;
            case 3:
                killGoal = 6;
                break;
            case 4:
                killGoal = 1;
                bossHealthBar.gameObject.SetActive(true);
                MusicManager.instance.SwitchTrack("Boss");
                break;
        }
        SetWalls(num);
        cam.SetNewTarget(encounterPos[num-1].transform.position);
        inEncounter = true;
    }
    public void ShowSpecialTutorial()
    {
        if(!seenSpecialTut)
        {
            specialTut.SwitchOnOff(true);
            seenSpecialTut = true;
        }
        
    }
    public void HideSpecialTutorial()
    {
        specialTut.SwitchOnOff(false);
    }
    public void ShowDousingTutorial()
    {
        if(!seenDousingTut)
        {
            dousingTut.SwitchOnOff(true);
            seenDousingTut = true;
        }
        
    }
    public void HideDousingTutorial()
    {
        dousingTut.SwitchOnOff(false);
    }
    public void EndEncounter()
    {
        killCount = 0;
        killGoal = 0;
        currEnemies = 0;
        maxEncounter = currEncounter;
        currEncounter = 0;
        inEncounter = false;
        Destroy(rWall);
        Destroy(lWall);
        cam.SetNewTarget(player.transform.position);
        if (maxEncounter == 1)
        {
            ShowDousingTutorial();
            combatTut.SwitchOnOff(false);
        }
    }

    public void ResetGameStats()
    {
        cam = FindObjectOfType<CameraFollow>();
        baseMeleeEnemy = Resources.Load<GameObject>("Prefab/MeleeEnemy");
        baseRangedEnemy = Resources.Load<GameObject>("Prefab/RangedEnemy");
        baseBoss = Resources.Load<GameObject>("Prefab/Boss");
        baseWall = Resources.Load<GameObject>("Prefab/Wall");
        player = FindObjectOfType<PlayerMain>();
        encounterPos = FindObjectsOfType<EncounterMarker>();
        powerSpawn = FindObjectsOfType<PowerPos>();
        bossSpawn = FindObjectOfType<BossSpawnMarker>().transform;
        EndEncounter();
        SetupEncounters();
        FindTutorials();
        bossHealthBar = FindObjectOfType<BossUI>();
        bossHealthBar.gameObject.SetActive(false);
        cutsceneTimer = 0;

        currEnemies = 0;
        enemySpawnCooldown = .3f;

        pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu");
        GetComponent<PauseMenu>().FindMenuElements();
        ResumeGame();

        MusicManager.instance.SwitchTrack("Gameplay");
        if (SaveFiles.instance)
        {
            if (SaveFiles.instance.CheckDataInSlot(SaveFiles.instance.chosenSlot))
            {
                LoadInSaveData();
                player.Reset();
            }
            else
            {
                movementTut.SwitchOnOff(true);
                LoadInSaveData();
                SpawnPowerups();
                player.Reset(false);
                totalKills = 0;
            }

        }
        else
        {
            movementTut.SwitchOnOff(true);
            SpawnPowerups();
            player.Reset(false);
        }
        
    }

    private void LoadInSaveData()
    {
        player.transform.position = SaveFiles.instance.playerPos;
        player.hasSpecial = SaveFiles.instance.playerHasSpecial;
        player.health = SaveFiles.instance.playerHealth;
        player.specialMeter = SaveFiles.instance.specialMeter;
        player.currSpecial = SaveFiles.instance.currentSpecial;

        totalKills = SaveFiles.instance.enemiesKilled;
        maxEncounter = SaveFiles.instance.lastEncounter;
        seenSpecialTut = player.hasSpecial;
        if (maxEncounter > 0)
        {
            encounter1.gameObject.SetActive(false);
            seenCombatTut = true;
            seenMoveTut = true;
            seenDousingTut = false;
        }
        if (maxEncounter > 1)
        {
            encounter2.gameObject.SetActive(false);
            seenCombatTut = true;
            seenMoveTut = true;
            seenDousingTut = true;
        }
        if (maxEncounter > 2)
        {
            encounter3.gameObject.SetActive(false);
            seenCombatTut = true;
            seenMoveTut = true;
            seenDousingTut = true;
        }

        if (!player.hasSpecial) SpawnPowerups();
    }
    private void SpawnPowerups()
    {
        PowerupPickup[] temp = FindObjectsOfType<PowerupPickup>();
        if(temp.Length > 0)
        {
            foreach (PowerupPickup p in temp)
            {
                Destroy(p.gameObject);
            }
        }
        

        GameObject p1 = Instantiate(powerupBase, powerSpawn[0].transform);
        p1.GetComponent<PowerupPickup>().powerupType = 1;

        GameObject p2 = Instantiate(powerupBase, powerSpawn[1].transform);
        p2.GetComponent<PowerupPickup>().powerupType = 2;
    }

    private void SetWalls(int num)
    {
        switch(num)
        {
            case 1:
                lWall = Instantiate(baseWall, new Vector3(-4f, -2f, 0f), Quaternion.identity);
                rWall = Instantiate(baseWall, new Vector3(13f, -2f, 0f), Quaternion.identity);
                break;
            case 2:
                lWall = Instantiate(baseWall, new Vector3(15.5f, -2f, 0f), Quaternion.identity);
                rWall = Instantiate(baseWall, new Vector3(33f, -2f, 0f), Quaternion.identity);
                break;
            case 3:
                lWall = Instantiate(baseWall, new Vector3(47f, -12f, 0f), Quaternion.identity);
                rWall = Instantiate(baseWall, new Vector3(60f, -12f, 0f), Quaternion.identity);
                break;
            case 4:
                lWall = Instantiate(baseWall, new Vector3(15.5f, -23f, 0f), Quaternion.identity);
                rWall = Instantiate(baseWall, new Vector3(33f, -23f, 0f), Quaternion.identity);
                break;

        }
    }

    private void FindTutorials()
    {
        TutorialMove[] temp = FindObjectsOfType<TutorialMove>();

        foreach(TutorialMove t in temp)
        {
            switch (t.title)
            {
                case "movement":
                    movementTut = t;
                    break;
                case "combat":
                    combatTut = t;
                    break;
                case "special":
                    specialTut = t;
                    break;
                case "dousing":
                    dousingTut = t;
                    break;
            }
            t.SwitchOnOff(false);
        }
    }
    private void SetupEncounters()
    {
        EncounterMarker[] temp = new EncounterMarker[encounterPos.Length];

        foreach (EncounterMarker e in encounterPos)
        {
            temp[e.encounterNum - 1] = e;
        }
        encounterPos = temp;

        EncounterBox[] temp2 = FindObjectsOfType<EncounterBox>();

        foreach (EncounterBox e in temp2)
        {
            switch(e.boxNum)
            {
                case 1:
                    encounter1 = e;
                    break;
                case 2:
                    encounter2 = e;
                    break;
                case 3:
                    encounter3 = e;
                    break;

            }
        }

    }

}
