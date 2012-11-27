namespace ImarisSelectorAdmin
{
    partial class MainWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.buttonImarisPath = new System.Windows.Forms.Button();
            this.buttonAbout = new System.Windows.Forms.Button();
            this.buttonHelp = new System.Windows.Forms.Button();
            this.labelExpl = new System.Windows.Forms.Label();
            this.labelProductsText = new System.Windows.Forms.Label();
            this.checkedListBoxProducts = new System.Windows.Forms.CheckedListBox();
            this.labelProductDescription = new System.Windows.Forms.Label();
            this.buttonSave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonImarisPath
            // 
            this.buttonImarisPath.Location = new System.Drawing.Point(11, 42);
            this.buttonImarisPath.Name = "buttonImarisPath";
            this.buttonImarisPath.Size = new System.Drawing.Size(492, 32);
            this.buttonImarisPath.TabIndex = 0;
            this.buttonImarisPath.Text = "...";
            this.buttonImarisPath.UseVisualStyleBackColor = true;
            this.buttonImarisPath.Click += new System.EventHandler(this.buttonImarisPath_Click);
            // 
            // buttonAbout
            // 
            this.buttonAbout.Location = new System.Drawing.Point(447, 3);
            this.buttonAbout.Name = "buttonAbout";
            this.buttonAbout.Size = new System.Drawing.Size(25, 25);
            this.buttonAbout.TabIndex = 9;
            this.buttonAbout.Text = "A";
            this.buttonAbout.UseVisualStyleBackColor = true;
            this.buttonAbout.Click += new System.EventHandler(this.buttonAbout_Click);
            // 
            // buttonHelp
            // 
            this.buttonHelp.Location = new System.Drawing.Point(478, 3);
            this.buttonHelp.Name = "buttonHelp";
            this.buttonHelp.Size = new System.Drawing.Size(25, 25);
            this.buttonHelp.TabIndex = 8;
            this.buttonHelp.Text = "?";
            this.buttonHelp.UseVisualStyleBackColor = true;
            this.buttonHelp.Click += new System.EventHandler(this.buttonHelp_Click);
            // 
            // labelExpl
            // 
            this.labelExpl.AutoSize = true;
            this.labelExpl.Location = new System.Drawing.Point(11, 15);
            this.labelExpl.Name = "labelExpl";
            this.labelExpl.Size = new System.Drawing.Size(283, 13);
            this.labelExpl.TabIndex = 10;
            this.labelExpl.Text = "Click here to choose the Imaris executable to manage:";
            // 
            // labelProductsText
            // 
            this.labelProductsText.AutoSize = true;
            this.labelProductsText.Location = new System.Drawing.Point(11, 88);
            this.labelProductsText.Name = "labelProductsText";
            this.labelProductsText.Size = new System.Drawing.Size(301, 13);
            this.labelProductsText.TabIndex = 11;
            this.labelProductsText.Text = "Choose the products that will be visible in ImarisSelector:";
            // 
            // checkedListBoxProducts
            // 
            this.checkedListBoxProducts.FormattingEnabled = true;
            this.checkedListBoxProducts.Location = new System.Drawing.Point(11, 115);
            this.checkedListBoxProducts.Name = "checkedListBoxProducts";
            this.checkedListBoxProducts.Size = new System.Drawing.Size(489, 225);
            this.checkedListBoxProducts.TabIndex = 12;
            this.checkedListBoxProducts.SelectedIndexChanged += new System.EventHandler(this.checkedListBoxProducts_SelectedIndexChanged);
            // 
            // labelProductDescription
            // 
            this.labelProductDescription.AutoSize = true;
            this.labelProductDescription.Location = new System.Drawing.Point(11, 354);
            this.labelProductDescription.Name = "labelProductDescription";
            this.labelProductDescription.Size = new System.Drawing.Size(0, 13);
            this.labelProductDescription.TabIndex = 13;
            // 
            // buttonSave
            // 
            this.buttonSave.Enabled = false;
            this.buttonSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonSave.Location = new System.Drawing.Point(14, 381);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(486, 39);
            this.buttonSave.TabIndex = 14;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(513, 429);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.labelProductDescription);
            this.Controls.Add(this.checkedListBoxProducts);
            this.Controls.Add(this.labelProductsText);
            this.Controls.Add(this.labelExpl);
            this.Controls.Add(this.buttonAbout);
            this.Controls.Add(this.buttonHelp);
            this.Controls.Add(this.buttonImarisPath);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainWindow";
            this.Text = "ImarisSelector :: Admin";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonImarisPath;
        private System.Windows.Forms.Button buttonAbout;
        private System.Windows.Forms.Button buttonHelp;
        private System.Windows.Forms.Label labelExpl;
        private System.Windows.Forms.Label labelProductsText;
        private System.Windows.Forms.CheckedListBox checkedListBoxProducts;
        private System.Windows.Forms.Label labelProductDescription;
        private System.Windows.Forms.Button buttonSave;
    }
}

