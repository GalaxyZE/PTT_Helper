#region FormView
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PTT_Helper
{
    public partial class FormView : Form
    {
        //#region AeroDefine
        //[StructLayout(LayoutKind.Sequential)]
        //public struct MARGINS
        //{
        //    public int Left;
        //    public int Right;
        //    public int Top;
        //    public int Bottom;
        //}
        ////DLL申明
        //[DllImport("dwmapi.dll", PreserveSig = false)]
        //static extern void DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS
        //margins);
        ////DLL申明
        //[DllImport("dwmapi.dll", PreserveSig = false)]
        //static extern bool DwmIsCompositionEnabled();
        ////直接添加代碼
        //protected override void OnLoad(EventArgs e)
        //{
        //    if (DwmIsCompositionEnabled())
        //    {
        //        MARGINS margins = new MARGINS();
        //        margins.Right = margins.Left = margins.Top = margins.Bottom =
        //        this.Width + this.Height;
        //        DwmExtendFrameIntoClientArea(this.Handle, ref margins);
        //    }
        //    base.OnLoad(e);
        //}
        //protected override void OnPaintBackground(PaintEventArgs e)
        //{
        //    base.OnPaintBackground(e);
        //    if (DwmIsCompositionEnabled())
        //    {
        //        e.Graphics.Clear(Color.Black);
        //    }
        //}

        ////-------------------------------------------------------------
        //[DllImport("kernel32.dll", SetLastError = true)]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //static extern bool AllocConsole();

        //[DllImport("Kernel32")]
        //public static extern void FreeConsole();
        //#endregion

        public FormView()
        {
            InitializeComponent();
        }
        public FormView(String arg)
        {
            InitializeComponent();
            label1.Text = arg;
        }

        private void FormView_Load(object sender, EventArgs e)
        {
            textBox1.Visible = false;
            this.WindowState = FormWindowState.Maximized;
        }
        public string SetValue
        {
            set
            {
                label1.Text = value;
                textBox1.Text = label1.Text;
            }
            get
            {
                return label1.Text;
            }
        }

        private void Label1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            textBox1.Visible = !textBox1.Visible;
            label1.Visible = !label1.Visible;
        }

        private void TextBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            textBox1.Visible = !textBox1.Visible;
            label1.Visible = !label1.Visible;
        }

    

        private void FormView_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
                this.Close();

        }
    }
}
#endregion