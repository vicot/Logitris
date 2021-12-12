using System.Linq;
using UnityEngine;


public class WeightedRandomParam<T>
{
    public T Value { get; private set; }
    public float Ratio { get; private set; }

    public WeightedRandomParam(T value, float ratio)
    {
        this.Value = value;
        this.Ratio = ratio;
    }

}

public class WeightedRandomExecutor<T>
{
    public WeightedRandomParam<T>[] Parameters { get; private set; }

    public float Ratio
    {
        get { return this.Parameters.Sum(param => param.Ratio); }
    }

    public WeightedRandomExecutor(params WeightedRandomParam<T>[] parameters)
    {
        this.Parameters = parameters;
    }

    public T Next()
    {
        float randValue = Random.Range(0.0f, 1.0f) * this.Ratio;
        T currentValue = this.Parameters[0].Value;

        foreach(var parameter in this.Parameters)
        {
            currentValue = parameter.Value;
            randValue -= parameter.Ratio;
            if (randValue <= 0)
                break;
        }

        return currentValue;
    }
}
