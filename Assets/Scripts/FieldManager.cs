using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using Unity.Mathematics;

public class FieldManager : MonoBehaviour
{
    [SerializeField] private GameObject cubePrefab;

    [SerializeField] private int cubeCount=6;

    [SerializeField] private int xOffset=-6;
    [SerializeField] private int yOffset=2;

    [SerializeField] private float litFactorMin = 0.25f;
    [SerializeField] private float litFactorMax = 0.45f;

    [SerializeField] private BrickManager brickManager;
    
    private Field[,] map = null;
    
    void Start()
    {
        CreateCubes();
    }

    private void DestroyCubes()
    {
        if (map is null) return;
        foreach (var field in map)
        {
            Destroy(field.visual);
        }
    }

    private void LitRandomCubes()
    {
        var allCubeCount = cubeCount * cubeCount;
        var cubeLitCount = (int) math.round(UnityEngine.Random.Range(litFactorMin, litFactorMax) * allCubeCount);

        foreach (var index in Enumerable.Range(0, allCubeCount).OrderBy(_ => UnityEngine.Random.Range(0, 1.0f)).Take(cubeLitCount))
        {
            map[index / cubeCount, index % cubeCount].Type = FieldType.Lit;
        }
    }
    
    private void CreateCubes()
    {
        DestroyCubes();
        map = new Field[cubeCount, cubeCount];
        
        for (int i = 0; i < cubeCount; i++)
        {
            for (int j = 0; j < cubeCount; j++)
            {
                var o = Instantiate(cubePrefab, new Vector3(xOffset + i, 0, yOffset - j), Quaternion.identity);
                o.name = $"{i},{j}";
                map[i, j] = new Field(o);
            }
        }

        LitRandomCubes();
    }

    public List<Field> FindFields(Ray ray, Brick brick)
    {
        if (Physics.Raycast(ray, out var hit, 100.0f, LayerMask.GetMask("fields")))
        {
            var obj = hit.transform.gameObject;
            var splitName = obj.name.Split(",");
            var (x, y) = (int.Parse(splitName[0]), int.Parse(splitName[1]));

            return brick.Capacity == 2 ? HandleSmallBrick(brick, x, y) : HandleBigBrick(brick, x, y);
        }

        return new List<Field>();
    }

    private List<Field> HandleSmallBrick(Brick brick, int x, int y)
    {
        var dirx = x < cubeCount - 1 ? 1 : -1;
        var diry = y > 0 ? -1 : 1;

        List<Field> fields;
        
        if (brick.Fields.Any(f => f.pos == 2))
        {
            //vertical
            fields = new List<Field>
            {
                map[x, y],
                map[x, y + diry],
            };
            
            if (diry == 1)
            {
                (fields[0], fields[1]) = (fields[1], fields[0]);
            }

            if (brick.Fields[0].pos == 0)
            {
                fields[0].Operator = brick.Fields[0].oper;
                fields[1].Operator = brick.Fields[1].oper;
            }
            else
            {
                fields[0].Operator = brick.Fields[1].oper;
                fields[1].Operator = brick.Fields[0].oper;
            }
        }
        else
        {
            //horizontal
            fields = new List<Field>
            {
                map[x, y],
                map[x + dirx, y],
            };   
            
            if (dirx == -1)
            {
                (fields[0], fields[1]) = (fields[1], fields[0]);
            }
            
            if (brick.Fields[0].pos == 0)
            {
                fields[0].Operator = brick.Fields[0].oper;
                fields[1].Operator = brick.Fields[1].oper;
            }
            else
            {
                fields[0].Operator = brick.Fields[1].oper;
                fields[1].Operator = brick.Fields[0].oper;
            }
        }
        
        return fields;
    }
    
