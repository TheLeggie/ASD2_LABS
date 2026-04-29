
using System.Collections.Generic;
using System.Linq;
using ASD.Graphs;

/// <summary>
/// Klasa rozszerzająca klasę Graph o rozwiązania problemów największej kliki i izomorfizmu grafów metodą pełnego przeglądu (backtracking)
/// </summary>
public static class Lab10GraphExtender
{
    /// <summary>
    /// Wyznacza największą klikę w grafie i jej rozmiar metodą pełnego przeglądu (backtracking)
    /// </summary>
    /// <param name="g">Badany graf</param>
    /// <param name="clique">Wierzchołki znalezionej największej kliki - parametr wyjściowy</param>
    /// <returns>Rozmiar największej kliki</returns>
    /// <remarks>
    /// Nie wolno modyfikować badanego grafu.
    /// </remarks>
    public static int MaxClique(this Graph g, out int[] clique)
    {
        int[] bestS = new int[0];
        List<int> S = new List<int>();

        void MaxCliqueRec(int k)
        {
            List<int> C = new();
            for (int i = k; i < g.VertexCount; i++)
            {
                bool connToAll = true;
                foreach (var v in S)
                {
                    if (!g.HasEdge(i, v))
                    {
                        connToAll = false;
                        break;
                    }
                }

                if (connToAll)
                {
                    C.Add(i);
                }
            }

            if (S.Count + C.Count <= bestS.Length)
            {
                return;
            }

            if (S.Count > bestS.Length)
            {
                bestS = S.ToArray();
            }

            foreach (var m in C)
            {
                S.Add(m);
                MaxCliqueRec(m + 1);
                S.Remove(m);
            }
        }
        
        MaxCliqueRec(0);
        clique = bestS;
        return clique.Length;
    }

    /// <summary>
    /// Bada izomorfizm grafów metodą pełnego przeglądu (backtracking)
    /// </summary>
    /// <param name="g">Pierwszy badany graf</param>
    /// <param name="h">Drugi badany graf</param>
    /// <param name="map">Mapowanie wierzchołków grafu h na wierzchołki grafu g (jeśli grafy nie są izomorficzne to null) - parametr wyjściowy</param>
    /// <returns>Informacja, czy grafy g i h są izomorficzne</returns>
    /// <remarks>
    /// 1) Uwzględniamy wagi krawędzi
    /// 3) Nie wolno modyfikować badanych grafów.
    /// </remarks>
    public static bool IsomorphismTest(this Graph<int> g, Graph<int> h, out int[] map)
    {
        if (h.VertexCount != g.VertexCount || h.EdgeCount != g.EdgeCount)
        {
            map = null;
            return false;
        }
        int n = h.VertexCount;
        bool[] used = new bool[n];
        int[] mapped = new int[n];
        bool IsomorphicCheck(int k)
        {
            if (k == n) return true;
            for (int i = 0; i < n; i++)
            {
                if (!used[i])
                {
                    bool isValid = true;

                    if (h.Degree(k) != g.Degree(i))continue;
                    
                        for (int v = 0; v < k; v++)
                        {
                            int mappedG = mapped[v];
                            bool edgeH = h.HasEdge(v, k);
                            bool edgeG = g.HasEdge(mappedG, i);
                            if (edgeH != edgeG) 
                            {
                                isValid = false; break;
                            }

                            if (edgeH && edgeG)
                            {
                                int v_k = h.GetEdgeWeight(v, k);
                                int mappedG_i = g.GetEdgeWeight(mappedG, i);
                                if (v_k != mappedG_i)
                                {
                                    isValid = false;
                                    break;
                                }
                            }
                        }
                    
                    if (isValid)
                    {
                        mapped[k] = i;
                        used[i] = true;
                        if (IsomorphicCheck(k + 1))
                        {
                            return true;
                        }
                        used[i] = false;
                    }
                }

              
            }

           
            return false;
        }
        if (IsomorphicCheck(0))
        {
            map = mapped;
            return true;
        }

        map = null;
        return false;
    }

}

