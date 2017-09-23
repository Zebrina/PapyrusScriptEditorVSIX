using Papyrus.Games;
using Papyrus.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Papyrus {
    internal partial class PapyrusOptionPageUserControl : UserControl {
        private ComboBox targetGameComboBox;
        private Label targetGameLabel;
        private PapyrusOptionPage optionPage;

        public PapyrusOptionPageUserControl(PapyrusOptionPage optionPage) {
            this.optionPage = optionPage;

            InitializeComponent();
        }

        public void Initialize() {
            targetGameComboBox.Items.AddRange(PapyrusEditor.RegisteredGames.ToArray());
            targetGameComboBox.SelectedItem = PapyrusEditor.ActiveGame;
        }

        private void InitializeComponent() {
            this.targetGameComboBox = new System.Windows.Forms.ComboBox();
            this.targetGameLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // targetGameComboBox
            // 
            this.targetGameComboBox.FormattingEnabled = true;
            this.targetGameComboBox.Location = new System.Drawing.Point(180, 41);
            this.targetGameComboBox.Name = "targetGameComboBox";
            this.targetGameComboBox.Size = new System.Drawing.Size(121, 21);
            this.targetGameComboBox.TabIndex = 0;
            this.targetGameComboBox.SelectedIndexChanged += new System.EventHandler(this.targetGameComboBox_SelectedIndexChanged);
            // 
            // targetGameLabel
            // 
            this.targetGameLabel.AutoSize = true;
            this.targetGameLabel.Location = new System.Drawing.Point(105, 44);
            this.targetGameLabel.Name = "targetGameLabel";
            this.targetGameLabel.Size = new System.Drawing.Size(69, 13);
            this.targetGameLabel.TabIndex = 1;
            this.targetGameLabel.Text = "Target Game";
            // 
            // PapyrusOptionPageUserControl
            // 
            this.Controls.Add(this.targetGameLabel);
            this.Controls.Add(this.targetGameComboBox);
            this.Name = "PapyrusOptionPageUserControl";
            this.Size = new System.Drawing.Size(579, 427);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void targetGameComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (targetGameComboBox.SelectedItem != null) {
                PapyrusEditor.ActiveGame = (IGameInfo)targetGameComboBox.SelectedItem;
            }
        }
    }
}