    private List<Field> HandleBigBrick(Brick brick, int x, int y)
    {
        var dirx = x < cubeCount - 1 ? 1 : -1;
        var diry = y > 0 ? -1 : 1;
        
        var fields = new List<Field>
        {
            map[x, y],
            map[x + dirx, y],
            map[x, y + diry],
            map[x + dirx, y + diry]
        };

        var positions = brick.Fields.Select(f => f.pos);

        if (dirx == -1)
        {
            var f0 = fields[0];
            var f2 = fields[2];
            fields[0] = fields[1];
            fields[1] = f0;
            fields[2] = fields[3];
            fields[3] = f2;
        }

        if (diry == 1)
        {
            var f0 = fields[0];
            var f1 = fields[1];
            fields[0] = fields[2];
            fields[1] = fields[3];
            fields[2] = f0;
            fields[3] = f1;
        }

        for (int i = 0; i < brick.Capacity; i++)
        {
            fields[brick.Fields[i].pos].Operator = brick.Fields[i].oper;
        }
        
        return positions.Select(position => fields[position]).ToList();
    }

    public void ApplyBrick(Brick brick, List<Field> fields)
    {
        Func<Field, Field, bool> func;
        switch (brick.Operation)
        {
            case LogicOperation.EMPTY:
                Debug.LogWarning("Should not happen");
                func = (x, y) => true;
                break;
            case LogicOperation.OR:
                func = (x, y) => x.Type == FieldType.Lit || y.Type == FieldType.Lit;
                break;
            case LogicOperation.AND:
                func = (x, y) => x.Type == FieldType.Lit && y.Type == FieldType.Lit;
                break;
            case LogicOperation.NOT:
                func = (x, _) => x.Type == FieldType.Unlit;
                break;
            default:
                throw new NotImplementedException();
        }

        List<Field> operands = new List<Field>();
        List<Field> resultFields = new List<Field>();

        foreach (var field in fields)
        {
            switch(field.Operator)
            {
                case LogicOperation.EMPTY: resultFields.Add(field);
                    break;
                case LogicOperation.NOT:
                    operands.Add(field);
                    operands.Add(field);
                    break;
                default:
                    operands.Add(field);
                    break;
            }
        }

        Debug.Assert(operands.Count == 2);
        
        var result = func(operands[0], operands[1]);

        foreach (var field in resultFields)
        {
            if(result && field.Type == FieldType.Unlit) ScoreManager.TileLit();
            field.Type = result ? FieldType.Lit : FieldType.Unlit;
        }

        CalculateBonusPoints();
    }

    private void CalculateBonusPoints()
    {
        List<Field> fieldsToClear = new ();
        
        // find vertical lines
        for (int x = 0; x < cubeCount; x++)
        {
            int count = 0;
            for (int y = 0; y < cubeCount; y++)
            {
                if (map[x, y].Type == FieldType.Lit) count++;
            }

            if (count == cubeCount)
            {
                //line found
                for (int y = 0; y < cubeCount; y++)
                {
                    fieldsToClear.Add(map[x,y]);
                }
                ScoreManager.LineCleared();
                brickManager.AddFreeBricks();
                
            }
        }

        // find horizontal lines
        for (int y = 0; y < cubeCount; y++)
        {
            int count = 0;
            for (int x = 0; x < cubeCount; x++)
            {
                if (map[x, y].Type == FieldType.Lit) count++;
            }

            if (count == cubeCount)
            {
                //line found
                for (int x = 0; x < cubeCount; x++)
                {
                    fieldsToClear.Add(map[x,y]);
                }
                ScoreManager.LineCleared();
                brickManager.AddFreeBricks();
            }
        }

        // clear cleared tiles
        foreach (var field in fieldsToClear)
        {
            field.Type = FieldType.Unlit;
        }
        
        // check for full clear
        int emptyFields = 0;
        for (int x = 0; x < cubeCount; x++)
        {
            for (int y = 0; y < cubeCount; y++)
            {
                if (map[x, y].Type == FieldType.Unlit) emptyFields++;
            }
        }

        if (emptyFields == cubeCount * cubeCount)
        {
            ScoreManager.FullClear();
            brickManager.AddFreeBricks();
            LitRandomCubes();
        }
        
    }
}