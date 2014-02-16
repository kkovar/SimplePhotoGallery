using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace SimplePhotoGallery.Utilities
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.OpenFileDialog ofd;
		private Bitmap img = null;
		private System.Windows.Forms.Button loadImage;
		private System.Windows.Forms.NumericUpDown angle;
		private System.Windows.Forms.PictureBox pictureBox;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.loadImage = new System.Windows.Forms.Button();
			this.angle = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.ofd = new System.Windows.Forms.OpenFileDialog();
			this.pictureBox = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.angle)).BeginInit();
			this.SuspendLayout();
			// 
			// loadImage
			// 
			this.loadImage.Location = new System.Drawing.Point(8, 8);
			this.loadImage.Name = "loadImage";
			this.loadImage.Size = new System.Drawing.Size(88, 23);
			this.loadImage.TabIndex = 0;
			this.loadImage.Text = "Load &Image...";
			this.loadImage.Click += new System.EventHandler(this.loadImage_Click);
			// 
			// angle
			// 
			this.angle.DecimalPlaces = 1;
			this.angle.Enabled = false;
			this.angle.Location = new System.Drawing.Point(176, 8);
			this.angle.Maximum = new System.Decimal(new int[] {
																  360,
																  0,
																  0,
																  0});
			this.angle.Minimum = new System.Decimal(new int[] {
																  1,
																  0,
																  0,
																  -2147483648});
			this.angle.Name = "angle";
			this.angle.Size = new System.Drawing.Size(64, 20);
			this.angle.TabIndex = 1;
			this.angle.ValueChanged += new System.EventHandler(this.angle_ValueChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(104, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(72, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "&Rotate Image";
			// 
			// pictureBox
			// 
			this.pictureBox.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pictureBox.Location = new System.Drawing.Point(8, 40);
			this.pictureBox.Name = "pictureBox";
			this.pictureBox.Size = new System.Drawing.Size(312, 264);
			this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.pictureBox.TabIndex = 3;
			this.pictureBox.TabStop = false;
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(328, 315);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.pictureBox,
																		  this.label1,
																		  this.angle,
																		  this.loadImage});
			this.Name = "Form1";
			this.Text = "Image Rotation demo";
			((System.ComponentModel.ISupportInitialize)(this.angle)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		private void loadImage_Click(object sender, System.EventArgs e)
		{
			if( ofd.ShowDialog() == DialogResult.OK )
			{
				img = (Bitmap) Bitmap.FromFile(ofd.FileName);
				angle.Enabled = true;

				// Rotate/draw
				angle_ValueChanged(null, EventArgs.Empty);
			}
		}

		private void angle_ValueChanged(object sender, System.EventArgs e)
		{
			if( angle.Value > 359.9m )
			{
				angle.Value = 0;
				return ;
			}

			if( angle.Value < 0.0m )
			{
				angle.Value = 359;
				return ;
			}
			
			Image oldImage = pictureBox.Image;
			pictureBox.Image = ImageProcessor.RotateImage(img, (float) angle.Value);

			if( oldImage != null )
			{
				oldImage.Dispose();
			}
		}
	}
}
