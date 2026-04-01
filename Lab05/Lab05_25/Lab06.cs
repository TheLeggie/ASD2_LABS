using ASD.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASD
{
    public class Lab06 : MarshalByRefObject
    {
        /// <summary>Etap I</summary>
        /// <param name="G">Graf opisujący połączenia szlakami turystycznymi z podanym czasem przejścia krawędzi w wadze.</param>
        /// <param name="waitTime">Czas oczekiwania Studenta-Podróżnika w danym wierzchołku.</param>
        /// <param name="s">Wierzchołek startowy (początek trasy).</param>
        /// <returns>Pierwszy element krotki to wierzchołek końcowy szukanej trasy. Drugi element to długość trasy w minutach. Trzeci element to droga będąca rozwiązaniem: sekwencja odwiedzanych wierzchołków (zawierająca zarówno wierzchołek początkowy, jak i końcowy).</returns>
        public (int t, int l, int[] path) Stage1(DiGraph<int> G, int[] waitTime, int s)
        {
            int n = G.VertexCount;
            int[] dist = new int[n];
            int[] parent =  new int[n];

            for (int i = 0; i < n; i++)
            {
                dist[i] = int.MaxValue;
                parent[i] = -1;
            }
            dist[s] = 0;
            
            PriorityQueue<int,int> pq = new PriorityQueue<int, int>();
            pq.Insert(s, 0);
            while (pq.Count > 0)
            {
                int u = pq.Extract();
                foreach (var edge in G.OutEdges(u))
                {
                    int v = edge.To;
                    
                    int currWaitTime = (u == s) ? 0: waitTime[u];
                    int newDist = dist[u] + edge.Weight + currWaitTime;
                    if (newDist < dist[v])
                    {
                        dist[v] = newDist;
                        parent[v] = u;
                        pq.Insert(v, newDist);
                    }

                }
            }

            int bestVertex = s;
            int maxDist = 0;
            for (int i = 0; i < n; i++)
            {
                if (dist[i] != int.MaxValue && dist[i] > maxDist)
                {
                    maxDist = dist[i];
                    bestVertex = i;
                }
            }
            List<int> path = new List<int>();
            int current = bestVertex;
            while (current != -1)
            {
                path.Add(current);
                current = parent[current];
            }
            path.Reverse();
            return (bestVertex,maxDist,path.ToArray());
        }

        /// <summary>Etap II</summary>
        /// <param name="G">Graf opisujący połączenia szlakami turystycznymi z podanym czasem przejścia krawędzi w wadze.</param>
        /// <param name="C">Graf opisujący koszty przejścia krawędziami w grafie G.</param>
        /// <param name="waitTime">Czas oczekiwania Studenta-Podróżnika w danym wierzchołku.</param>
        /// <param name="s">Wierzchołek startowy (początek trasy).</param>
        /// <param name="t">Wierzchołek końcowy (koniec trasy).</param>
        /// <returns>Pierwszy element krotki to długość trasy w minutach. Drugi element to koszt przebycia trasy w złotych. Trzeci element to droga będąca rozwiązaniem: sekwencja odwiedzanych wierzchołków (zawierająca zarówno wierzchołek początkowy, jak i końcowy). Jeśli szukana trasa nie istnieje, funkcja zwraca `null`.</returns>
        public (int l, int c, int[] path)? Stage2(DiGraph<int> G, Graph<int> C, int[] waitTime, int s, int t)
        {
            // zapakuj cost i time w long i daj jako priority do PriorityQueue
            return null;
        }
    }
}