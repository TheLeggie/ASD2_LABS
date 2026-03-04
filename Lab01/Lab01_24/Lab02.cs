using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ASD
{
    public class Lab02 : MarshalByRefObject
    {
        
        private class MoveNode
        {
            public int MoveIndex;
            public MoveNode Prev;

            public MoveNode(int moveIndex, MoveNode prev)
            {
                MoveIndex = moveIndex;
                Prev = prev;
            }
        }
        /// <summary>
        /// Etap 1 - wyznaczenie najtańszej trasy, zgodnie z którą pionek przemieści się z pozycji poczatkowej (0,0) na pozycję docelową
        /// </summary>
        /// <param name="n">wysokość prostokąta</param>
        /// <param name="m">szerokość prostokąta</param>
        /// <param name="moves">tablica z dostępnymi ruchami i ich kosztami (di - o ile zwiększamy numer wiersza, dj - o ile zwiększamy numer kolumnj, cost - koszt ruchu)</param>
        /// <returns>(bool result, int cost, (int, int)[] path) - result ma wartość true jeżeli trasa istnieje, false wpp., cost to minimalny koszt, path to wynikowa trasa</returns>
        public (bool result, int cost, (int i, int j)[] path) Lab02Stage1(int n, int m, ((int di, int dj) step, int cost)[] moves)
        {
            int[,] dp = new int[n, m];
            (int i, int j)[,] parent = new (int i, int j)[n, m];

            // Inicjalizacja tablicy kosztów
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    dp[i, j] = int.MaxValue;
                }
            }

            dp[0, 0] = 0;

            // Przechodzimy po planszy z góry na dół i od lewej do prawej.
            // Zapewnia to poprawne przetwarzanie dla ruchów, które można powtarzać (Unbounded Knapsack)
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    if (dp[i, j] == int.MaxValue) continue;

                    foreach (var move in moves)
                    {
                        // Pomijamy ruchy puste, żeby uniknąć nieskończonych pętli w miejscu
                        if (move.step.di == 0 && move.step.dj == 0) continue;

                        int ni = i + move.step.di;
                        int nj = j + move.step.dj;

                        // Jeśli po wykonaniu ruchu mieścimy się na planszy
                        if (ni < n && nj < m)
                        {
                            if (dp[i, j] + move.cost < dp[ni, nj])
                            {
                                dp[ni, nj] = dp[i, j] + move.cost;
                                parent[ni, nj] = (i, j); // zapamiętujemy skąd przyszliśmy
                            }
                        }
                    }
                }
            }

            // Szukamy minimalnego kosztu w ostatnim wierszu (n - 1)
            int minCost = int.MaxValue;
            int endJ = -1;
            for (int j = 0; j < m; j++)
            {
                if (dp[n - 1, j] < minCost)
                {
                    minCost = dp[n - 1, j];
                    endJ = j;
                }
            }

            if (minCost == int.MaxValue)
            {
                return (false, int.MaxValue, null);
            }

            // Odtwarzanie ścieżki
            List<(int i, int j)> path = new List<(int i, int j)>();
            int currI = n - 1;
            int currJ = endJ;
            
            while (currI != 0 || currJ != 0)
            {
                path.Add((currI, currJ));
                var p = parent[currI, currJ];
                currI = p.i;
                currJ = p.j;
            }
            path.Add((0, 0));
            path.Reverse(); // Odwracamy trasę, aby prowadziła od (0,0) do mety

            return (true, minCost, path.ToArray());
            
        }


        /// <summary>
        /// Etap 2 - wyznaczenie najtańszej trasy, zgodnie z którą pionek przemieści się z pozycji poczatkowej (0,0) na pozycję docelową - dodatkowe założenie, każdy ruch może być wykonany co najwyżej raz
        /// </summary>
        /// <param name="n">wysokość prostokąta</param>
        /// <param name="m">szerokość prostokąta</param>
        /// <param name="moves">tablica z dostępnymi ruchami i ich kosztami (di - o ile zwiększamy numer wiersza, dj - o ile zwiększamy numer kolumnj, cost - koszt ruchu)</param>
        /// <returns>(bool result, int cost, (int, int)[] path) - result ma wartość true jeżeli trasa istnieje, false wpp., cost to minimalny koszt, path to wynikowa trasa</returns>
        public (bool result, int cost, (int i, int j)[] pat) Lab02Stage2(int n, int m, ((int di, int dj) step, int cost)[] moves)
        {
            int[,] dp = new int[n, m];
            MoveNode[,] pathNode = new MoveNode[n, m];

            // Inicjalizacja tablicy kosztów
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    dp[i, j] = int.MaxValue;
                }
            }

            dp[0, 0] = 0;

            // Zewnętrzna pętla po DOSTĘPNYCH RUCHACH (mechanizm plecaka 0-1)
            for (int k = 0; k < moves.Length; k++)
            {
                var move = moves[k];
                if (move.step.di == 0 && move.step.dj == 0) continue;

                // Wewnętrzne pętle idą OD TYŁU (z dołu do góry, od prawej do lewej).
                // Dzięki temu nowo wyliczony stan w tej iteracji nie zostanie ponownie zużyty dla tego samego ruchu.
                for (int i = n - 1; i >= 0; i--)
                {
                    for (int j = m - 1; j >= 0; j--)
                    {
                        if (dp[i, j] != int.MaxValue)
                        {
                            int ni = i + move.step.di;
                            int nj = j + move.step.dj;

                            if (ni < n && nj < m)
                            {
                                if (dp[i, j] + move.cost < dp[ni, nj])
                                {
                                    dp[ni, nj] = dp[i, j] + move.cost;
                                    // Zapamiętujemy zbiór użytych ruchów w postaci węzła (Node)
                                    pathNode[ni, nj] = new MoveNode(k, pathNode[i, j]);
                                }
                            }
                        }
                    }
                }
            }

            // Szukamy minimalnego kosztu w ostatnim wierszu
            int minCost = int.MaxValue;
            int endJ = -1;
            for (int j = 0; j < m; j++)
            {
                if (dp[n - 1, j] < minCost)
                {
                    minCost = dp[n - 1, j];
                    endJ = j;
                }
            }

            if (minCost == int.MaxValue)
            {
                return (false, int.MaxValue, null);
            }

            // Odtwarzanie użytych ruchów z podlinkowanych węzłów
            List<int> usedMoves = new List<int>();
            MoveNode curr = pathNode[n - 1, endJ];
            while (curr != null)
            {
                usedMoves.Add(curr.MoveIndex);
                curr = curr.Prev;
            }
            usedMoves.Reverse(); // Układamy je w takiej kolejności, w jakiej iterowaliśmy (kolejność k)

            // Konstruowanie fizycznych współrzędnych krok po kroku
            List<(int i, int j)> path = new List<(int i, int j)>();
            int ci = 0, cj = 0;
            path.Add((ci, cj));
            foreach (int k in usedMoves)
            {
                ci += moves[k].step.di;
                cj += moves[k].step.dj;
                path.Add((ci, cj));
            }

            return (true, minCost, path.ToArray());

        }
    }
}