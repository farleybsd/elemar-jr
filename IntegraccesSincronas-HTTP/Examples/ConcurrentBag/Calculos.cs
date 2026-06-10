using System.Collections.Concurrent;

namespace ConcurrentBag;

public class Calculos
{
    private ConcurrentBag<int> _results = new ConcurrentBag<int>();

    public void PerformCalculations(int[] data)
    {
        Parallel.ForEach(data, number =>
        {
            int result = ComplexCalculation(number);
            _results.Add(result);
        });
    }
    private int ComplexCalculation(int number)
    {
        return number * number; // Exemplo simples de cálculo
    }
    public IEnumerable<int> GetResults()
    {
        return _results;
    }
}
