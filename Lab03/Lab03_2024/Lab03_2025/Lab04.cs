using ASD.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASD
{
    public class Lab04 : MarshalByRefObject
    {
        /// <summary>
        /// Etap 1 - Wyznaczenie liczby oraz listy zainfekowanych serwisów po upływie K dni.
        /// Algorytm analizuje propagację infekcji w grafie i zwraca wszystkie dotknięte nią serwisy.
        /// </summary>
        /// <param name="G">Graf reprezentujący infrastrukturę serwisów.</param>
        /// <param name="K">Liczba dni propagacji infekcji.</param>
        /// <param name="s">Indeks początkowo zainfekowanego serwisu.</param>
        /// <returns>
        /// (int numberOfInfectedServices, int[] listOfInfectedServices) - 
        /// numberOfInfectedServices: liczba zainfekowanych serwisów,
        /// listOfInfectedServices: tablica zawierająca numery zainfekowanych serwisów w kolejności rosnącej.
        /// </returns>
        public (int numberOfInfectedServices, int[] listOfInfectedServices) Stage1(Graph G, int K, int s)
        {
            int n = G.VertexCount;
            int[] dist = new int[n];
            
            for (int i = 0; i < n; i++) dist[i] = int.MaxValue;

            dist[s] = 0;
            Queue<int> q = new Queue<int>();
            q.Enqueue(s);

            while (q.Count > 0)
            {
                int u = q.Dequeue();
                
                if (dist[u] == K) continue;

                foreach (int v in G.OutNeighbors(u))
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
                if (dist[i] <= K) result.Add(i);
            }

            return (result.Count, result.ToArray());
        }

        /// <summary>
        /// Etap 2 - Wyznaczenie liczby oraz listy zainfekowanych serwisów przy uwzględnieniu wyłączeń.
        /// Algorytm analizuje propagację infekcji z możliwością wcześniejszego wyłączania serwisów.
        /// </summary>
        /// <param name="G">Graf reprezentujący infrastrukturę serwisów.</param>
        /// <param name="K">Liczba dni propagacji infekcji.</param>
        /// <param name="s">Tablica początkowo zainfekowanych serwisów.</param>
        /// <param name="serviceTurnoffDay">Tablica zawierająca dzień, w którym dany serwis został wyłączony (K + 1 oznacza brak wyłączenia).</param>
        /// <returns>
        /// (int numberOfInfectedServices, int[] listOfInfectedServices) - 
        /// numberOfInfectedServices: liczba zainfekowanych serwisów,
        /// listOfInfectedServices: tablica zawierająca numery zainfekowanych serwisów w kolejności rosnącej.
        /// </returns>
        public (int numberOfInfectedServices, int[] listOfInfectedServices) Stage2(Graph G, int K, int[] s, int[] serviceTurnoffDay)
        {
            int n = G.VertexCount;
            int[] dist = new int[n];
            for (int i = 0; i < n; i++) dist[i] = int.MaxValue;

            Queue<int> q = new Queue<int>();
            
            foreach (int startNode in s)
            {
                if (0 < serviceTurnoffDay[startNode])
                {
                    dist[startNode] = 0;
                    q.Enqueue(startNode);
                }
            }

            while (q.Count > 0)
            {
                int u = q.Dequeue();
                if (dist[u] == K) continue;

                foreach (int v in G.OutNeighbors(u))
                {
                    int nextTime = dist[u] + 1;
                    
                  
                    if (dist[v] == int.MaxValue && nextTime < serviceTurnoffDay[v])
                    {
                        dist[v] = nextTime;
                        q.Enqueue(v);
                    }
                }
            }

            List<int> result = new List<int>();
            for (int i = 0; i < n; i++)
            {
                if (dist[i] <= K) result.Add(i);
            }

            return (result.Count, result.ToArray());
        }

        /// <summary>
        /// Etap 3 - Wyznaczenie liczby oraz listy zainfekowanych serwisów z możliwością ponownego włączenia wyłączonych serwisów.
        /// Algorytm analizuje propagację infekcji uwzględniając serwisy, które mogą być ponownie uruchamiane po określonym czasie.
        /// </summary>
        /// <param name="G">Graf reprezentujący infrastrukturę serwisów.</param>
        /// <param name="K">Liczba dni propagacji infekcji.</param>
        /// <param name="s">Tablica początkowo zainfekowanych serwisów.</param>
        /// <param name="serviceTurnoffDay">Tablica zawierająca dzień, w którym dany serwis został wyłączony (K + 1 oznacza brak wyłączenia).</param>
        /// <param name="serviceTurnonDay">Tablica zawierająca dzień, w którym dany serwis został ponownie włączony.</param>
        /// <returns>
        /// (int numberOfInfectedServices, int[] listOfInfectedServices) - 
        /// numberOfInfectedServices: liczba zainfekowanych serwisów,
        /// listOfInfectedServices: tablica zawierająca numery zainfekowanych serwisów w kolejności rosnącej.
        /// </returns>
        public (int numberOfInfectedServices, int[] listOfInfectedServices) Stage3(Graph G, int K, int[] s, int[] serviceTurnoffDay, int[] serviceTurnonDay)
        {
            int n = G.VertexCount;
            int[] dist = new int[n];
            for (int i = 0; i < n; i++) dist[i] = int.MaxValue;

            Queue<int>[] buckets = new Queue<int>[K + 1];
            for (int i = 0; i <= K; i++) buckets[i] = new Queue<int>();

            foreach (int startNode in s)
            {
                int arrival = 0;
                
                if (arrival >= serviceTurnoffDay[startNode] && arrival < serviceTurnonDay[startNode])
                {
                    arrival = serviceTurnonDay[startNode];
                }

                if (arrival <= K && arrival < dist[startNode])
                {
                    dist[startNode] = arrival;
                    buckets[arrival].Enqueue(startNode);
                }
            }

            for (int d = 0; d <= K; d++)
            {
                while (buckets[d].Count > 0)
                {
                    int u = buckets[d].Dequeue();
                    
                    if (dist[u] < d) continue;
                    
                    if (d == K) continue; 

                    foreach (int v in G.OutNeighbors(u))
                    {
                        int nextTime = d + 1;
                        
                        if (nextTime >= serviceTurnoffDay[v] && nextTime < serviceTurnonDay[v])
                        {
                            nextTime = serviceTurnonDay[v];
                        }

                        if (nextTime <= K && nextTime < dist[v])
                        {
                            dist[v] = nextTime;
                            buckets[nextTime].Enqueue(v);
                        }
                    }
                }
            }

            List<int> result = new List<int>();
            for (int i = 0; i < n; i++)
            {
                if (dist[i] <= K) result.Add(i);
            }

            return (result.Count, result.ToArray());
        }
        
    }
}
