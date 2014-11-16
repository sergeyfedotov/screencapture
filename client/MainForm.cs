using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Reflection;

namespace Screencapture
{
    public partial class MainForm : Form
    {
        static string UploadAddress = "http://example.com/upload.php/?secretKey=secretword";

        Point SelectionStartPoint;
        Rectangle SelectionRectangle;
        bool SelectionStarted;
        Graphics Screenshot;

        NotifyIcon NotifyIcon;
        KeyboardHook KeyboardHook;
        ImageUploader ImageUploader;

        string LastUploadedUrl;

        public MainForm()
        {
            InitializeComponent();
            Initialize();
            RegisterHotKey();
            CreateImageUploader();
        }

        void Initialize()
        {
            FormBorderStyle = FormBorderStyle.None;
            DoubleBuffered = true;
            ShowInTaskbar = false;
            Overlay.BackColor = Color.Transparent;

            Bitmap bitmap = new Bitmap(
                Screen.PrimaryScreen.Bounds.Width,
                Screen.PrimaryScreen.Bounds.Height,
                PixelFormat.Format32bppArgb
                );
            Screenshot = Graphics.FromImage(bitmap);
            BackgroundImage = bitmap;

            NotifyIcon = new NotifyIcon();
            NotifyIcon.Text = Application.ProductName;
            NotifyIcon.Icon = new Icon(SystemIcons.Application, 16, 16);
            NotifyIcon.Visible = true;
            NotifyIcon.BalloonTipClicked += delegate(object sender, EventArgs e)
            {
                Clipboard.SetText(LastUploadedUrl);
            };
            NotifyIcon.ContextMenuStrip = new ContextMenuStrip();
            NotifyIcon.ContextMenuStrip.Items.Add("Exit", null, delegate(object sender, EventArgs e) 
            {
                Application.Exit();
            });
        }

        void RegisterHotKey()
        {
            KeyboardHook = new KeyboardHook();
            KeyboardHook.RegisterHotKey(
                Screencapture.ModifierKeys.Control | Screencapture.ModifierKeys.Shift,
                Keys.D4
                );
            KeyboardHook.RegisterHotKey(
                Screencapture.ModifierKeys.Control | Screencapture.ModifierKeys.Shift,
                Keys.D3
                );
            KeyboardHook.KeyPressed += delegate(object sender, HotKeyPressEventArgs e)
            {
                if (e.Key == Keys.D3)
                {
                    ImageUploader.Upload(MakeScreenshot());
                }
                else if (e.Key == Keys.D4)
                {
                    Screenshot.DrawImage(MakeScreenshot(), 0, 0);
                    TopMost = true;
                    Visible = true;
                    Focus();
                    Cursor = Cursors.Default;
                }
            };
        }

        void CreateImageUploader()
        {
            ImageUploader = new ImageUploader(UploadAddress);
            ImageUploader.Uploaded += delegate(object sender, UploadDataCompletedEventArgs e)
            {
                try
                {
                    LastUploadedUrl = Encoding.ASCII.GetString(e.Result, 0, e.Result.Length);
                    NotifyIcon.ShowBalloonTip(
                        10000,
                        "Screenshot uploaded",
                        LastUploadedUrl,
                        ToolTipIcon.Info
                        );
                }
                catch (TargetInvocationException ex)
                {
                    NotifyIcon.ShowBalloonTip(
                        1000,
                        "Upload failed",
                        ex.InnerException.Message,
                        ToolTipIcon.Error
                        );
                }
            };
        }

        Bitmap MakeScreenshot()
        {
            Size screenSize = Screen.PrimaryScreen.Bounds.Size;
            IntPtr hDesk = NativeMethods.GetDesktopWindow();
            IntPtr hSrce = NativeMethods.GetWindowDC(hDesk);
            IntPtr hDest = NativeMethods.CreateCompatibleDC(hSrce);
            IntPtr hBmp = NativeMethods.CreateCompatibleBitmap(
                hSrce,
                screenSize.Width,
                screenSize.Height
                );
            IntPtr hOldBmp = NativeMethods.SelectObject(hDest, hBmp);
            bool b = NativeMethods.BitBlt(
                hDest,
                0,
                0,
                screenSize.Width,
                screenSize.Height,
                hSrce,
                0,
                0,
                CopyPixelOperation.SourceCopy | CopyPixelOperation.CaptureBlt
                );
            Bitmap bmp = Bitmap.FromHbitmap(hBmp);
            NativeMethods.SelectObject(hDest, hOldBmp);
            NativeMethods.DeleteObject(hBmp);
            NativeMethods.DeleteDC(hDest);
            NativeMethods.ReleaseDC(hDesk, hSrce);

            return bmp;
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Maximized;
            Visible = false;
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Visible = false;
                SelectionStarted = false;
            }
        }

        private void Overlay_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(
                new SolidBrush(Color.FromArgb(64, Color.Black)),
                0,
                0,
                Width,
                Height
                );

            if (SelectionStarted)
            {
                e.Graphics.DrawImage(BackgroundImage, SelectionRectangle, SelectionRectangle, GraphicsUnit.Pixel);
                e.Graphics.DrawRectangle(Pens.Silver, SelectionRectangle);
            }
        }

        private void Overlay_MouseDown(object sender, MouseEventArgs e)
        {
            SelectionRectangle = new Rectangle(e.Location, new Size(0, 0));
            SelectionStartPoint = e.Location;
            SelectionStarted = true;
            Overlay.Invalidate();
        }

        private void Overlay_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.X > SelectionStartPoint.X)
            {
                SelectionRectangle.X = SelectionStartPoint.X;
                SelectionRectangle.Width = e.X - SelectionStartPoint.X;
            }
            else
            {
                SelectionRectangle.X = e.X;
                SelectionRectangle.Width = SelectionStartPoint.X - e.X;
            }

            if (e.Y > SelectionStartPoint.Y)
            {
                SelectionRectangle.Y = SelectionStartPoint.Y;
                SelectionRectangle.Height = e.Y - SelectionStartPoint.Y;
            }
            else
            {
                SelectionRectangle.Y = e.Y;
                SelectionRectangle.Height = SelectionStartPoint.Y - e.Y;
            }

            Overlay.Invalidate();
        }

        private void Overlay_MouseUp(object sender, MouseEventArgs e)
        {
            if (!SelectionStarted)
            {
                return;
            }

            Visible = false;
            SelectionStarted = false;

            Bitmap bitmap = new Bitmap(SelectionRectangle.Width, SelectionRectangle.Height);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.DrawImage(
                BackgroundImage,
                new Rectangle(new Point(0, 0), SelectionRectangle.Size),
                SelectionRectangle,
                GraphicsUnit.Pixel
                );
            ImageUploader.Upload(bitmap);
        }
    }
}
