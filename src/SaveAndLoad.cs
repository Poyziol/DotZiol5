using System;

namespace src
{
    [Serializable]
    public class SaveAndLoad
    {
        public List<List<int>> PointsPlaced { get; set; } = null!;
        public bool Player1Turn { get; set; }
        public List<Line> FormedLines { get; set; }

        // Constructor to convert 2D array to List<List<int>>
        public SaveAndLoad()
        {
            FormedLines = new List<Line>();
        }

        public void SetPointsPlaced(int[,] points)
        {
            PointsPlaced = new List<List<int>>();

            for (int i = 0; i < points.GetLength(0); i++)
            {
                List<int> row = new List<int>();
                for (int j = 0; j < points.GetLength(1); j++)
                {
                    row.Add(points[i, j]);
                }
                PointsPlaced.Add(row);
            }
        }

        public int[,] GetPointsPlaced()
        {
            int[,] points = new int[PointsPlaced.Count, PointsPlaced[0].Count];
            for (int i = 0; i < PointsPlaced.Count; i++)
            {
                for (int j = 0; j < PointsPlaced[i].Count; j++)
                {
                    points[i, j] = PointsPlaced[i][j];
                }
            }
            return points;
        }
    }

}