namespace ImarisSelector
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
            this.checkedListBoxLicenses = new System.Windows.Forms.CheckedListBox();
            this.radioSelSimple = new System.Windows.Forms.RadioButton();
            this.radioSelAdvanced = new System.Windows.Forms.RadioButton();
            this.buttonStartImaris = new System.Windows.Forms.Button();
            this.labelLicenseDescription = new System.Windows.Forms.Label();
            this.labelLicenseName = new System.Windows.Forms.Label();
            this.buttonHelp = new System.Windows.Forms.Button();
            this.buttonAbout = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // checkedListBoxLicenses
            // 
            this.checkedListBoxLicenses.CheckOnClick = true;
            this.checkedListBoxLicenses.FormattingEnabled = true;
            this.checkedListBoxLicenses.Location = new System.Drawing.Point(12, 35);
            this.checkedListBoxLicenses.Name = "checkedListBoxLicenses";
            this.checkedListBoxLicenses.Size = new System.Drawing.Size(460, 289);
            this.checkedListBoxLicenses.TabIndex = 0;
            this.checkedListBoxLicenses.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBoxLicenses_ItemCheck);
            this.checkedListBoxLicenses.SelectedIndexChanged += new System.EventHandler(this.checkedListBoxLicenses_SelectedIndexChanged);
            // 
            // radioSelSimple
            // 
            this.radioSelSimple.AutoSize = true;
            this.radioSelSimple.Checked = true;
            this.radioSelSimple.Location = new System.Drawing.Point(12, 12);
            this.radioSelSimple.Name = "radioSelSimple";
            this.radioSelSimple.Size = new System.Drawing.Size(59, 17);
            this.radioSelSimple.TabIndex = 1;
            this.radioSelSimple.TabStop = true;
            this.radioSelSimple.Text = "Simple";
            this.radioSelSimple.UseVisualStyleBackColor = true;
            this.radioSelSimple.CheckedChanged += new System.EventHandler(this.radioSelSimple_CheckedChanged);
            // 
            // radioSelAdvanced
            // 
            this.radioSelAdvanced.AutoSize = true;
            this.radioSelAdvanced.Location = new System.Drawing.Point(81, 12);
            this.radioSelAdvanced.Name = "radioSelAdvanced";
            this.radioSelAdvanced.Size = new System.Drawing.Size(75, 17);
            this.radioSelAdvanced.TabIndex = 2;
            this.radioSelAdvanced.TabStop = true;
            this.radioSelAdvanced.Text = "Advanced";
            this.radioSelAdvanced.UseVisualStyleBackColor = true;
            this.radioSelAdvanced.CheckedChanged += new System.EventHandler(this.radioSelAdvanced_CheckedChanged);
            // 
            // buttonStartImaris
            // 
            this.buttonStartImaris.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonStartImaris.Location = new System.Drawing.Point(12, 410);
            this.buttonStartImaris.Name = "buttonStartImaris";
            this.buttonStartImaris.Size = new System.Drawing.Size(460, 39);
            this.buttonStartImaris.TabIndex = 3;
            this.buttonStartImaris.Text = "Start Imaris";
            this.buttonStartImaris.UseVisualStyleBackColor = true;
            this.buttonStartImaris.Click += new System.EventHandler(this.buttonStartImaris_Click);
            // 
            // labelLicenseDescription
            // 
            this.labelLicenseDescription.AutoSize = true;
            this.labelLicenseDescription.Location = new System.Drawing.Point(12, 353);
            this.labelLicenseDescription.Name = "labelLicenseDescription";
            this.labelLicenseDescription.Size = new System.Drawing.Size(0, 13);
            this.labelLicenseDescription.TabIndex = 4;
            // 
            // labelLicenseName
            // 
            this.labelLicenseName.AutoSize = true;
            this.labelLicenseName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelLicenseName.Location = new System.Drawing.Point(12, 331);
            this.labelLicenseName.Name = "labelLicenseName";
            this.labelLicenseName.Size = new System.Drawing.Size(0, 13);
            this.labelLicenseName.TabIndex = 5;
            // 
            // buttonHelp
            // 
            this.buttonHelp.Location = new System.Drawing.Point(449, 4);
            this.buttonHelp.Name = "buttonHelp";
            this.buttonHelp.Size = new System.Drawing.Size(25, 25);
            this.buttonHelp.TabIndex = 6;
            this.buttonHelp.Text = "?";
            this.buttonHelp.UseVisualStyleBackColor = true;
            this.buttonHelp.Click += new System.EventHandler(this.buttonHelp_Click);
            // 
            // buttonAbout
            // 
            this.buttonAbout.Location = new System.Drawing.Point(418, 4);
            this.buttonAbout.Name = "buttonAbout";
            this.buttonAbout.Size = new System.Drawing.Size(25, 25);
            this.buttonAbout.TabIndex = 7;
            this.buttonAbout.Text = "A";
            this.buttonAbout.UseVisualStyleBackColor = true;
            this.buttonAbout.Click += new System.EventHandler(this.buttonAbout_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 461);
            this.Controls.Add(this.buttonAbout);
            this.Controls.Add(this.buttonHelp);
            this.Controls.Add(this.labelLicenseName);
            this.Controls.Add(this.labelLicenseDescription);
            this.Controls.Add(this.buttonStartImaris);
            this.Controls.Add(this.radioSelAdvanced);
            this.Controls.Add(this.radioSelSimple);
            this.Controls.Add(this.checkedListBoxLicenses);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "MainWindow";
            this.Text = "ImarisSelector";
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox checkedListBoxLicenses;
        private System.Windows.Forms.RadioButton radioSelSimple;
        private System.Windows.Forms.RadioButton radioSelAdvanced;
        private System.Windows.Forms.Button buttonStartImaris;
        private System.Windows.Forms.Label labelLicenseDescription;
        private System.Windows.Forms.Label labelLicenseName;
        private System.Windows.Forms.Button buttonHelp;
        private System.Windows.Forms.Button buttonAbout;
    }
}
