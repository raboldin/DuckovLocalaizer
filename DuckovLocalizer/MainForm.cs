using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace DuckovLocalizer;

public class MainForm : Form
{
	private string _gamePath = "";

	private const string TARGET_SUBFOLDER = "Duckov_Data\\StreamingAssets\\Localization";

	private Button _autoButton;

	private Button _browseButton;

	private Button _installButton;

	private ProgressBar _progressBar;

	private Label _statusLabel;

	public MainForm()
	{
		InitializeComponents();
	}

	private void InitializeComponents()
	{
		Text = "Escape from Duckov Localizer";
		base.Size = new Size(500, 260);
		base.StartPosition = FormStartPosition.CenterScreen;
		base.FormBorderStyle = FormBorderStyle.FixedSingle;
		base.MaximizeBox = false;
		Label label = new Label
		{
			Text = "Select game folder:",
			Location = new Point(20, 20),
			AutoSize = true
		};
		_autoButton = new Button
		{
			Text = "Auto-detect path",
			Location = new Point(20, 50),
			Size = new Size(180, 30)
		};
		_autoButton.Click += AutoDetectPath_Click;
		_browseButton = new Button
		{
			Text = "Browse manually...",
			Location = new Point(210, 50),
			Size = new Size(180, 30)
		};
		_browseButton.Click += BrowsePath_Click;
		_installButton = new Button
		{
			Text = "Install Italian.csv",
			Location = new Point(20, 100),
			Size = new Size(430, 40),
			Enabled = false
		};
		_installButton.Click += InstallFile_Click;
		_progressBar = new ProgressBar
		{
			Location = new Point(20, 150),
			Size = new Size(430, 20),
			Style = ProgressBarStyle.Marquee,
			Visible = false,
			MarqueeAnimationSpeed = 30
		};
		_statusLabel = new Label
		{
			Location = new Point(20, 180),
			Size = new Size(430, 20),
			TextAlign = ContentAlignment.MiddleCenter
		};
		base.Controls.AddRange(label, _autoButton, _browseButton, _installButton, _progressBar, _statusLabel);
	}

	private void AutoDetectPath_Click(object sender, EventArgs e)
	{
		_statusLabel.Text = "Searching for game...";
		_statusLabel.ForeColor = Color.Blue;
		Application.DoEvents();
		string text = null;
		string text2 = Path.Combine(Environment.GetEnvironmentVariable("ProgramFiles(x86)") ?? "C:\\Program Files (x86)", "Steam\\steamapps\\common\\Escape_from_Duckov");
		if (Directory.Exists(text2) && Directory.Exists(Path.Combine(text2, "Duckov_Data")))
		{
			text = text2;
		}
		else
		{
			DriveInfo[] drives = DriveInfo.GetDrives();
			foreach (DriveInfo driveInfo in drives)
			{
				if (!driveInfo.IsReady || driveInfo.DriveType != DriveType.Fixed)
				{
					continue;
				}
				try
				{
					string name = driveInfo.Name;
					string[] obj = new string[7] { "Users", "Games", "Downloads", "Desktop", "Documents", "Program Files", "Program Files (x86)" };
					bool flag = false;
					string[] array = obj;
					foreach (string path in array)
					{
						string path2 = Path.Combine(name, path);
						if (!Directory.Exists(path2))
						{
							continue;
						}
						try
						{
							string[] directories = Directory.GetDirectories(path2, "*Escape*From*Duckov*", SearchOption.AllDirectories);
							foreach (string text3 in directories)
							{
								if (Directory.Exists(Path.Combine(text3, "Duckov_Data")))
								{
									text = text3;
									flag = true;
									break;
								}
							}
						}
						catch
						{
						}
						if (flag)
						{
							break;
						}
					}
					if (flag)
					{
						break;
					}
				}
				catch
				{
				}
			}
		}
		if (text != null)
		{
			_gamePath = text;
			_statusLabel.Text = "Found: " + Path.GetFileName(text);
			_statusLabel.ForeColor = Color.Green;
			_installButton.Enabled = true;
		}
		else
		{
			MessageBox.Show("Game folder not found automatically.\nPlease select it manually.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			_statusLabel.Text = "Path not set.";
			_statusLabel.ForeColor = Color.Red;
		}
	}

	private void BrowsePath_Click(object sender, EventArgs e)
	{
		using FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
		folderBrowserDialog.Description = "Select Escape_from_Duckov game folder";
		if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
		{
			_gamePath = folderBrowserDialog.SelectedPath;
			_statusLabel.Text = "Selected: " + Path.GetFileName(_gamePath);
			_statusLabel.ForeColor = Color.Green;
			_installButton.Enabled = true;
		}
	}

	private void InstallFile_Click(object sender, EventArgs e)
	{
		_installButton.Enabled = false;
		_progressBar.Visible = true;
		_statusLabel.Text = "Installing...";
		_statusLabel.ForeColor = Color.Black;
		Application.DoEvents();
		try
		{
			string text = Path.Combine(_gamePath, "Duckov_Data\\StreamingAssets\\Localization");
			Directory.CreateDirectory(text);
			string path = Path.Combine(text, "Italian.csv");
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			string text2 = "DuckovLocalizer.Italian.csv";
			using (Stream stream = executingAssembly.GetManifestResourceStream(text2))
			{
				if (stream == null)
				{
					throw new Exception("Resource '" + text2 + "' not found. Make sure Italian.csv is in project and marked as EmbeddedResource.");
				}
				using FileStream destination = File.Create(path);
				stream.CopyTo(destination);
			}
			_statusLabel.Text = "Italian.csv installed successfully!";
			_statusLabel.ForeColor = Color.Green;
		}
		catch (Exception ex)
		{
			_statusLabel.Text = "Error: " + ex.Message;
			_statusLabel.ForeColor = Color.Red;
			MessageBox.Show("Failed to install file:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
		finally
		{
			_progressBar.Visible = false;
			_installButton.Enabled = true;
		}
	}
}
