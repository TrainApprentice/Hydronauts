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
        // If the P key or Esc key is pressed, pause the game
        if (Input.GetKeyDown("p") || Input.GetKeyDown("escape"))
        {
            if (Time.timeScale == 0) ResumeGame();
            else PauseGame();
        }
        // If the player isn't in an encounter, keep updating the camera's position to the player
        if(!inEncounter && player) cam.SetNewTarget(player.transform.position, true);

        // If the player dies, send them to the Game Over screen
        if (player.isDead && SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Level1"))
        {
            GoToGameOver();
            player.Reset();
            
        }
        // If the player is in the boss' death cutscene, count up the cutscene timer
        if (player.isInCutscene) cutsceneTimer += Time.deltaTime;

        // If the cutscene is complete and we're in the game scene, go to the credits
        if(cutsceneTimer > 4f && SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Level1"))
        {
            GoToCredits();
            player.Reset();
        }
        
    }

    private void FixedUpdate()
    {
        // Updates and spawns all enemies here
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
        
        
        // If the player has killed all the enemies in the encounter, end it
        if (killCount >= killGoal && inEncounter) EndEncounter();
    }

    /// <summary>
    /// Pauses the game, setting the time scale to zero and displaying the pause menu
    /// </summary>
    void PauseGame()
    {
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
    }
    /// <summary>
    /// Resumes the game, setting the time scale to one and hiding the pause menu
    /// </summary>
    public void ResumeGame()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
    }
    /// <summary>
    /// Begin the boss' death cutscene, and start moving the player automatically
    /// </summary>
    public void StartEndCutscene()
    {
        if (player.isInCutscene) return;
        player.isInCutscene = true;
        player.SetMovementInCutscene("left");
    }
    /// <summary>
    /// Send the player to the game over screen
    /// </summary>
    private void GoToGameOver()
    {
        SceneManager.LoadScene("GameOver");
    }
    /// <summary>
    /// Send the player to the credits screen
    /// </summary>
    private void GoToCredits()
    {
        MusicManager.instance.SwitchTrack("Win");
        SceneManager.LoadScene("EndScene");
        
    }
    /// <summary>
    /// Update all the enemies to check if they're dead, and update the kill count if they are
    /// </summary>
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
    /// <summary>
    /// Begins an encounter based on an int input, putting walls in place and setting kill goals
    /// </summary>
    /// <param name="num"></param>
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
    /// <summary>
    /// Displays the special tutorial if the player hasn't seen it
    /// </summary>
    public void ShowSpecialTutorial()
    {
        if(!seenSpecialTut)
        {
            specialTut.SwitchOnOff(true);
            seenSpecialTut = true;
        }
        
    }
    /// <summary>
    /// Hides the special tutorial
    /// </summary>
    public void HideSpecialTutorial()
    {
        specialTut.SwitchOnOff(false);
    }
    /// <summary>
    /// Displays the dousing tutorial if the player hasn't seen it
    /// </summary>
    public void ShowDousingTutorial()
    {
        if(!seenDousingTut)
        {
            dousingTut.SwitchOnOff(true);
            seenDousingTut = true;
        }
        
    }
    /// <summary>
    /// Hides the dousing tutorial
    /// </summary>
    public void HideDousingTutorial()
    {
        dousingTut.SwitchOnOff(false);
    }
    /// <summary>
    /// Ends the current encounter, setting the camera's target back to player and destroying invisible walls
    /// </summary>
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

    /// <summary>
    /// Resets all game stats whenever the game scene is reloaded
    /// </summary>
    public void ResetGameStats()
    {
        // Find all necessary objects in the game scene
        cam = FindObjectOfType<CameraFollow>();
        baseMeleeEnemy = Resources.Load<GameObject>("Prefab/MeleeEnemy");
        baseRangedEnemy = Resources.Load<GameObject>("Prefab/RangedEnemy");
        baseBoss = Resources.Load<GameObject>("Prefab/Boss");
        baseWall = Resources.Load<GameObject>("Prefab/Wall");
        player = FindObjectOfType<PlayerMain>();
        encounterPos = FindObjectsOfType<EncounterMarker>();
        powerSpawn = FindObjectsOfType<PowerPos>();
        bossSpawn = FindObjectOfType<BossSpawnMarker>().transform;
        FindTutorials();

        // Reset the encounters
        EndEncounter();
        SetupEncounters();
        
        bossHealthBar = FindObjectOfType<BossUI>();
        bossHealthBar.gameObject.SetActive(false);
        
        // Resets all relevant values
        cutsceneTimer = 0;
        currEnemies = 0;
        enemySpawnCooldown = .3f;

        // Set up the pause menu and make sure the game isn't paused
        pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu");
        GetComponent<PauseMenu>().FindMenuElements();
        ResumeGame();

        // Switch to the gameplay track
        MusicManager.instance.SwitchTrack("Gameplay");

        // If there's save data, check to see if it's being used
        if (SaveFiles.instance)
        {
            // If there's save data being loaded, load all of it in, otherwise reset to a clean slate
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
    /// <summary>
    /// Loads in all save data from the SaveFiles, and updates the level and player accordingly
    /// </summary>
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
    /// <summary>
    /// Spawns powerups in the level at the correct locations
    /// </summary>
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

    /// <summary>
    /// Sets the invisible walls' locations based on which encounter the player is in
    /// </summary>
    /// <param name="num"></param>
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

    /// <summary>
    /// Finds all tutorial objects in the UI and sets each stored GameObject for them accordingly
    /// </summary>
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
    /// <summary>
    /// Finds all encounter hitboxes in the level and organizes the storage array in ascending numerical order
    /// </summary>
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
