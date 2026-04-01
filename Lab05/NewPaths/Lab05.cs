using System.Linq;

namespace ASD
{
    using ASD.Graphs;
    using System;
    using System.Collections.Generic;

    public class Lab06 : System.MarshalByRefObject
    {
        public List<int> WidePath(DiGraph<int> G, int start, int end)
        {
            int n = G.VertexCount;
            int[] width = new int[n];
            for (int i = 0; i < n; i++)
            {
                width[i] = -1;
            }
            width[start] = int.MaxValue;
            int[] parent = new int[n];
            PriorityQueue<int,int> q = new PriorityQueue<int,int>();
            q.Insert(start, -int.MaxValue);
            while (q.Count > 0)
            {
                int u = q.Extract();
                if (u == end) break;
                foreach (var edge in G.OutEdges(u))
                {
                    int v = edge.To;
                    int newWidth = Math.Min(width[u], edge.Weight);
                    if (newWidth > width[v])
                    {
                        width[v] = newWidth;
                        parent[v] = u;
                        q.Insert(v, -newWidth);
                    }
                }
            }
            var path  = new List<int>();
            if (width[end] == -1) return path;
            int current = end;
            while (current != start)
            {
                path.Add(current);
                current = parent[current];
            }
            path.Add(start);
            path.Reverse();
            return path;
        }


        public List<int> WeightedWidePath(DiGraph<int> G, int start, int end, int[] weights, int maxWeight)
        {
            int n = G.VertexCount;
            HashSet<int> distinctWidths = new HashSet<int>();
            for (int i = 0; i < n; i++)
            {
                foreach (var edge in G.OutEdges(i))
                {
                    distinctWidths.Add(edge.Weight);
                }
            }
            int bestDiff = int.MinValue;
            List<int> bestPath = new List<int>();

            foreach (int  w in distinctWidths)
            {
                int[] dist = new int[n];
                int[] parent = new int[n];
                int[] edgeWidthToParent = new int[n];

                for (int i = 0; i < n; i++)
                {
                    dist[i] = int.MaxValue;
                    parent[i] = -1;
                }
                dist[start] = weights[start];

                PriorityQueue<int,int> q =  new PriorityQueue<int,int>();
                q.Insert(start, dist[start]);

                while (q.Count > 0)
                {
                    int u = q.Extract();
                    if (u == end) break;
                    foreach (var edge in G.OutEdges(u))
                    {
                        if(edge.Weight < w) continue;
                        int v = edge.To;
                        int newDist = dist[u] + weights[v];
                        if (newDist < dist[v])
                        {
                            dist[v] = newDist;
                            parent[v] = u;
                            edgeWidthToParent[v] = edge.Weight;
                            q.Insert(v, newDist);
                        }
                    }
                }

                if (dist[end] != int.MaxValue)
                {
                    int minEdge = int.MaxValue;
                    int current = end;
                    List<int> currPath = new List<int>();
                    while (current != start)
                    {
                        currPath.Add(current);
                        minEdge = Math.Min(minEdge, edgeWidthToParent[current]);
                        current = parent[current];
                    }
                    currPath.Add(start);
                    currPath.Reverse();
                    
                    int diff = minEdge - dist[end];

                    if (diff > bestDiff)
                    {
                        bestDiff = diff;
                        bestPath = currPath;
                    }
                }
                
            }
            return bestPath;
        }
    }
}