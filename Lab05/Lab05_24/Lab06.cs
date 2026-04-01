using ASD.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASD
{
	public class Lab06 : MarshalByRefObject
	{
		/// <summary>Etap 1</summary>
		/// <param name="n">Liczba kolorów (równa liczbie wierzchołków w c)</param>
		/// <param name="c">Graf opisujący możliwe przejścia między kolorami. Waga to wysiłek.</param>
		/// <param name="g">Graf opisujący drogi w mieście. Waga to kolor drogi.</param>
		/// <param name="target">Wierzchołek docelowy (dom Grzesia).</param>
		/// <param name="start">Wierzchołek startowy (wejście z lasu).</param>
		/// <returns>Pierwszy element pary to informacja, czy rozwiązanie istnieje. Drugi element pary, to droga będąca rozwiązaniem: sekwencja odwiedzanych wierzchołków (pierwszy musi być start, ostatni target). W przypadku, gdy nie ma rozwiązania, ma być tablica o długości 0.</returns>
		public (bool possible, int[] path) Stage1(
			int n, DiGraph<int> c, Graph<int> g, int target, int start
		)
		{
			bool[,] colorChanges = new bool[n, n];
			for (int i = 0; i < n; i++)
			{
				colorChanges[i, i] = true;
				foreach (var edge in c.OutEdges(i))
				{
					colorChanges[i, edge.To] = true;
				}
			}

			int numVertices = g.VertexCount;
			bool[,] visited = new bool[numVertices, n + 1];
			(int u, int c)[,] parent = new (int u, int c)[numVertices, n + 1];
			Queue<(int u, int c)> q = new Queue<(int u, int c)>();
			q.Enqueue((start, n));
			visited[start, n] = true;

			while (q.Count > 0)
			{
				var current = q.Dequeue();
				int u  = current.u;
				int current_color = current.c;
				if (u == target)
				{
					List<int> path = new List<int>();
					int last_u = u;
					int last_c = current_color;

					while (last_c != n)
					{
						path.Add(last_u);
						var p = parent[last_u, last_c];
						last_u = p.u;
						last_c = p.c;
					}
					path.Add(start);
					path.Reverse();

					return (true, path.ToArray());
				}

				foreach (var edge in g.OutEdges(u) )
				{
					int v  = edge.To;
					int next_color = edge.Weight;
					bool canChange = false;
					if (current_color == n)
					{
						canChange = true;
					}
					else if (colorChanges[current_color, next_color])
					{
						canChange = true;
					}

					if (canChange && !visited[v, next_color])
					{
						visited[v, next_color] = true;
						parent[v, next_color] = (u, current_color);
						q.Enqueue((v, next_color));
					}
				}
			}
			
		    return (false, new int[0]);
		}

		/// <summary>Drugi etap</summary>
		/// <param name="n">Liczba kolorów (równa liczbie wierzchołków w c)</param>
		/// <param name="c">Graf opisujący możliwe przejścia między kolorami. Waga to wysiłek.</param>
		/// <param name="g">Graf opisujący drogi w mieście. Waga to kolor drogi.</param>
		/// <param name="target">Wierzchołek docelowy (dom Grzesia).</param>
		/// <param name="starts">Wierzchołki startowe (wejścia z lasu).</param>
		/// <returns>Pierwszy element pary to koszt najlepszego rozwiązania lub null, gdy rozwiązanie nie istnieje. Drugi element pary, tak jak w etapie 1, to droga będąca rozwiązaniem: sekwencja odwiedzanych wierzchołków (pierwszy musi być start, ostatni target). W przypadku, gdy nie ma rozwiązania, ma być tablica o długości 0.</returns>
		public (int? cost, int[] path) Stage2(
			int n, DiGraph<int> c, Graph<int> g, int target, int[] starts
		)
		{
			// todo
		    return (null, new int[0]);
		}
	}
}
