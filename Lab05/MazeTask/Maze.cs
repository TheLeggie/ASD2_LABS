using ASD.Graphs;
using System;
using System.Linq;
using System.Text;

namespace ASD
{
    public class Maze : MarshalByRefObject
    {

        /// <summary>
        /// Wersje zadania I oraz II
        /// Zwraca najkrótszy możliwy czas przejścia przez labirynt bez dynamitów lub z dowolną ich liczbą
        /// </summary>
        /// <param name="maze">labirynt</param>
        /// <param name="withDynamite">informacja, czy dostępne są dynamity 
        /// Wersja I zadania -> withDynamites = false, Wersja II zadania -> withDynamites = true</param>
        /// <param name="path">zwracana ścieżka</param>
        /// <param name="t">czas zburzenia ściany (dotyczy tylko wersji II)</param> 
        public int FindShortestPath(char[,] maze, bool withDynamite, out string path, int t = 0)
        {
            path = "";
            int rows = maze.GetLength(0);
            int cols = maze.GetLength(1);

            int startIdx = 0;
            int endIdx = 0;

            DiGraph<int> g = new DiGraph<int>(rows * cols);

            var directions = new (int row, int col)[]
            {
                (-1, 0),
                (1, 0),
                (0, -1),
                (0, 1)
            };
            char[] geoDirections = new[] { 'W', 'E', 'N', 'S' };

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    int vFrom = i * cols + j;
                    if (maze[i, j] == 'S')
                    {
                        startIdx = i * cols + j;
                    }

                    if (maze[i, j] == 'E')
                    {
                        endIdx = i * cols + j;
                    }

                    for (int k = 0; k < 4; k++)
                    {
                        int new_row = directions[k].row + i;
                        int new_col = directions[k].col + j;

                        if (new_row < 0 || new_row >= rows || new_col < 0 || new_col >= cols)
                        {
                            continue;
                        }

                        int vTo = new_row * cols + new_col;
                        if (maze[i, j] != 'X')
                        {
                            g.AddEdge(vFrom, vTo, 1);
                        }

                        if (withDynamite)
                        {
                            if (maze[i, j] == 'X')
                            {
                                g.AddEdge(vFrom, vTo, t);
                            }
                        }

                    }
                }


            }

            PathsInfo<int> pi = Paths.Dijkstra(g, startIdx);
            if (pi.Reachable(startIdx, endIdx) == false)
            {
                path = string.Empty;
                return -1;
            }

            StringBuilder sb = new StringBuilder();
            int[] vertexPath = pi.GetPath(startIdx, endIdx);
            for (int i = 0; i < vertexPath.Length - 1; i++)
            {
                int u = vertexPath[i];
                int v = vertexPath[i + 1];

                int rFrom = u / cols;
                int cFrom = u % cols;
                int rTo = v / cols;
                int cTo = v % cols;

                if (rTo < rFrom) sb.Append('N');
                else if (rTo > rFrom) sb.Append('S');
                else if (cTo < cFrom) sb.Append('W');
                else if (cTo > cFrom) sb.Append('E');



            }

            path = sb.ToString();
            return pi.GetDistance(startIdx, endIdx);
        }

        /// <summary>
        /// Wersja III i IV zadania
        /// Zwraca najkrótszy możliwy czas przejścia przez labirynt z użyciem co najwyżej k lasek dynamitu
        /// </summary>
        /// <param name="maze">labirynt</param>
        /// <param name="k">liczba dostępnych lasek dynamitu, dla wersji III k=1</param>
        /// <param name="path">zwracana ścieżka</param>
        /// <param name="t">czas zburzenia ściany</param>
        public int FindShortestPathWithKDynamites(char[,] maze, int k, out string path, int t)
        {
            path = "";
            int rows = maze.GetLength(0);
            int cols = maze.GetLength(1);
            int n = rows * cols;
            int startIdx = 0;
            int endIdx = 0;

            DiGraph<int> g = new DiGraph<int>((k + 1) * rows * cols);

            var directions = new (int row, int col)[]
            {
                (-1, 0),
                (1, 0),
                (0, -1),
                (0, 1)
            };
            char[] geoDirections = new[] { 'W', 'E', 'N', 'S' };

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (maze[i, j] == 'S')
                    {
                        startIdx = i * cols + j;
                    }

                    if (maze[i, j] == 'E')
                    {
                        endIdx = i * cols + j;
                    }
                }
            }

            for (int layer = 0; layer <= k; layer++)
            {
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        int vFrom = i * cols + j;
                        int vFromLayer = layer * n + vFrom;
                        for (int l = 0; l < 4; l++)
                        {
                            int new_row = directions[l].row + i;
                            int new_col = directions[l].col + j;

                            if (new_row < 0 || new_row >= rows || new_col < 0 || new_col >= cols)
                            {
                                continue;
                            }

                            int vTo = new_row * cols + new_col;
                            if (maze[new_row, new_col] != 'X')
                            {
                                int vToLayer = layer * n + vTo;
                                g.AddEdge(vFromLayer, vToLayer, 1);
                            }
                            else if (maze[new_row, new_col] == 'X' && layer < k)
                            {
                                int vToNextLayer = (layer + 1) * n + vTo;
                                g.AddEdge(vFromLayer, vToNextLayer, t);
                            }
                        }

                    }
                }


            }
        
    

        PathsInfo<int> pi = Paths.Dijkstra(g, startIdx);
    
        int best_distance = int.MaxValue;
        int best_endIdx = -1;
        for (int layer = 0; layer <= k; layer++)
        {
            int endIdxLayer = layer * n + endIdx;
            if (pi.Reachable(startIdx, endIdxLayer))
            {
                int distance = pi.GetDistance(startIdx, endIdxLayer);
                if (distance < best_distance)
                {
                    best_distance = distance;
                    best_endIdx = endIdxLayer;
                }
            }
            
        }
                if (best_endIdx == -1)
                {
                    path = string.Empty;
                    return -1;
                }

                StringBuilder sb = new StringBuilder();
                int[] vertexPath = pi.GetPath(startIdx, best_endIdx);
                for(int i = 0; i < vertexPath.Length - 1; i++)
                {
                    int u = vertexPath[i] % n;
                    int v = vertexPath[i + 1] % n;

                    int rFrom = u / cols;
                    int cFrom = u % cols;
                    int rTo = v / cols;
                    int cTo = v % cols;

                    if (rTo < rFrom) sb.Append('N');
                    else if (rTo > rFrom) sb.Append('S');
                    else if (cTo < cFrom) sb.Append('W');
                    else if (cTo > cFrom) sb.Append('E');



                }
                path = sb.ToString();
                return best_distance;
        }
    }
}