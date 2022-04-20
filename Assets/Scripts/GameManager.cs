//using System;
using System;
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
    public Transform[] encounterPos = new Transform[4];
    public TutorialMove movementTut, combatTut, specialTut;
    public Transform powerSpawn1, powerSpawn2;
    public GameObject encounter1, encounter2, encounter3;

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

    private bool inEncounter = false;
    private int currEncounter = 0;
    private int killGoal = 0;
    private int currEnemies = 0;
    private float enemySpawnCooldown = .3f;

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
            print("Thing");
        }
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Level1") && !player) ResetGameStats();
        if (Input.GetKeyDown("p"))
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

        
    }

    private void FixedUpdate()
    {
        
        UpdateEnemies();
        if (inEncounter && currEnemies < killGoal)
        {
            if (enemySpawnCooldown > 0) enemySpawnCooldown -= Time.fixedDeltaTime;
            else
            {
                var randSpawn = new Vector3(UnityEngine.Random.Range(lWall.transform.position.x + 1f, rWall.transform.position.x - 1f), UnityEngine.Random.Range(lWall.transform.position.y + 3f, lWall.transform.position.y - 2f), 0f);
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
        player.isInCutscene = true;
        SceneManager.LoadScene("EndScene");
    }
    private void GoToGameOver()
    {
        SceneManager.LoadScene("GameOver");
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
                killGoal = 5;
                movementTut.SetNewPosition(false);
                combatTut.SetNewPosition(true);
                break;
            case 2:
                killGoal = 10;
                break;
            case 3:
                killGoal = 8;
                break;
            case 4:
                killGoal = 1;
                bossHealthBar.gameObject.SetActive(true);
                break;
        }
        SetWalls(num);
        cam.SetNewTarget(encounterPos[num-1].position);
        inEncounter = true;
    }
    public void ShowSpecialTutorial()
    {
        specialTut.SetNewPosition(true);
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
        if (maxEncounter == 1) combatTut.SetNewPosition(false);
    }

    public void ResetGameStats()
    {
        cam = FindObjectOfType<CameraFollow>();
        baseMeleeEnemy = Resources.Load<GameObject>("Prefab/MeleeEnemy");
        baseRangedEnemy = Resources.Load<GameObject>("Prefab/RangedEnemy");
        baseBoss = Resources.Load<GameObject>("Prefab/Boss");
        baseWall = Resources.Load<GameObject>("Prefab/Wall");
        player = FindObjectOfType<PlayerMain>();

        pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu");
        GetComponent<PauseMenu>().FindMenuElements();
        
        if(SaveFiles.instance)
        {
            if (SaveFiles.instance.CheckDataInSlot(SaveFiles.instance.chosenSlot))
            {
                LoadInSaveData();
                player.Reset();

            }
            else
            {
                movementTut.SetNewPosition(true);
                SpawnPowerups();
                player.Reset(false);
            }
            
        }
        else
        {
            movementTut.SetNewPosition(true);
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

        if (maxEncounter > 0) encounter1.SetActive(false);
        if (maxEncounter > 1) encounter2.SetActive(false);
        if (maxEncounter > 2) encounter3.SetActive(false);

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
        

        GameObject p1 = Instantiate(powerupBase, powerSpawn1);
        p1.GetComponent<PowerupPickup>().powerupType = 1;

        GameObject p2 = Instantiate(powerupBase, powerSpawn2);
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
    private void OnDestroy()
    {
        
    }

}
