/*
 * Este exemplo demonstra o uso de Parallel.ForEach para operações que exigem muito processamento da CPU.
 * Ao executar o exemplo, ele gera aleatoriamente 2 milhões de números e tenta filtrar para encontrar os números primos.
 */

/*
 *  Evite esse Casos
 *  
 *  Em código sequencial, é comum ler ou escrever em variáveis estáticas ou campos de classe. No entanto, 
 *  quando várias threads acessam essas variáveis simultaneamente, existe um grande potencial para condições de corrida.
 *  Embora seja possível usar locks para sincronizar o acesso à variável, o custo da sincronização pode prejudicar o desempenho. Portanto,
 *  recomendamos que você evite, ou pelo menos limite, o acesso a estado compartilhado em um loop paralelo sempre que possível. 
 *  
 *  
 *  Evite o paralelismo excessivo.
 *  Ao usar loops paralelos, você incorre nos custos adicionais de particionamento da coleção de origem e sincronização das threads de trabalho.
 *  Os benefícios da paralelização são ainda mais limitados pelo número de processadores no computador. 
 *  Não há ganho de desempenho ao executar várias threads com uso intensivo de computação em apenas um processador. 
 *  Portanto, você deve ter cuidado para não paralelizar demais um loop.
 *  
 *  
 *  
 *  Evite chamadas a métodos que não sejam thread-safe(Concorrencia entre THREADS)
 *  Evite executar loops paralelos na thread da interface do usuário.
 */

/*
 *  Vantagens
 *  
 *  Fácil de Usar
 *  Escalabilidade
 *  Balanceamento de Carga
 */

/*
 *  Desvantagens
 *  
 *  Sobrecarga de Contexto
 *  Thread Safety
 *  Tamanho do Trabalho
 */

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ParallelExample
{
    class Program
    {
        static void Main()
        {
            // 2 million
            var limit = 2_000_000;
            var numbers = Enumerable.Range(0, limit).ToList();

            var watch = Stopwatch.StartNew();
            var primeNumbersFromForeach = GetPrimeList(numbers);
            watch.Stop();

            var watchForParallel = Stopwatch.StartNew();
            var primeNumbersFromParallelForeach = GetPrimeListWithParallel(numbers);
            watchForParallel.Stop();

            Console.WriteLine($"Classical foreach loop | Total prime numbers : {primeNumbersFromForeach.Count} | Time Taken : {watch.ElapsedMilliseconds} ms.");
            Console.WriteLine($"Parallel.ForEach loop  | Total prime numbers : {primeNumbersFromParallelForeach.Count} | Time Taken : {watchForParallel.ElapsedMilliseconds} ms.");

            Console.WriteLine("Press 'Enter' to exit.");
            Console.ReadLine();
        }

        /// <summary>
        /// GetPrimeList returns Prime numbers by using sequential ForEach
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        private static IList<int> GetPrimeList(IList<int> numbers) => numbers.Where(IsPrime).ToList();

        /// <summary>
        /// GetPrimeListWithParallel returns Prime numbers by using Parallel.ForEach
        /// </summary>
        /// <param name="numbers"></param>
        /// <returns></returns>
        private static IList<int> GetPrimeListWithParallel(IList<int> numbers)
        {
            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount
            };
            var primeNumbers = new ConcurrentBag<int>();

            Parallel.ForEach(numbers, parallelOptions, number =>
            {
                if (IsPrime(number))
                {
                    primeNumbers.Add(number);
                }
            });

            return primeNumbers.ToList();
        }

        /// <summary>
        /// IsPrime returns true if number is Prime, else false.(https://en.wikipedia.org/wiki/Prime_number)
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private static bool IsPrime(int number)
        {
            if (number < 2)
            {
                return false;
            }

            for (var divisor = 2; divisor <= Math.Sqrt(number); divisor++)
            {
                if (number % divisor == 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}