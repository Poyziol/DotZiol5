using System.Windows.Forms;
using System.Drawing;

namespace src
{
    public partial class MainMenu : Form
    {
        private Button btnPlay = null!;
        private Button btnExit = null!;
        private Label titleLabel = null!;

        public MainMenu()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.btnPlay = new Button();
            this.btnExit = new Button();

            // 
            // btnPlay
            // 
            this.btnPlay.Location = new System.Drawing.Point(100, 100);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(100, 50);
            this.btnPlay.Text = "Play";
            this.btnPlay.Click += new System.EventHandler(this.BtnPlay_Click);
            
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(100, 200);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(100, 50);
            this.btnExit.Text = "Exit";
            this.btnExit.Click += new System.EventHandler(this.BtnExit_Click);

            // 
            // MainMenu
            // 
            this.ClientSize = new System.Drawing.Size(300, 320);
            this.Controls.Add(this.btnPlay);
            this.Controls.Add(this.btnExit);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "DotZiol - Main Menu";

            // 
            // titleLabel
            // 
            titleLabel = new Label()
            {
                Text = "DotZiol 5",
                Font = new Font("Arial", 24, FontStyle.Bold),
                ForeColor = Color.Black, // Changed to Black to ensure visibility against a white background
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter,
                Height = 100
            };

            this.Controls.Add(titleLabel); // Corrected to Controls.Add
        }

        private void BtnPlay_Click(object? sender, System.EventArgs e)
        {
            this.Hide();
            Dotgame dotgame = new Dotgame();
            dotgame.Show();
        }

        private void BtnExit_Click(object? sender, System.EventArgs e)
        {
            Application.Exit();
        }
    }
}
