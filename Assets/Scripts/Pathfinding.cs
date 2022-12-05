using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Pathfinding
{
	enum State
	{
		Unavailable,
		Available,
		Visited,
		Path
	}

	static Vector2Int[] directions = { Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down };

	public static int yOffset = 4;
	public static int xOffset = 4;
	private static State[,] grid;


	// Esta función se puede optimizar pero por ahora se queda así, si eso en la versión final se pueden tocar un par de cosita y hacerla mucho más rápida.
	public static List<Vector2Int> GetPath(Transform tOrigin, Transform tDestination, Tile[] tiles)
	{

		grid = new State[17, 17];

		Vector2Int origin = new Vector2Int(Mathf.RoundToInt(tOrigin.position.x + xOffset), Mathf.RoundToInt(tOrigin.position.y + yOffset));
		Vector2Int destination = new Vector2Int(Mathf.RoundToInt(tDestination.localPosition.x + xOffset), Mathf.RoundToInt(tDestination.localPosition.y + yOffset));


		// INIT GRID
		for (int i = 0; i < tiles.Length; i++)
		{
			Tile tile = tiles[i];

			int row = (int)tile.transform.localPosition.x + xOffset;
			int col = (int)tile.transform.localPosition.y + yOffset;

			if (tile.isWalkable)
				grid[row, col] = State.Available;
			else
				grid[row, col] = State.Unavailable;

		}


		List<Vector2Int> path = getPath(origin, destination, grid);

		//displayPath(path);

		return path;
	}

	// public static void HighlightTiles(Tile[] tiles) {
	// 	grid = new State[50,50];

	// 	// INIT GRID
	// 	for (int i = 0; i < tiles.Length; i++) {
	// 		Tile tile = tiles[i];

	// 		int row = (int) tile.transform.localPosition.x + xOffset;
	// 		int col = (int) tile.transform.localPosition.y + yOffset;

	// 		if (tile.isWalkable)
	// 			grid[row, col] = State.Available;
	// 		else
	// 			grid[row, col] = State.Unavailable;

	// 	}


	// 	List<Vector2Int> path = getPath(origin, destination, grid);

	// 	//displayPath(path);
	// }



	public static List<Tile> GetRealWalkableTiles(Transform tOrigin, ref Tile[] tiles)
	{

		// #C optimizable
		State[,] grid = new State[17, 17];


		// INIT GRID
		for (int i = 0; i < tiles.Length; i++)
		{
			Tile tile = tiles[i];

			int row = (int)tile.transform.localPosition.x + xOffset;
			int col = (int)tile.transform.localPosition.y + yOffset;

			if (tile.isWalkable)
				grid[row, col] = State.Available;
			else
				grid[row, col] = State.Unavailable;

		}


		Vector2Int origin = new Vector2Int(Mathf.RoundToInt(tOrigin.position.x + xOffset), Mathf.RoundToInt(tOrigin.position.y + yOffset));


		Queue<Vector2Int> toVisit = new Queue<Vector2Int>();
		toVisit.Enqueue(origin);
		grid[origin.x, origin.y] = State.Visited;


		Vector2Int[,] _grid = new Vector2Int[grid.GetLength(0), grid.GetLength(1)];

		Vector2Int cell;
		while (toVisit.Count > 0)
		{

			cell = toVisit.Dequeue();

			foreach (Vector2Int direction in directions)
			{

				Vector2Int target = cell + direction;

				if (!inBounds(ref target, ref grid))
					continue;

				if (grid[target.x, target.y] != State.Visited)
					_grid[target.x, target.y] = cell;

				// if (target == destination)
				// 	return getPath(ref origin, ref destination, ref _grid);


				if (grid[target.x, target.y] == State.Available)
				{
					grid[target.x, target.y] = State.Visited;
					toVisit.Enqueue(target);

				}

			}

		}

		List<Tile> walkableTiles = new List<Tile>();

		foreach (Tile tile in tiles)
		{
			int row = (int)tile.transform.localPosition.x + xOffset;
			int col = (int)tile.transform.localPosition.y + yOffset;

			if (grid[row, col] == State.Visited)
				walkableTiles.Add(tile);
		}

		return walkableTiles;

	}

	public static void HighlightTiles(Transform tOrigin, ref Tile[] tiles)
	{

		List<Tile> walkableTiles = GetRealWalkableTiles(tOrigin, ref tiles);

		foreach (Tile tile in walkableTiles)
			tile.Highlight();
	}

	public static Tile GetTileFromTransform(Transform target, ref Tile[] tiles)
	{
		int targetRow = (int)target.localPosition.x + xOffset;
		int targetCol = (int)target.localPosition.y + yOffset;



		foreach (Tile tile in tiles)
		{
			int tileRow = (int)tile.transform.localPosition.x + xOffset;
			int tileCol = (int)tile.transform.localPosition.y + yOffset;

			if (tileRow == targetRow && tileCol == targetCol)
				return tile;
		}

		return null;

	}


	private static List<Vector2Int> getPath(Vector2Int origin, Vector2Int destination, State[,] grid)
	{

		Queue<Vector2Int> toVisit = new Queue<Vector2Int>();
		toVisit.Enqueue(origin);
		grid[origin.x, origin.y] = State.Visited;


		Vector2Int[,] _grid = new Vector2Int[grid.GetLength(0), grid.GetLength(1)];


		Vector2Int cell;
		while (toVisit.Count > 0)
		{
			cell = toVisit.Dequeue();

			foreach (Vector2Int direction in directions)
			{

				Vector2Int target = cell + direction;

				if (!inBounds(ref target, ref grid))
					continue;

				if (grid[target.x, target.y] != State.Visited)
					_grid[target.x, target.y] = cell;

				if (target == destination)
					return getPath(ref origin, ref destination, ref _grid);


				if (grid[target.x, target.y] == State.Available)
				{
					grid[target.x, target.y] = State.Visited;
					toVisit.Enqueue(target);

				}

			}

		}

		return null;
	}

	private static List<Vector2Int> getPath(ref Vector2Int origin, ref Vector2Int destination, ref Vector2Int[,] _field)
	{

		List<Vector2Int> path = new List<Vector2Int>();

		for (Vector2Int cell = destination; cell != origin; cell = _field[cell.x, cell.y])
		{
			path.Add(new Vector2Int(cell.x - xOffset, cell.y - yOffset));
		}

		path.Reverse();
		return path;
	}

	/* Comprobamos que no nos salgamos de la matríz */
	private static bool inBounds(ref Vector2Int cell, ref State[,] field)
	{
		return cell.x >= 0 && cell.y >= 0 && cell.x < field.GetLength(0) && cell.y < field.GetLength(1);
	}

	private static void displayPath(List<Vector2Int> path)
	{
		foreach (Vector2Int v in path)
		{
			Debug.Log(v);
		}
	}

}
