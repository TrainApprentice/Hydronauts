//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private Camera cam;
    private GameObject lWall, rWall;
    public GameObject pauseMenu;

    private GameObject baseMeleeEnemy, baseRangedEnemy, baseWall;
    private List<EnemyMain> enemies = new List<EnemyMain>();
    private bool inEncounter = false;
    private int currEncounter = 0;
    public int killCount = 0;
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
        }
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        baseMeleeEnemy = Resources.Load<GameObject>("Prefab/MeleeEnemy");
        baseRangedEnemy = Resources.Load<GameObject>("Prefab/RangedEnemy");
        baseWall = Resources.Load<GameObject>("Prefab/Wall");
    }
    private void Update()
    {
        if (Input.GetKeyDown("p"))
        {
            if (Time.timeScale == 0) ResumeGame();
            else PauseGame();
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
                        newEnemy = Instantiate(baseRangedEnemy, randSpawn, Quaternion.identity);
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
    private void UpdateEnemies()
    {
        for(int i = 0; i < enemies.Count; i++)
        {
            if(enemies[i].isDead)
            {
                Destroy(enemies[i].gameObject);
                enemies.Remove(enemies[i]);
                
                if(inEncounter) killCount++;
            }
        }
    }

    public void StartEncounter(int num)
    {
        currEncounter = num;
        switch(num)
        {
            case 1:
                killGoal = 1;
                break;
            case 2:
                killGoal = 10;
                break;
            case 3:
                killGoal = 8;
                break;
            case 4:
                killGoal = 1;
                break;
        }
        SetWalls(num);
        var test = (num <= 2);
        cam.GetComponent<CameraFollow>().SwapFreeze(test);
        inEncounter = true;
    }

    public void EndEncounter()
    {
        killCount = 0;
        killGoal = 0;
        currEnemies = 0;
        currEncounter = 0;
        inEncounter = false;
        Destroy(rWall);
        Destroy(lWall);
        cam.GetComponent<CameraFollow>().SwapFreeze(false);
    }

    private void SetWalls(int num)
    {
        switch(num)
        {
            case 1:
                lWall = Instantiate(baseWall, new Vector3(-4f, -4.3f, 0f), Quaternion.identity);
                rWall = Instantiate(baseWall, new Vector3(13f, -4.3f, 0f), Quaternion.identity);
                break;
            case 2:
                lWall = Instantiate(baseWall, new Vector3(15.5f, -4.3f, 0f), Quaternion.identity);
                rWall = Instantiate(baseWall, new Vector3(33f, -4.3f, 0f), Quaternion.identity);
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


}
