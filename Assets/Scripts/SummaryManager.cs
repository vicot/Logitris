using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SummaryManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreValueText;
    // Start is called before the first frame update
    void Start()
    {
        scoreValueText.text = ScoreManager.GetScore().ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
