using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace src
{
    public partial class Dotgame : Form
    {
        private Panel gamePanel = null!;
        private bool player1Turn = true;
        private bool player2Turn = false;
        private const int gridSize = 13;
        private Point[,] gridPoints = null!;
        private int[,] pointsPlaced = null!; // 0 for empty, 1 for player1, 2 for player2
        private const int cellSize = 35;
        private const int pointRadius = 5;
        private Label turnLabel = null!; 
        private string defaultSaveDirectory = @"E:\ITU\Dotnet\Dotgame\save";
        private List<Line> formedLines = new List<Line>();
        private Button suggestionJ1Button = null!;
        private Button suggestionJ2Button = null!;
        private Point? suggestedPoint = null;

        public Dotgame()
        {
            InitializeComponent();
            InitializeGamePanel();
            InitializeMenu();
            InitializeSuggestionButtons();
            InitializeTurnLabel();

            this.FormClosing += new FormClosingEventHandler(Dotgame_FormClosing);
        }

        private void InitializeGamePanel()
        {
            this.gamePanel = new Panel();
            this.gamePanel.Location = new Point(50, 50);
            this.gamePanel.Size = new Size(gridSize * cellSize, gridSize * cellSize);
            this.gamePanel.BackColor = Color.White;
            this.Controls.Add(this.gamePanel);
            this.Size = new Size(800, 600);
            this.gamePanel.Paint += new PaintEventHandler(this.GamePanel_Paint);
            this.gamePanel.MouseClick += new MouseEventHandler(this.GamePanel_MouseClick);

            InitializeGridPoints();
        }

        private void InitializeMenu()
        {
            MenuStrip menuStrip = new MenuStrip();
            ToolStripMenuItem fileMenu = new ToolStripMenuItem("File");
            ToolStripMenuItem saveItem = new ToolStripMenuItem("Save", null, SaveGame);
            ToolStripMenuItem loadItem = new ToolStripMenuItem("Load", null, LoadGame);

            ToolStripMenuItem gameMenu = new ToolStripMenuItem("Game");
            ToolStripMenuItem resetItem = new ToolStripMenuItem("New Game", null, ResetGame);

            fileMenu.DropDownItems.Add(saveItem);
            fileMenu.DropDownItems.Add(loadItem);
            gameMenu.DropDownItems.Add(resetItem);
            menuStrip.Items.Add(fileMenu);
            menuStrip.Items.Add(gameMenu);
            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);
        }

        private void InitializeTurnLabel()
        {
            turnLabel = new Label();
            turnLabel.Text = "Tour : Joueur 1";
            turnLabel.Font = new Font("Arial", 14);
            turnLabel.Location = new Point(585, 50); 
            turnLabel.AutoSize = true;
            UpdateTurnLabel();
            this.Controls.Add(turnLabel);
        }

        private void UpdateTurnLabel()
        {
            if(player1Turn)
            {
                turnLabel.Text = "Tour: Joueur 1";
                turnLabel.ForeColor = Color.Blue; 
            }
            else
            {
                turnLabel.Text = "Tour: Joueur 2";
                turnLabel.ForeColor = Color.Red; 
            }
        }

        private void InitializeSuggestionButtons()
        {
            suggestionJ1Button = new Button();
            suggestionJ1Button.Text = "Suggestion J1";
            suggestionJ1Button.ForeColor = Color.Blue;
            suggestionJ1Button.Location = new Point(50, 500);
            suggestionJ1Button.Size = new Size(120, 40);
            suggestionJ1Button.Click += SuggestionJ1Button_Click;
            this.Controls.Add(suggestionJ1Button);

            suggestionJ2Button = new Button();
            suggestionJ2Button.Text = "Suggestion J2";
            suggestionJ2Button.ForeColor = Color.Red; 
            suggestionJ2Button.Location = new Point(386, 500);
            suggestionJ2Button.Size = new Size(120, 40);
            suggestionJ2Button.Click += SuggestionJ2Button_Click;
            this.Controls.Add(suggestionJ2Button);

            UpdateSuggestionButtons();
        }

        private void UpdateSuggestionButtons()
        {
            suggestionJ1Button.Enabled = player1Turn && !player2Turn; 
            suggestionJ2Button.Enabled = player2Turn && !player1Turn; 
        }

        private void SuggestionJ1Button_Click(object? sender, EventArgs e)
        {
            Console.WriteLine("J1's Turn: " + player1Turn); 
            if (!player1Turn)
            {
                if(TryMakeStrategicMove(1, 2)) 
                {
                    player2Turn = false; 
                    player1Turn = true;  
                    gamePanel.Invalidate();  
                    UpdateSuggestionButtons(); 
                }
            }
        }
        
        private void SuggestionJ2Button_Click(object? sender, EventArgs e)
        {
            Console.WriteLine("J2's Turn: " + player2Turn);
            if(!player2Turn) 
            {
                if (TryMakeStrategicMove(2, 1)) 
                {
                    player1Turn = false; 
                    player2Turn = true;   
                    gamePanel.Invalidate(); 
                    UpdateSuggestionButtons();  
                }
            }
        }
        
        private bool TryMakeStrategicMove(int player, int opponent)
        {
            if (TryCompleteLine(player, 4))
            {
                return true;
            }
        
            if (TryBlockOpponent(opponent, 4))
            {
                return true;
            }
        
            if (TryExtendLine(player, 3))
            {
                return true;
            }
        
            // Priorité 4 : Autres possibilités
            /*if (TryOtherMoves(player))
            {
                return true;
            }*/
        
            MessageBox.Show($"No strategic move found for Player {player}");
            return false;
        }
        
        private bool TryCompleteLine(int player, int length)
        {
            for (int ni = 0; ni < gridSize; ni++)
            {
                for (int ji = 0; ji < gridSize; ji++)
                {
                    if (pointsPlaced[ni, ji] == 0)
                    {
                        if (CanExtendLine(ni, ji, player, length))
                        {
                            PlacePoint(ni, ji);
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        
        private bool TryBlockOpponent(int opponent, int length)
        {
            for (int ni = 0; ni < gridSize; ni++)
            {
                for (int ji = 0; ji < gridSize; ji++)
                {
                    if (pointsPlaced[ni, ji] == 0)
                    {
                        if (CanBlockLine(ni, ji, opponent, length))
                        {
                            PlacePoint(ni, ji);
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        
        private bool TryExtendLine(int player, int length)
        {
            for (int ni = 0; ni < gridSize; ni++)
            {
                for (int ji = 0; ji < gridSize; ji++)
                {
                    if (pointsPlaced[ni, ji] == 0)
                    {
                        if (CanExtendLine(ni, ji, player, length))
                        {
                            PlacePoint(ni, ji);
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        
        /*private bool TryOtherMoves(int player)
        {
            for (int ni = 0; ni < gridSize; ni++)
            {
                for (int ji = 0; ji < gridSize; ji++)
                {
                    if (pointsPlaced[ni, ji] == 0)
                    {
                        PlacePoint(ni, ji);
                        return true;
                    }
                }
            }
            return false;
        }*/
        
        private bool CanExtendLine(int x, int y, int player, int length)
        {
            return CheckPotentialLine(x, y, player, length);
        }
        
        private bool CanBlockLine(int x, int y, int opponent, int length)
        {
            return CheckPotentialLine(x, y, opponent, length);
        }
        
        private bool CheckPotentialLine(int x, int y, int player, int length)
        {
            bool result = CheckLineDirection(x, y, 1, 0, player, length) || 
                          CheckLineDirection(x, y, 0, 1, player, length) || 
                          CheckLineDirection(x, y, 1, 1, player, length) || 
                          CheckLineDirection(x, y, 1, -1, player, length);
            return result;
        }
        
        private bool CheckLineDirection(int x, int y, int deltaX, int deltaY, int player, int length)
        {
            int count = 1; 
        
            for (int step = 1; step < length; step++)
            {
                int newX = x + deltaX * step;
                int newY = y + deltaY * step;
        
                if (newX < 0 || newY < 0 || newX >= gridSize || newY >= gridSize || pointsPlaced[newX, newY] != player)
                    break;
        
                count++;
            }
        
            for (int step = 1; step < length; step++)
            {
                int newX = x - deltaX * step;
                int newY = y - deltaY * step;
        
                if (newX < 0 || newY < 0 || newX >= gridSize || newY >= gridSize || pointsPlaced[newX, newY] != player)
                    break;
        
                count++;
            }
        
            return count >= length;
        }


        private void SaveGame(object? sender, EventArgs e)
        {
            SaveAndLoad saveData = new SaveAndLoad()
            {
                Player1Turn = player1Turn,
                FormedLines = formedLines
            };

            saveData.SetPointsPlaced(pointsPlaced); 

            if (!Directory.Exists(defaultSaveDirectory))
            {
                Directory.CreateDirectory(defaultSaveDirectory);
            }

            string filePath = Path.Combine(defaultSaveDirectory, "Dotgame_Save.json");
            string json = System.Text.Json.JsonSerializer.Serialize(saveData);
            File.WriteAllText(filePath, json);

            MessageBox.Show($"Game saved successfully in {filePath}");
        }

        private void LoadGame(object? sender, EventArgs e)
        {
            string filePath = Path.Combine(defaultSaveDirectory, "Dotgame_Save.json");
        
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                SaveAndLoad saveData = System.Text.Json.JsonSerializer.Deserialize<SaveAndLoad>(json);
        
                if (saveData != null)
                {
                    pointsPlaced = saveData.GetPointsPlaced();
                    player1Turn = saveData.Player1Turn;
                    formedLines = saveData.FormedLines;
        
                    gamePanel.Invalidate(); 
                    MessageBox.Show("Game loaded successfully.");
                }
            }
            else
            {
                MessageBox.Show($"No save file found in {filePath}");
            }
        }

        private void InitializeGridPoints()
        {
            gridPoints = new Point[gridSize, gridSize];
            pointsPlaced = new int[gridSize, gridSize]; 

            for (int ni = 0; ni < gridSize; ni++)
            {
                for (int ji = 0; ji < gridSize; ji++)
                {
                    gridPoints[ni, ji] = new Point(ni * cellSize, ji * cellSize);
                }
            }
        }

        private void GamePanel_Paint(object? sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen pen = new Pen(Color.Black);

            // Dessiner la grille
            for (int ni = 0; ni <= gridSize; ni++)
            {
                g.DrawLine(pen, ni * cellSize, 0, ni * cellSize, gamePanel.Height);
                g.DrawLine(pen, 0, ni * cellSize, gamePanel.Width, ni * cellSize);
            }

            // Dessiner les points placés
            for (int ni = 0; ni < gridSize; ni++)
            {
                for (int ji = 0; ji < gridSize; ji++)
                {
                    if (pointsPlaced[ni, ji] != 0)
                    {
                        Brush brush = pointsPlaced[ni, ji] == 1 ? Brushes.Red : Brushes.Blue;

                        // Vérifie si le point est suggéré pour changer la couleur
                        if (suggestedPoint.HasValue && suggestedPoint.Value.X == ni && suggestedPoint.Value.Y == ji)
                        {
                            brush = pointsPlaced[ni, ji] == 1 ? Brushes.LightCoral : Brushes.LightBlue; // Couleurs différentes pour les suggestions
                        }

                        Point centerPoint = gridPoints[ni, ji];
                        g.FillEllipse(brush, centerPoint.X - pointRadius, centerPoint.Y - pointRadius, pointRadius * 2, pointRadius * 2);
                    }
                }
            }

            // Dessiner les lignes formées
            foreach (var line in formedLines)
            {
                g.DrawLine(new Pen(player1Turn ? Color.Red : Color.Blue, 2), line.Start, line.End);
            }
        }



        private void GamePanel_MouseClick(object? sender, MouseEventArgs e)
        {
            for (int ni = 0; ni < gridSize; ni++)
            {
                for (int ji = 0; ji < gridSize; ji++)
                {
                    Rectangle clickableArea = new Rectangle(gridPoints[ni, ji].X - cellSize / 2, gridPoints[ni, ji].Y - cellSize / 2, cellSize, cellSize);

                    if (clickableArea.Contains(e.Location) && pointsPlaced[ni, ji] == 0)
                    {
                        PlacePoint(ni, ji);
                        return;
                    }
                }
            }
        }

        
        private bool CheckForWin(int x, int y, out List<Point> winningPoints)
        {
            winningPoints = new List<Point>();
            return CheckDirection(x, y, 1, 0, ref winningPoints) || // Horizontal
                   CheckDirection(x, y, 0, 1, ref winningPoints) || // Vertical
                   CheckDirection(x, y, 1, 1, ref winningPoints) || // Diagonal \
                   CheckDirection(x, y, 1, -1, ref winningPoints);  // Diagonal /
        }

        private bool CheckDirection(int startX, int startY, int deltaX, int deltaY, ref List<Point> winningPoints)
        {
            int player = pointsPlaced[startX, startY];
            if (player == 0) return false;

            int count = 1;
            winningPoints.Clear();
            winningPoints.Add(gridPoints[startX, startY]);

            // Vérification dans une direction
            for (int step = 1; step < 5; step++)
            {
                int newX = startX + deltaX * step;
                int newY = startY + deltaY * step;

                if (newX < 0 || newY < 0 || newX >= gridSize || newY >= gridSize || pointsPlaced[newX, newY] != player)
                    break;

                winningPoints.Add(gridPoints[newX, newY]);
                count++;
            }

            // Vérification dans l'autre direction
            for (int step = 1; step < 5; step++)
            {
                int newX = startX - deltaX * step;
                int newY = startY - deltaY * step;

                if (newX < 0 || newY < 0 || newX >= gridSize || newY >= gridSize || pointsPlaced[newX, newY] != player)
                    break;

                winningPoints.Insert(0, gridPoints[newX, newY]);
                count++;
            }

            // Retourner vrai si l'alignement atteint cinq points
            return count >= 5;
        }

        private void PlacePoint(int xIndex, int yIndex)
        {
            pointsPlaced[xIndex, yIndex] = player1Turn ? 1 : 2;
        
            // Enregistrer le point suggéré
            suggestedPoint = new Point(xIndex, yIndex);
        
            gamePanel.Invalidate();
        
            UpdateSuggestionButtons();
            UpdateTurnLabel();
        
            if (CheckForWin(xIndex, yIndex, out List<Point> winningPoints))
            {
                formedLines.Add(new Line(winningPoints[0], winningPoints[4]));
                gamePanel.Invalidate(); 
        
                WinnerDialog winnerDialog = new WinnerDialog($"Player {(player2Turn ? "1" : "2")} wins!");
                winnerDialog.DialogPosition = new Point(940, 390); 
                winnerDialog.ShowDialog();
        
                ResetGame();
            }
            else
            {
                player1Turn = !player1Turn;
                player2Turn = !player2Turn;
            }
        }


        private void ResetGame()
        {
            pointsPlaced = new int[gridSize, gridSize];
            formedLines.Clear();
            gamePanel.Invalidate();
        }

        private void ResetGame(object? sender, EventArgs e)
        {
            ResetGame();
        }

        private void Dotgame_FormClosing(object? sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }

   [Serializable]
    public class Line
    {
        public Point Start { get; set; }
        public Point End { get; set; }
    
        public Line(Point start, Point end)
        {
            Start = start;
            End = end;
        }
    }

}