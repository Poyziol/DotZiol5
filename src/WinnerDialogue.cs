using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace src
{
    public class WinnerDialog : Form
    {
        private Label messageLabel = null!;
        private Button okButton = null!;

        public WinnerDialog(string message)
        {
            InitializeComponents(message);
        }

        private void InitializeComponents(string message)
        {
            this.messageLabel = new Label();
            this.okButton = new Button();

            this.messageLabel.Text = message;
            this.messageLabel.AutoSize = true;
            this.messageLabel.Location = new Point(20, 20);

            this.okButton.Text = "OK";
            this.okButton.Location = new Point(70, 50);
            this.okButton.Click += new EventHandler(OkButton_Click);

            this.Controls.Add(this.messageLabel);
            this.Controls.Add(this.okButton);

            this.StartPosition = FormStartPosition.Manual; // Allow manual position setting
            this.ClientSize = new Size(200, 100);
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Point DialogPosition
        {
            get { return this.Location; }
            set { this.Location = value; }
        }

        private void OkButton_Click(object? sender, EventArgs e)
        {
            this.Close();
        }
    }
}
