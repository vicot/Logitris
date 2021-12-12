using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class BrickManager : MonoBehaviour
{
    [SerializeField] private GameObject brickPrefab;
    [SerializeField] private TextMeshProUGUI bricksLeftLabel; 
    public Queue<Brick> Bricks { get; private set; } = new Queue<Brick>();
    [SerializeField] private float xOffset = 2.0f;
    [SerializeField] private float yOffset = 4.0f;
    [SerializeField] private float brickOffset = 1.0f;
    [SerializeField] private float brickScale = 0.4f;
    [SerializeField] private int bricksLeft = 30;

    private Brick GenerateBrick()
    {
        var brickSize = new WeightedRandomExecutor<int>(
            new WeightedRandomParam<int>(2, 10), 
            new WeightedRandomParam<int>(3, 45), 
            new WeightedRandomParam<int>(4, 45)
            ).Next();
        var operation = LogicOperation.NOT;
        if (brickSize > 2)
        {
            operation = new WeightedRandomExecutor<LogicOperation>(
                            new WeightedRandomParam<LogicOperation>(LogicOperation.NOT, 1),
                            new WeightedRandomParam<LogicOperation>(LogicOperation.AND, 1),
                            new WeightedRandomParam<LogicOperation>(LogicOperation.OR, 1)
                            ).Next();
        }
            
        var newBrick = new Brick(new GameObject(), brickSize, operation);
        for (var j = 0; j < newBrick.Capacity; j++)
        {
            var localX = newBrick.Fields[j].pos % 2;
            var localY = newBrick.Fields[j].pos / 2;
            var o = Instantiate(brickPrefab, new Vector3(localX, 0, localY), Quaternion.identity);
            o.transform.parent = newBrick.Obj.transform;
            o.GetComponent<BrickMaterialChanger>().ChangeMaterial(newBrick.Fields[j].oper);
        }
        newBrick.Obj.transform.localScale = new Vector3(brickScale, 1, brickScale);

        return newBrick;
    }

    private void DrawBricks()
    {
        var bricksList = Bricks.AsEnumerable().ToList();
        var x = xOffset;
        var y = yOffset;
        for (var i = 0; i < Bricks.Count; i++)
        {
            bricksList[i].Obj.transform.position = new Vector3(x, -0.15f, y);
            y -= brickOffset;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        for(var i = 0; i < 4; i++)
        {
            Bricks.Enqueue(GenerateBrick());
        }
        DrawBricks();
        bricksLeftLabel.text = bricksLeft.ToString();
    }

    void Update()
    {
        DrawBricks();
    }

    public Brick GetNextBrick()
    {
        if (Bricks.Count == 0) return null;

        var currentBricksCount = Bricks.Count;
        var brick = Bricks.Dequeue();
        brick.Obj.transform.localScale = Vector3.one;
        brick.Obj.transform.position = Vector3.zero;

        if(bricksLeft - currentBricksCount > 0)
            Bricks.Enqueue(GenerateBrick());
        DrawBricks();
        bricksLeftLabel.text = bricksLeft.ToString();

        return brick;
    }

    public int BricksLeft()
    {
        return bricksLeft;
    }

    public void DecreaseBricks()
    {
        if(bricksLeft > 0)
            bricksLeft--;
    }

    public void AddFreeBricks()
    {
        bricksLeft += 2;
    }
}
