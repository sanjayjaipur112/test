namespace WASender
{
    partial class BuiltInVariable
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.gridMarker = new System.Windows.Forms.DataGridView();
            this.Select = new System.Windows.Forms.DataGridViewButtonColumn();
            this.materialButton1 = new MaterialSkin.Controls.MaterialButton();
            ((System.ComponentModel.ISupportInitialize)(this.gridMarker)).BeginInit();
            this.SuspendLayout();
            // 
            // gridMarker
            // 
            this.gridMarker.AllowUserToAddRows = false;
            this.gridMarker.AllowUserToDeleteRows = false;
            this.gridMarker.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridMarker.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridMarker.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridMarker.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Select});
            this.gridMarker.Location = new System.Drawing.Point(6, 83);
            this.gridMarker.Name = "gridMarker";
            this.gridMarker.ReadOnly = true;
            this.gridMarker.RowTemplate.Height = 24;
            this.gridMarker.Size = new System.Drawing.Size(433, 418);
            this.gridMarker.TabIndex = 1;
            this.gridMarker.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridMarker_CellClick);
            // 
            // Select
            // 
            this.Select.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Select.HeaderText = "";
            this.Select.Name = "Select";
            this.Select.ReadOnly = true;
            this.Select.Text = "<< Add";
            this.Select.UseColumnTextForButtonValue = true;
            // 
            // materialButton1
            // 
            this.materialButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.materialButton1.AutoSize = false;
            this.materialButton1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.materialButton1.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.materialButton1.Depth = 0;
            this.materialButton1.HighEmphasis = true;
            this.materialButton1.Icon = global::WASender.Properties.Resources.icons8_close_window_24px;
            this.materialButton1.Location = new System.Drawing.Point(275, 517);
            this.materialButton1.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.materialButton1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialButton1.Name = "materialButton1";
            this.materialButton1.NoAccentTextColor = System.Drawing.Color.Empty;
            this.materialButton1.Size = new System.Drawing.Size(159, 36);
            this.materialButton1.TabIndex = 3;
            this.materialButton1.Text = "Close";
            this.materialButton1.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.materialButton1.UseAccentColor = false;
            this.materialButton1.UseVisualStyleBackColor = true;
            this.materialButton1.Click += new System.EventHandler(this.materialButton1_Click);
            // 
            // BuiltInVariable
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(445, 573);
            this.Controls.Add(this.materialButton1);
            this.Controls.Add(this.gridMarker);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BuiltInVariable";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Built In Variable";
            this.Load += new System.EventHandler(this.BuiltInVariable_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridMarker)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView gridMarker;
        private System.Windows.Forms.DataGridViewButtonColumn Select;
        private MaterialSkin.Controls.MaterialButton materialButton1;

    }
}