using System;
using ASD.Graphs;
using ASD;
using System.Collections.Generic;

namespace ASD
{

    public class Lab04 : System.MarshalByRefObject
    {
        /// <summary>
        /// Etap 1 - szukanie trasy z miasta start_v do miasta end_v, startując w dniu day
        /// </summary>
        /// <param name="g">Ważony graf skierowany będący mapą</param>
        /// <param name="start_v">Indeks wierzchołka odpowiadającego miastu startowemu</param>
        /// <param name="end_v">Indeks wierzchołka odpowiadającego miastu docelowemu</param>
        /// <param name="day">Dzień startu (w tym dniu należy wyruszyć z miasta startowego)</param>
        /// <param name="days_number">Liczba dni uwzględnionych w rozkładzie (tzn. wagi krawędzi są z przedziału [0, days_number-1])</param>
        /// <returns>(result, route) - result ma wartość true gdy podróż jest możliwa, wpp. false, 
        /// route to tablica z indeksami kolejno odwiedzanych miast (pierwszy indeks to indeks miasta startowego, ostatni to indeks miasta docelowego),
        /// jeżeli result == false to route ustawiamy na null</returns>
        public (bool result, int[] route) Lab04_FindRoute(DiGraph<int> g, int start_v, int end_v, int day, int days_number)
        {
            int n = g.VertexCount;
            int g_new_n  = n * days_number;
            DiGraph<int> g_new = new DiGraph<int>(g_new_n);
            for (int i = 0; i < g.VertexCount; i++)
            {
                foreach (var e in g.OutEdges(i))
                {
                    int next_day = (e.Weight + 1) % days_number;
                    g_new.AddEdge(i*days_number+e.Weight, e.To*days_number + next_day );
                }
            }
            bool[] visited = new bool[g_new_n];
            int [] parents = new int[g_new_n];
            for (int i = 0; i < g_new_n; i++)
            {
                parents[i] = -1;
            }
            Queue<int> queue = new Queue<int>();
            int startNode = start_v*days_number + day;
            queue.Enqueue(startNode);
            visited[startNode] = true;
            int endNode = -1;
            while (queue.Count > 0)
            {
                int currentNode = queue.Dequeue();
                int current_v = currentNode/days_number;
                if (current_v == end_v)
                {
                    endNode = currentNode;
                }
                foreach (var e in g_new.OutEdges(currentNode))
                {
                    if (visited[e.To] == false)
                    {
                        queue.Enqueue(e.To);
                        visited[e.To] = true;
                        parents[e.To] = currentNode;
                    }
                }
            }

            if (endNode == -1)
            {
                return (false, null);
            }
           List<int> path = new List<int>();
           int currPathNode =  endNode;
           while (currPathNode != -1)
           {
               int realNode = currPathNode / days_number;
               path.Add(realNode);
               currPathNode = parents[currPathNode];
           }
           path.Reverse();
           return (true, path.ToArray());
        }

        /// <summary>
        /// Etap 2 - szukanie trasy z jednego z miast z tablicy start_v do jednego z miast z tablicy end_v (startować można w dowolnym dniu)
        /// </summary>
        /// <param name="g">Ważony graf skierowany będący mapą</param>
        /// <param name="start_v">Tablica z indeksami wierzchołków startowych (trasę trzeba zacząć w jednym z nich)</param>
        /// <param name="end_v">Tablica z indeksami wierzchołków docelowych (trasę trzeba zakończyć w jednym z nich)</param>
        /// <param name="days_number">Liczba dni uwzględnionych w rozkładzie (tzn. wagi krawędzi są z przedziału [0, days_number-1])</param>
        /// <returns>(result, route) - result ma wartość true gdy podróż jest możliwa, wpp. false, 
        /// route to tablica z indeksami kolejno odwiedzanych miast (pierwszy indeks to indeks miasta startowego, ostatni to indeks miasta docelowego),
        /// jeżeli result == false to route ustawiamy na null</returns>
        public (bool result, int[] route) Lab04_FindRouteSets(DiGraph<int> g, int[] start_v, int[] end_v, int days_number)
        {
            int n = g.VertexCount;
            int g_new_n  = n * days_number;
            DiGraph<int> g_new = new DiGraph<int>(g_new_n);
            for (int i = 0; i < g.VertexCount; i++)
            {
                foreach (var e in g.OutEdges(i))
                {
                    int next_day = (e.Weight + 1) % days_number;
                    g_new.AddEdge(i*days_number+e.Weight, e.To*days_number + next_day );
                }
            }
            bool[] visited = new bool[g_new_n];
            int [] parents = new int[g_new_n];
            for (int i = 0; i < g_new_n; i++)
            {
                parents[i] = -1;
            }
            Queue<int> queue = new Queue<int>();
            foreach (int v in start_v)
            {
                for (int i = 0; i < days_number; i++)
                {
                    queue.Enqueue(v*days_number + i);
                    visited[v*days_number+i] = true;
                }
            }
            int endNode = -1;
            HashSet<int> endNodes = new HashSet<int>(end_v);
            while (queue.Count > 0)
            {
                int currentNode = queue.Dequeue();
                int current_v = currentNode/days_number;
                if (endNodes.Contains(current_v))
                {
                    endNode = currentNode;
                    break;
                }
                foreach (var e in g_new.OutEdges(currentNode))
                {
                    if (visited[e.To] == false)
                    {
                        queue.Enqueue(e.To);
                        visited[e.To] = true;
                        parents[e.To] = currentNode;
                    }
                }
            }

            if (endNode == -1)
            {
                return (false, null);
            }
            List<int> path = new List<int>();
            int currPathNode =  endNode;
            while (currPathNode != -1)
            {
                int realNode = currPathNode / days_number;
                path.Add(realNode);
                currPathNode = parents[currPathNode];
            }
            path.Reverse();
            return (true, path.ToArray());
        }
    }
}
