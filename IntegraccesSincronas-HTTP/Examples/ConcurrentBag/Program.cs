
/*
 * CARACTERÍSTICAS:
 * IDEAL PARA CENÁRIOS ONDE MÚLTIPLAS
   THREADS PRECISAM ADICIONAR E
   REMOVER ITENS

    LOCK-FREE PARA OPERAÇÕES DE
    ADICIONAR E REMOVER ITENS

 */

using ConcurrentBag;

///Exemplo: Suponha que você está executando cálculos em paralelo e precisa armazenar os resultados.
///
Calculos calculos = new Calculos();


int[] numeros = new int[5] { 1, 2, 3, 4, 5 };

calculos.PerformCalculations(numeros);
calculos.GetResults().ToList().ForEach(result => Console.WriteLine(result));