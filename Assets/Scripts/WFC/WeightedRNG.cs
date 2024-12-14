using System;
using System.Linq;

public class WeightedRNG
{
    private Random _random;
    private (string, int)[] _weights;
    private (int, int)[] _intWeights;

    // For strings
    public WeightedRNG((string, int)[] weights)
    {
        _random = new Random();
        _weights = weights;
    }

    // For ints (which im using for room index searching
    public WeightedRNG((int, int)[] weights)
    {
        _random = new Random();
        _intWeights = weights;
    }

    public string GetRandomStringItem()
    {
        var totalWeight = _weights.Sum(w => w.Item2);
        var roll = _random.Next(totalWeight);

        var cumulativeWeight = 0;
        foreach (var (item, weight) in _weights)
        {
            cumulativeWeight += weight;
            if (roll < cumulativeWeight)
            {
                return item;
            }
        }

        throw new InvalidOperationException("Should not reach here");
    }

    public int GetRandomIntItem()
    {
        var totalWeight = _intWeights.Sum(w => w.Item2);
        var roll = _random.Next(totalWeight);

        var cumulativeWeight = 0;
        foreach (var (item, weight) in _intWeights)
        {
            cumulativeWeight += weight;
            if (roll < cumulativeWeight)
            {
                return item;
            }
        }

        throw new InvalidOperationException("Should not reach here");
    }
}