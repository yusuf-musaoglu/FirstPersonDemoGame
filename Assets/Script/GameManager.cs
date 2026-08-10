using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get; private set;}

    FishingRod_Script frs;
    BaitHolder_Script baitHolder;

    public List<FishMovment> fishInTheRange = new List<FishMovment>();

    private FishMovment selectedFish = null;
    public bool charmed = false;
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
            Destroy(gameObject);

        frs = FishingRod_Script.Instance;
    }

    void Update()
    {
        
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
        if (fishInTheRange.Count == 0)
        {
            Debug.LogWarning("balik yok!");
            return;
        }

        int randomIndex = Random.Range(0, fishInTheRange.Count);
        selectedFish = fishInTheRange[randomIndex];
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
    public void PerformResetIndex(FishMovment obj)
    {
        obj.OnNotSelected();
        selectedFish = null;
        Debug.Log(selectedFish);
    }
}