using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoutManagement : MonoBehaviour
{
    SubstitutionPuzzleManager substitutionPuzzleManager;

    public bool isOnTheWay;
    public int requiredDays;

    public static ScoutManagement Instance { get; private set; }

    private void Awake()
    {

        if (Instance == null) { Instance = this; }

    }

    // Start is called before the first frame update
    void Start()
    {
        substitutionPuzzleManager = FindObjectOfType<SubstitutionPuzzleManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ScoutsWorking()
    {
       if(!isOnTheWay) return;

        if (requiredDays > 0)
        {
            requiredDays -= 1;
            Debug.Log("Days left111: " + requiredDays);
        }
        else if (requiredDays <= 0)
        {
            substitutionPuzzleManager.AddSingleExtraPhrase();
            Debug.Log("Days left222: " + requiredDays);
            isOnTheWay = false;
        }

    }
}
