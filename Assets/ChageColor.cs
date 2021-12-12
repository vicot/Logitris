using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChageColor : MonoBehaviour
{
    private TextMeshProUGUI text;
    public float timeToChange = 0.1f;
    private float timeSinceChange = 0f;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceChange += Time.deltaTime;
        if(text != null && timeSinceChange >= timeToChange)
        {
            Color color = new Color(Random.value, Random.value, Random.value);
            text.color = color;
            timeSinceChange = 0f;
        }
        
    }
}
