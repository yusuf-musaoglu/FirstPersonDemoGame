using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get; private set;}

    private List<FishMovment> allTheFish = new List<FishMovment>();
    private List<FishMovment> fishInTheRange = new List<FishMovment>();

    private FishMovment selectedFish = null;
    public bool pickedAFish = false;
    public bool resetFishPose = false;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        
    }

    public void RegisterAllTheFish(FishMovment obj)
    {
        if (!allTheFish.Contains(obj))
        {
            allTheFish.Add(obj);
        }
    }
    public void FishInTheRange(FishMovment obj)
    {
        if (!fishInTheRange.Contains(obj))
        {
            fishInTheRange.Add(obj);
        }
    }

    public void PerformRandomSelection()
    {
        if (allTheFish.Count == 0 || fishInTheRange.Count == 0)
        {
            Debug.LogWarning("balik yok!");
            return;
        }
        
        int randomIndex = Random.Range(0, fishInTheRange.Count);
        selectedFish = fishInTheRange[randomIndex];
        Debug.Log(randomIndex);
        Debug.Log(fishInTheRange.Count);


        foreach (FishMovment obj in fishInTheRange)
        {
            if (obj == selectedFish)
            {
                pickedAFish = true;
                obj.OnSelected(false);
            }
            else
            {
                obj.OnNotSelected();
            }
        }
    }
}