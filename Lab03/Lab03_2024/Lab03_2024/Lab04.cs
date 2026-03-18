using System;
using ASD.Graphs;
using System.Collections.Generic;

namespace ASD
{
    public class Lab04 : MarshalByRefObject
    {
        /// <summary>
        /// Etap 1 - Szukanie mozliwych do odwiedzenia miast z grafu skierowanego
        /// przy zalozeniu, ze pociagi odjezdzaja co godzine.
        /// </summary>
        /// <param name="graph">Graf skierowany przedstawiający siatke pociagow</param>
        /// <param name="miastoStartowe">Numer miasta z ktorego zaczyna sie podroz pociagiem</param>
        /// <param name="K">Godzina o ktorej musi zakonczyc sie nasza podroz</param>
        /// <returns>Tablica numerow miast ktore mozna odwiedzic. Posortowana rosnaco.</returns>
        public int[] Lab04Stage1(DiGraph graph, int miastoStartowe, int K)
        {
            int n = graph.VertexCount;
            int[] dist = new int[n];
            for (int i = 0; i < n; i++) dist[i] = int.MaxValue;
            
            dist[miastoStartowe] = 0;
            int maxGodzin = K - 8;
            
            Queue<int> q = new Queue<int>();
            q.Enqueue(miastoStartowe);
            
            while (q.Count > 0)
            {
                int u = q.Dequeue();
        
                if (dist[u] == maxGodzin) continue;
        
                foreach (int v in graph.OutNeighbors(u))
                {
                    if (dist[v] == int.MaxValue)
                    {
                        dist[v] = dist[u] + 1;
                        q.Enqueue(v);
                    }
                }
            }
            List<int> result = new List<int>();
            for (int i = 0; i < n; i++)
            {
                if (dist[i] <= maxGodzin) result.Add(i);
            }
    
            return result.ToArray();

        }

        /// <summary>
        /// Etap 2 - Szukanie mozliwych do odwiedzenia miast z grafu skierowanego.
        /// Waga krawedzi oznacza, ze pociag rusza o tej godzinie
        /// </summary>
        /// <param name="graph">Wazony graf skierowany przedstawiający siatke pociagow</param>
        /// <param name="miastoStartowe">Numer miasta z ktorego zaczyna sie podroz pociagiem</param>
        /// <param name="K">Godzina o ktorej musi zakonczyc sie nasza podroz</param>
        /// <returns>Tablica numerow miast ktore mozna odwiedzic. Posortowana rosnaco.</returns>
        public int[] Lab04Stage2(DiGraph<int> graph, int miastoStartowe, int K)
        {
            int n = graph.VertexCount;
            int[] minArrival = new int[n];
            for (int i = 0; i < n; i++) minArrival[i] = int.MaxValue;
    
            minArrival[miastoStartowe] = 8;
    
            Queue<int> q = new Queue<int>();
            bool[] inQueue = new bool[n];
    
            q.Enqueue(miastoStartowe);
            inQueue[miastoStartowe] = true;
    
            while (q.Count > 0)
            {
                int u = q.Dequeue();
                inQueue[u] = false;
        
                foreach (var e in graph.OutEdges(u))
                {
                    int departureTime = e.Weight; 
            
                    if (minArrival[u] <= departureTime && departureTime + 1 <= K)
                    {
                        int arrivalTime = departureTime + 1;
                
                        if (arrivalTime < minArrival[e.To])
                        {
                            minArrival[e.To] = arrivalTime;
                    
                            if (!inQueue[e.To])
                            {
                                q.Enqueue(e.To);
                                inQueue[e.To] = true;
                            }
                        }
                    }
                }
            }
    
            List<int> result = new List<int>();
            for (int i = 0; i < n; i++)
            {
                if (minArrival[i] <= K) result.Add(i);
            }
    
            return result.ToArray();
        }
    }
}
