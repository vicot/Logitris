using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private static int score=0;
    private static bool mustUpdate = true;
    private static int lines=0;
    private static int tiles=0;
    
    [SerializeField] private TextMeshProUGUI scoreText;

    void Start()
    {
        score = 0;
    }

    void Update()
    {
        if (!mustUpdate) return;

        score += ((int)math.pow(2.0,lines) - 1) * 100;
        score += tiles * 10;

        scoreText.text = score.ToString();
       
        lines = 0;
        tiles = 0;
        mustUpdate = false;
    }

    public static void LineCleared()
    {
        lines++;
        mustUpdate = true;
        AudioManager.queueSound(AudioType.Line);
        
    }

    public static void TileLit()
    {
        tiles++;
        mustUpdate = true;
        AudioManager.queueSound(AudioType.Tile);
    }

    public static void FullClear()
    {
        score += 1000;
        mustUpdate = true;
    }

    public static int GetScore()
    {
        return score;
    }
}
