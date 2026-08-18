using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get; private set;}

    private BaitHolder_Script baitHolder;

    public List<FishMovment> fishInTheRange = new List<FishMovment>();

    private float timerMeter;
    private FishMovment selectedFish = null;
    public bool charmed = false;
    public bool pickedAFish = false;
    public bool resetFishPose = false;
    public Ray ray;
    private Collider[] hits;
    [SerializeField] private LayerMask fishLayer;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        baitHolder = FindAnyObjectByType<BaitHolder_Script>();

        timerMeter = 0;
    }

    void Update()
    {
        if (FishingRod_Script.Instance.isFishing)
            FishInTheRange();
        
        TimeMeter();
    }

    public float TimeMeter()
    {
        return timerMeter += Time.deltaTime; 
    }

    public void FishInTheRange()
    {
        fishInTheRange.Clear();

        hits = Physics.OverlapSphere(BaitHolder_Script.Instance.transform.position, BaitHolder_Script.Instance.radius, fishLayer);
        foreach (Collider col in hits)
        {
            FishMovment fish = col.GetComponent<FishMovment>();

            if (!fishInTheRange.Contains(fish))
            {
                fishInTheRange.Add(fish);
            }
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

    public void ResetTheFish()
    {

        Debug.Log("resres");
        resetFishPose = true;
        pickedAFish = false;
        if (selectedFish != null)
            selectedFish.ResetFunction();
        selectedFish = null;

    }    

    public void PerformResetIndex(FishMovment obj)
    {
        obj.OnNotSelected();
        selectedFish = null;
        Debug.Log(selectedFish);
    }
}