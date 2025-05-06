namespace WaAutoReplyBot
{
    partial class AddMessage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddMessage));
            this.materialCard1 = new MaterialSkin.Controls.MaterialCard();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.materialButton19 = new MaterialSkin.Controls.MaterialButton();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.materialButton3 = new MaterialSkin.Controls.MaterialButton();
            this.materialButton2 = new MaterialSkin.Controls.MaterialButton();
            this.materialButton1 = new MaterialSkin.Controls.MaterialButton();
            this.lstViewFiles = new MaterialSkin.Controls.MaterialListView();
            this.Files = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.txtLongMessage = new MaterialSkin.Controls.MaterialMultiLineTextBox2();
            this.materialCard1.SuspendLayout();
            this.groupBox11.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // materialCard1
            // 
            this.materialCard1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.materialCard1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.materialCard1.Controls.Add(this.groupBox11);
            this.materialCard1.Controls.Add(this.materialButton3);
            this.materialCard1.Controls.Add(this.materialButton2);
            this.materialCard1.Controls.Add(this.materialButton1);
            this.materialCard1.Controls.Add(this.lstViewFiles);
            this.materialCard1.Controls.Add(this.txtLongMessage);
            this.materialCard1.Depth = 0;
            this.materialCard1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.materialCard1.Location = new System.Drawing.Point(17, 90);
            this.materialCard1.Margin = new System.Windows.Forms.Padding(14);
            this.materialCard1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialCard1.Name = "materialCard1";
            this.materialCard1.Padding = new System.Windows.Forms.Padding(14);
            this.materialCard1.Size = new System.Drawing.Size(914, 582);
            this.materialCard1.TabIndex = 0;
            // 
            // groupBox11
            // 
            this.groupBox11.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox11.Controls.Add(this.panel1);
            this.groupBox11.Location = new System.Drawing.Point(15, 410);
            this.groupBox11.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox11.Size = new System.Drawing.Size(899, 89);
            this.groupBox11.TabIndex = 76;
            this.groupBox11.TabStop = false;
            this.groupBox11.Text = "Polls";
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.materialButton19);
            this.panel1.Controls.Add(this.webBrowser1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 17);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(893, 70);
            this.panel1.TabIndex = 0;
            // 
            // materialButton19
            // 
            this.materialButton19.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.materialButton19.AutoSize = false;
            this.materialButton19.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.materialButton19.BackgroundImage = global::WASender.Properties.Resources.file_download;
            this.materialButton19.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.materialButton19.Depth = 0;
            this.materialButton19.HighEmphasis = true;
            this.materialButton19.Icon = global::WASender.Properties.Resources.icons8_add_24px;
            this.materialButton19.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.materialButton19.Location = new System.Drawing.Point(25, 14);
            this.materialButton19.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.materialButton19.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialButton19.Name = "materialButton19";
            this.materialButton19.NoAccentTextColor = System.Drawing.Color.Empty;
            this.materialButton19.Size = new System.Drawing.Size(207, 39);
            this.materialButton19.TabIndex = 5;
            this.materialButton19.Text = "Add Poll";
            this.materialButton19.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.materialButton19.UseAccentColor = false;
            this.materialButton19.UseVisualStyleBackColor = true;
            this.materialButton19.Click += new System.EventHandler(this.materialButton19_Click);
            // 
            // webBrowser1
            // 
            this.webBrowser1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.webBrowser1.IsWebBrowserContextMenuEnabled = false;
            this.webBrowser1.Location = new System.Drawing.Point(287, 2);
            this.webBrowser1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(591, 59);
            this.webBrowser1.TabIndex = 2;
            // 
            // materialButton3
            // 
            this.materialButton3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.materialButton3.AutoSize = false;
            this.materialButton3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.materialButton3.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.materialButton3.Depth = 0;
            this.materialButton3.HighEmphasis = true;
            this.materialButton3.Icon = global::WASender.Properties.Resources.icons8_cancel_25px_1;
            this.materialButton3.Location = new System.Drawing.Point(18, 526);
            this.materialButton3.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.materialButton3.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialButton3.Name = "materialButton3";
            this.materialButton3.NoAccentTextColor = System.Drawing.Color.Empty;
            this.materialButton3.Size = new System.Drawing.Size(155, 36);
            this.materialButton3.TabIndex = 75;
            this.materialButton3.Text = "Cancel";
            this.materialButton3.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined;
            this.materialButton3.UseAccentColor = false;
            this.materialButton3.UseVisualStyleBackColor = true;
            this.materialButton3.Click += new System.EventHandler(this.materialButton3_Click);
            // 
            // materialButton2
            // 
            this.materialButton2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.materialButton2.AutoSize = false;
            this.materialButton2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.materialButton2.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.materialButton2.Depth = 0;
            this.materialButton2.HighEmphasis = true;
            this.materialButton2.Icon = global::WASender.Properties.Resources.icons8_save_24px;
            this.materialButton2.Location = new System.Drawing.Point(728, 526);
            this.materialButton2.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.materialButton2.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialButton2.Name = "materialButton2";
            this.materialButton2.NoAccentTextColor = System.Drawing.Color.Empty;
            this.materialButton2.Size = new System.Drawing.Size(168, 36);
            this.materialButton2.TabIndex = 74;
            this.materialButton2.Text = "Add ";
            this.materialButton2.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.materialButton2.UseAccentColor = false;
            this.materialButton2.UseVisualStyleBackColor = true;
            this.materialButton2.Click += new System.EventHandler(this.materialButton2_Click);
            // 
            // materialButton1
            // 
            this.materialButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.materialButton1.AutoSize = false;
            this.materialButton1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.materialButton1.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.materialButton1.Depth = 0;
            this.materialButton1.HighEmphasis = true;
            this.materialButton1.Icon = global::WASender.Properties.Resources.icons8_file_24px;
            this.materialButton1.Location = new System.Drawing.Point(728, 17);
            this.materialButton1.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.materialButton1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialButton1.Name = "materialButton1";
            this.materialButton1.NoAccentTextColor = System.Drawing.Color.Empty;
            this.materialButton1.Size = new System.Drawing.Size(168, 36);
            this.materialButton1.TabIndex = 73;
            this.materialButton1.Text = "Add FIle";
            this.materialButton1.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined;
            this.materialButton1.UseAccentColor = false;
            this.materialButton1.UseVisualStyleBackColor = true;
            this.materialButton1.Click += new System.EventHandler(this.materialButton1_Click);
            // 
            // lstViewFiles
            // 
            this.lstViewFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstViewFiles.AutoSizeTable = false;
            this.lstViewFiles.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.lstViewFiles.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstViewFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Files});
            this.lstViewFiles.Depth = 0;
            this.lstViewFiles.FullRowSelect = true;
            this.lstViewFiles.Location = new System.Drawing.Point(547, 77);
            this.lstViewFiles.MinimumSize = new System.Drawing.Size(200, 100);
            this.lstViewFiles.MouseLocation = new System.Drawing.Point(-1, -1);
            this.lstViewFiles.MouseState = MaterialSkin.MouseState.OUT;
            this.lstViewFiles.Name = "lstViewFiles";
            this.lstViewFiles.OwnerDraw = true;
            this.lstViewFiles.ShowItemToolTips = true;
            this.lstViewFiles.Size = new System.Drawing.Size(349, 328);
            this.lstViewFiles.TabIndex = 72;
            this.lstViewFiles.UseCompatibleStateImageBehavior = false;
            this.lstViewFiles.View = System.Windows.Forms.View.Details;
            this.lstViewFiles.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lstViewFiles_KeyDown);
            // 
            // Files
            // 
            this.Files.Text = "Files";
            this.Files.Width = 450;
            // 
            // txtLongMessage
            // 
            this.txtLongMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLongMessage.AnimateReadOnly = false;
            this.txtLongMessage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.txtLongMessage.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.txtLongMessage.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtLongMessage.Depth = 0;
            this.txtLongMessage.HideSelection = true;
            this.txtLongMessage.Hint = "Type Your Message here";
            this.txtLongMessage.Location = new System.Drawing.Point(17, 17);
            this.txtLongMessage.MaxLength = 2147483647;
            this.txtLongMessage.MouseState = MaterialSkin.MouseState.OUT;
            this.txtLongMessage.Name = "txtLongMessage";
            this.txtLongMessage.PasswordChar = '\0';
            this.txtLongMessage.ReadOnly = false;
            this.txtLongMessage.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.txtLongMessage.SelectedText = "";
            this.txtLongMessage.SelectionLength = 0;
            this.txtLongMessage.SelectionStart = 0;
            this.txtLongMessage.ShortcutsEnabled = true;
            this.txtLongMessage.Size = new System.Drawing.Size(479, 388);
            this.txtLongMessage.TabIndex = 71;
            this.txtLongMessage.TabStop = false;
            this.txtLongMessage.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.txtLongMessage.UseSystemPasswordChar = false;
            // 
            // AddMessage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(948, 689);
            this.Controls.Add(this.materialCard1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddMessage";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Reply Message";
            this.Load += new System.EventHandler(this.AddMessage_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.AddMessage_KeyDown);
            this.materialCard1.ResumeLayout(false);
            this.groupBox11.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private MaterialSkin.Controls.MaterialCard materialCard1;
        private MaterialSkin.Controls.MaterialButton materialButton1;
        private MaterialSkin.Controls.MaterialListView lstViewFiles;
        private MaterialSkin.Controls.MaterialMultiLineTextBox2 txtLongMessage;
        private MaterialSkin.Controls.MaterialButton materialButton3;
        private MaterialSkin.Controls.MaterialButton materialButton2;
        private System.Windows.Forms.ColumnHeader Files;
        private System.Windows.Forms.GroupBox groupBox11;
        private System.Windows.Forms.Panel panel1;
        private MaterialSkin.Controls.MaterialButton materialButton19;
        private System.Windows.Forms.WebBrowser webBrowser1;
    }
}