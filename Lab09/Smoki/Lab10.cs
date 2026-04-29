using System;
using System.Linq;
using System.Collections.Generic;
using ASD.Graphs;

namespace ASD
{
    public class Lab10 : MarshalByRefObject
    {

        /// <param name="labyrinth">Graf reprezentujący labirynt</param>
        /// <param name="startingTorches">Ilość pochodni z jaką startują bohaterowie</param>
        /// <param name="roomTorches">Ilość pochodni w poszczególnych pokojach</param>
        /// <param name="debt>">Ilość złota jaką bohaterowie muszą zebrać</param>
        /// <param name="roomGold">Ilość złota w poszczególnych pokojach</param>
        /// <returns>Informację czy istnieje droga przez labirytn oraz tablicę reprezentującą kolejne wierzchołki na drodze. W przypadku, gdy zwracany jest false, wartość tego pola powinna być null.</returns>
        public (bool routeExists, int[] route) FindEscape(Graph labyrinth, int startingTorches, int[] roomTorches, int debt, int[] roomGold)
        {
            bool[] visited = new bool[labyrinth.VertexCount];
            List<int> path = new List<int>();


            int goldSum = 0;
            visited[0] = true;
            path.Add(0);
            bool found = FindEscapeRec(labyrinth, 0, ref path, ref visited, ref startingTorches, roomTorches,
                ref goldSum, debt, roomGold);

            if (found)
            {
                int[] route = path.ToArray();
                return (true, route);
            }
            else
            {
                return (false, null);
            }

        }

        private bool FindEscapeRec(Graph labyrinth, int i, ref List<int> path, ref bool[] visited,
            ref int startingTorches, int[] roomTorches, ref int goldSum, int debt, int[] roomGold)
        {

            // jesli w pokoju jest zloto to je zabieramy
            if (roomGold[i] > 0)
            {
                goldSum += roomGold[i];
            }

            // jesli w tym pokoju sa pochodnie to zbieramy 
            if (roomTorches[i] > 0)
            {
                startingTorches += roomTorches[i];
            }

            // jesli jestesmy w ostatnim pokoju 
            if (i == labyrinth.VertexCount - 1)
            {
                // czy mamy wystarczajaca ilosc zlota i pochodni
                if (goldSum >= debt)
                {
                  return true;
                }
            }
            else  
            {
                foreach (int nextRoom in labyrinth.OutNeighbors(i))
                {

                    // jesli nie bylismy jeszcze w tym pokoju
                    if (!visited[nextRoom] && startingTorches > 0)
                    {

                        // wchodzimy wiec zuzywamy jedna pochodnie
                        visited[nextRoom] = true;
                        startingTorches--;
                        path.Add(nextRoom);
                        if (FindEscapeRec(labyrinth, nextRoom, ref path, ref visited, ref startingTorches, roomTorches,
                                ref goldSum, debt, roomGold))
                        {
                            return true;
                        }

                        path.Remove(nextRoom);
                        startingTorches++;
                        visited[nextRoom] = false;
                    }
                }


            }
            
            if (roomGold[i] > 0)
            {
                goldSum -= roomGold[i];
            }

            if (roomTorches[i] > 0)
            {
                startingTorches -= roomTorches[i];
            }

            return false;
        }

        /// <param name="labyrinth">Graf reprezentujący labirynt</param>
        /// <param name="startingTorches">Ilość pochodni z jaką startują bohaterowie</param>
        /// <param name="roomTorches">Ilość pochodni w poszczególnych pokojach</param>
        /// <param name="debt">Ilość złota jaką bohaterowie muszą zebrać</param>
        /// <param name="roomGold">Ilość złota w poszczególnych pokojach</param>
        /// <param name="dragonDelay">Opóźnienie z jakim wystartuje smok</param>
        /// <returns>Informację czy istnieje droga przez labirynt oraz tablicę reprezentującą kolejne wierzchołki na drodze. W przypadku, gdy zwracany jest false, wartość tego pola powinna być null.</returns>

        public (bool routeExists, int[] route) FindEscapeWithHeadstart(Graph labyrinth, int startingTorches, int[] roomTorches, int debt, int[] roomGold, int dragonDelay)
        {
            return (false, null);
        }
        
        public (bool routeExists, int[] route) PlanDelivery(Graph<int> railway, int[] eggDemand, int truckCapacity,
        int tankEngineRange, bool[] isRefuelStation, bool anySolution)
    {
        int n = railway.VertexCount;
        List<int> S = [0];
        int curEggs = truckCapacity;
        int curSteam = tankEngineRange;
        int curTime = 0;
        List<int> bestS = [];
        int bestTime = int.MaxValue;

        bool possible = false;

        bool[] delivered = new bool[n];
        int ndelivered = 0;

        void FindPath(int last)
        {
            if (last == 0 && ndelivered == n - 1)
            {
                if (curTime < bestTime)
                {
                    bestS = S[..];
                    bestTime = curTime;
                }

                possible = true;
                return;
            }

            foreach (var e in railway.OutEdges(last))
            {
                if (delivered[e.To] || curTime + e.Weight > bestTime // czy mieścimy się w lepszym czasie
                                    || curSteam < e.Weight // czy starczy zasięgu parowozu
                                    || curEggs < eggDemand[e.To]) // każda stacja tylko raz -> trzeba rozwieźć od razu
                    continue;

                S.Add(e.To);
                if (e.To != 0)
                {
                    delivered[e.To] = true;
                    ndelivered++;
                }

                curTime += e.Weight;

                int lastSteam = curSteam;
                if (isRefuelStation[e.To])
                    curSteam = tankEngineRange;
                else
                    curSteam -= e.Weight;

                int lastEggs = curEggs;
                if (e.To == 0)
                    curEggs = truckCapacity;
                else
                    curEggs -= eggDemand[e.To];

                FindPath(e.To);
                if (possible && anySolution)
                    return;

                if (e.To != 0)
                {
                    delivered[e.To] = false;
                    ndelivered--;
                }

                curTime -= e.Weight;
                curSteam = lastSteam;
                curEggs = lastEggs;
                S.RemoveAt(S.Count - 1);
            }
        }

        FindPath(0);

        return (possible, bestS.ToArray());
    }
    }
}
