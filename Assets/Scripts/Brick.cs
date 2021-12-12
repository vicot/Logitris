using System.Collections.Generic;
using UnityEngine;


public class Brick
{
    public GameObject Obj { get; private set; }
    public List<(int pos, LogicOperation oper)> Fields { get; private set; } = new();
    public int Capacity { get; private set; }
    public LogicOperation Operation { get; private set; }

    public Brick(GameObject obj, int capacity, LogicOperation operation)
    {
        Obj = obj;
        Capacity = capacity;
        Operation = operation;
        var positions = new List<int>() { 0, 1, 2, 3 };
        var operatorCount = operation == LogicOperation.NOT ? 1 : 2;
        for(var i = 0; i < capacity; i++)
        {
            int indexToAdd;
            do
            {
                indexToAdd = positions[Random.Range(0, positions.Count)];
            }
            while (capacity == 2 && i == 1 && (Fields[0].pos ^ indexToAdd) == 3);

            var operatorToAdd = i < operatorCount ? operation : LogicOperation.EMPTY;

            Fields.Add((pos: indexToAdd, oper: operatorToAdd));
            positions.Remove(indexToAdd);
        }

        //move to bottom row or left column if single-line
        if (capacity == 2)
        {
            //top row, 2+3
            if (Fields[0].pos + Fields[1].pos == 5)
            {
                Fields[0] = (Fields[0].pos-2, Fields[0].oper);
                Fields[1] = (Fields[1].pos-2, Fields[1].oper);
            }
            
            //right column, 1+3
            else if (Fields[0].pos + Fields[1].pos == 4)
            {
                Fields[0] = (Fields[0].pos-1, Fields[0].oper);
                Fields[1] = (Fields[1].pos-1, Fields[1].oper);
            }
        }
    }
}
