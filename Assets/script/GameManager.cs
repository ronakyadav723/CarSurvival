using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Transform playerTransform;

    public GameObject policeCarPrefab;
    public int startPoliceCount = 10;
    public float spawnRadius = 500f;
    public float minSpawnDistance = 60f;
    public float increaseEverySeconds = 30f;
    public int maxPoliceLimit = 30;

    [Header("Lives")]
    public int currentLives;
    public int maxLives;
    public LivesUI livesUI;

    public TMP_Text scoreText;
    public TMP_Text highScoreText;
    public GameObject gameEndPanel;
    public GameObject leftControl;
    public GameObject rightControl;

    public bool playerIsDead;
    public float survivalTime;

    List<AutoCarController> policeCars = new List<AutoCarController>();

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        livesUI.Init(maxLives);
        livesUI.UpdateHearts(currentLives);

        highScoreText.text =
            "HighScore: " + PlayerPrefs.GetInt("HighScore", 0);

        for (int i = 0; i < startPoliceCount; i++)
            SpawnPoliceCar();
    }

    void Update()
    {
        if (playerIsDead) return;

        survivalTime += Time.deltaTime;
        scoreText.text = "Score: " + Mathf.FloorToInt(survivalTime * 10);

        HandleDifficulty();
        MaintainPoliceCars();
    }

   

    void HandleDifficulty()
    {
        int desiredCount =
            startPoliceCount + Mathf.FloorToInt(survivalTime / increaseEverySeconds);

        desiredCount = Mathf.Min(desiredCount, maxPoliceLimit);

        while (policeCars.Count < desiredCount)
            SpawnPoliceCar();
    }

    void MaintainPoliceCars()
    {
        for (int i = policeCars.Count - 1; i >= 0; i--)
        {
            if (policeCars[i] == null)
            {
                policeCars.RemoveAt(i);
                SpawnPoliceCar();
                continue;
            }

            float dist = Vector3.Distance(
                playerTransform.position,
                policeCars[i].transform.position
            );

            if (dist > spawnRadius)
            {
                Destroy(policeCars[i].gameObject);
                policeCars.RemoveAt(i);
                SpawnPoliceCar();
            }
        }
    }

    void SpawnPoliceCar()
    {
        Vector3 spawnPos = GetRandomPositionAroundPlayer();

        GameObject carObj =
            Instantiate(policeCarPrefab, spawnPos, Quaternion.identity);

        AutoCarController car = carObj.GetComponent<AutoCarController>();
        car.target = playerTransform;

        policeCars.Add(car);
    }

    Vector3 GetRandomPositionAroundPlayer()
    {
        Vector2 circle =
            Random.insideUnitCircle.normalized *
            Random.Range(minSpawnDistance, spawnRadius);

        Vector3 pos = playerTransform.position;
        pos += new Vector3(circle.x, 0f, circle.y);

        float terrainHeight =
            Terrain.activeTerrain.SampleHeight(pos) +
            Terrain.activeTerrain.transform.position.y;

        pos.y = terrainHeight;
        return pos;
    }

    public void NotifyPoliceDestroyed(AutoCarController car)
    {
        policeCars.Remove(car);
    }

    public void TakeDamage(int damage = 1)
    {
        if (playerIsDead) return;

        currentLives -= damage;
        livesUI.UpdateHearts(currentLives);

        if (currentLives <= 0)
            EndGame();
    }
    public void UpdateLivesUI()
    {
        if (livesUI != null)
            livesUI.UpdateHearts(currentLives);
        Debug.Log("Lives: " + currentLives);
    }

    void EndGame()
    {
        playerIsDead = true;

        int score = Mathf.FloorToInt(survivalTime * 10);
        int high = PlayerPrefs.GetInt("HighScore", 0);

        if (score > high)
        {
            PlayerPrefs.SetInt("HighScore", score);
            PlayerPrefs.Save();
        }

        gameEndPanel.SetActive(true);
        leftControl.SetActive(false);
        rightControl.SetActive(false);
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene(0);
    }
}
