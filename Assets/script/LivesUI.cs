using UnityEngine;
using System.Collections.Generic;

public class LivesUI : MonoBehaviour
{
    public GameObject heartPrefab;
    public Transform heartsContainer;

    private List<GameObject> hearts = new List<GameObject>();

    public void Init(int maxLives)
    {


        Debug.Log("LivesUI Init called. MaxLives = " + maxLives);

        foreach (var h in hearts)
            Destroy(h);

        hearts.Clear();

        for (int i = 0; i < maxLives; i++)
        {
            GameObject heart = Instantiate(heartPrefab, heartsContainer);
            hearts.Add(heart);
        }

        Debug.Log("Hearts created = " + hearts.Count);

    }

    public void UpdateHearts(int currentLives)
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            hearts[i].SetActive(i < currentLives);
        }
    }
}
