using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASD
{
    public class Lab02 : MarshalByRefObject
    {
        /// <summary>
        /// Etap 1 - Wyznaczenie ścieżki (seam) o minimalnym sumarycznym score.
        /// Ścieżka przebiega od górnego do dolnego wiersza obrazu.
        /// </summary>
        /// <param name="S">macierz score o wymiarach H x W, gdzie S[i, j] reprezentuje "ważność" piksela w wierszu i i kolumnie j</param>
        /// <returns>
        /// (int cost, (int, int)[] seam) - 
        /// cost: minimalny łączny koszt ścieżki (suma wartości pikseli);
        /// seam: tablica pozycji pikseli (włącznie z pikselem z pierwszego i ostatniego wiersza) tworzących ścieżkę.
        /// </returns>
        public (int cost, (int i, int j)[] seam) Stage1(int[,] S)
        {
            int H = S.GetLength(0);
            int W = S.GetLength(1);
            int[,] C = new int[H, W];
            (int di, int dj)[] moves = {(1,-1),(1,0),(1,1)};
            (int i, int j)[,] parent = new (int i, int j)[H, W];
            for (int i = 0; i < H; i++)
            {
                for (int j = 0; j < W; j++)
                {
                    C[i,j] = int.MaxValue;
                }
            }

            for (int i = 0; i < W; i++)
            {
                C[0, i] = S[0,i];
            }

            for (int i = 0; i < H; i++)
            {
                for (int j = 0; j < W; j++)
                {
                    if (C[i, j] == int.MaxValue) continue;
                    foreach (var move in moves)
                    {
                        int ni = i + move.di;
                        int nj = j + move.dj;
                        if (ni >= 0 && ni < H && nj >= 0 && nj < W)
                        {
                            if (C[i, j] + S[ni, nj] < C[ni, nj])
                            {
                                C[ni, nj] = C[i, j] + S[ni, nj];
                                parent[ni, nj] = (i, j);
                            }
                        }
                        
                    }
                }
            }
            int minCost = int.MaxValue;
            int endJ = -1;
            for (int j = 0; j < W; j++)
            {
                if (C[H - 1, j] < minCost)
                {
                    minCost = C[H - 1, j];
                    endJ = j;
                }
            }

            if (minCost == int.MaxValue)
            {
                return (int.MaxValue, null);
            }
            List<(int i,int j)> path = new List<(int i, int j)>();
            int currI = H - 1;
            int currJ = endJ;

            while (currI >0)
            {
                path.Add((currI, currJ));
                var p = parent[currI, currJ];
                currI = p.i;
                currJ = p.j;
            }
            path.Add((currI,currJ));
            path.Reverse();
            
            return(minCost, path.ToArray());
        }

        /// <summary>
        /// Etap 2 - Wyznaczenie ścieżki (seam) o minimalnym sumarycznym score z uwzględnieniem kary za zmianę kierunku.
        /// Przy każdym przejściu, gdy kierunek ruchu różni się od poprzedniego, do łącznego kosztu dodawana jest kara K.
        /// Pierwszy krok (z pierwszego wiersza) nie podlega karze.
        /// </summary>
        /// <param name="S">macierz score o wymiarach H x W</param>
        /// <param name="K">kara za zmianę kierunku (K >= 1)</param>
        /// <returns>
        /// (int cost, (int, int)[] seam) - 
        /// cost: minimalny łączny koszt ścieżki (suma wartości pikseli oraz naliczonych kar);
        /// seam: tablica pozycji pikseli tworzących ścieżkę.
        /// </returns>
        public (int cost, (int i, int j)[] seam) Stage2(int[,] S, int K)
        {
           int H = S.GetLength(0);
            int W = S.GetLength(1);
            int[,,] C = new int[H, W, 3];
            
           
            (int prevJ, int prevD)[,,] parent = new (int, int)[H, W, 3];

           
            for (int i = 0; i < H; i++)
                for (int j = 0; j < W; j++)
                    for (int d = 0; d < 3; d++)
                        C[i, j, d] = int.MaxValue;

            
            for (int j = 0; j < W; j++)
            {
                for (int d = 0; d < 3; d++)
                {
                    int dj = d - 1;       
                    int prevJ = j - dj;   
                    
                    if (prevJ >= 0 && prevJ < W)
                    {
                        C[1, j, d] = S[0, prevJ] + S[1, j];
                        parent[1, j, d] = (prevJ, -1); 
                    }
                }
            }
            
            for (int i = 2; i < H; i++)
            {
                for (int j = 0; j < W; j++)
                {
                    for (int d = 0; d < 3; d++) 
                    {
                        int dj = d - 1;
                        int prevJ = j - dj;

                        if (prevJ >= 0 && prevJ < W)
                        {
                            for (int prevD = 0; prevD < 3; prevD++) 
                            {
                                if (C[i - 1, prevJ, prevD] != int.MaxValue)
                                {
                                    int penalty = (d == prevD) ? 0 : K;
                                    int currentCost = C[i - 1, prevJ, prevD] + penalty + S[i, j];

                                    if (currentCost < C[i, j, d])
                                    {
                                        C[i, j, d] = currentCost;
                                        parent[i, j, d] = (prevJ, prevD);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            
            int minCost = int.MaxValue;
            int endJ = -1;
            int endD = -1;

            for (int j = 0; j < W; j++)
            {
                for (int d = 0; d < 3; d++)
                {
                    if (C[H - 1, j, d] < minCost)
                    {
                        minCost = C[H - 1, j, d];
                        endJ = j;
                        endD = d;
                    }
                }
            }

            if (minCost == int.MaxValue) return (int.MaxValue, null);
            
            List<(int i, int j)> path = new List<(int i, int j)>();
            int currI = H - 1;
            int currJ = endJ;
            int currD = endD;

            while (currI > 0)
            {
                path.Add((currI, currJ));
                var p = parent[currI, currJ, currD];
                currJ = p.prevJ;
                currD = p.prevD;
                currI--;
            }
            
            path.Add((0, currJ));
            path.Reverse();

            return (minCost, path.ToArray());
        }
    }
}
