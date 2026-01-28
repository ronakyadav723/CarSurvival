using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public LivesUI livesUI;

    public TMP_Text scoreText;
    public TMP_Text highScoreText;
    public GameObject gameEndPanel;
    public GameObject leftControl;
    public GameObject rightControl;

    public GameObject policeCarPrefab;
    public GameObject healthPrefab;
    public Transform playerTransform;

    public int initialCars ;
    public float survivalTime;

    public int currentLives ;
    public int maxLives ;

    public bool playerIsDead;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        livesUI.Init(maxLives);
        livesUI.UpdateHearts(currentLives);

        highScoreText.text =
            "HighScore: " + PlayerPrefs.GetInt("HighScore", 0);

        for (int i = 0; i < initialCars; i++)
        {
            SpawnCars();

        }
        for (int i = 0; i < initialCars / 3; i++)
        {
            SpawnLives();
        }



    }

    private void Update()
    {
        if (playerIsDead) return;

        survivalTime += Time.deltaTime;
        scoreText.text = "Score: " + Mathf.FloorToInt(survivalTime * 10);
    }

    public void SpawnCars()
    {
        float x = UnityEngine.Random.Range(-5000f, 5000f);
        float z = UnityEngine.Random.Range(-5000f, 5000f);

        Vector3 spawnPos = GetSafeSpawnPosition(x, z, 0f);

        GameObject car = Instantiate(policeCarPrefab, spawnPos, quaternion.identity);
        car.GetComponent<AutoCarController>().target = playerTransform;
    }

    public void SpawnLives()
    {
        float x = UnityEngine.Random.Range(-5000f, 5000f);
        float z = UnityEngine.Random.Range(-5000f, 5000f);

        Vector3 spawnPos = GetSafeSpawnPosition(x, z, 1.15f);
        GameObject life = Instantiate(healthPrefab, spawnPos, quaternion.identity);

    }

    public void TakeDamage(int damage = 1)
    {
        if (playerIsDead) return;

        currentLives -= damage;
        UpdateLivesUI();

        if (currentLives <= 0)
            EndGame();
    }

    private void EndGame()
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

    public void UpdateLivesUI()
    {
        if (livesUI != null)
            livesUI.UpdateHearts(currentLives);
        Debug.Log("Lives: " + currentLives);
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene(0);
    }
    Vector3 GetSafeSpawnPosition(float x, float z, float heightOffset)
    {
        float terrainHeight = Terrain.activeTerrain.SampleHeight(
            new Vector3(x, 0, z)
        );

        terrainHeight += Terrain.activeTerrain.transform.position.y;

        return new Vector3(x, terrainHeight + heightOffset, z);
    }

}
