using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using System.Security.Permissions;
using System.Security;
using System.Security.AccessControl;
using System.Reflection;


namespace portapapeles
{
    public partial class Form1 : Form
    {
        int numImage = 1;
        int width=160;
        int height=120;
        IntPtr nextClipboardViewer;
        bool defaultRoute = true;
        string ruta="imagen.jpg";
        bool original=false;
        private string[] args;
        Image img;

        static ImageFormat formato=ImageFormat.Jpeg;
             
        public Form1()
        {
           
            
          
        }



        public Form1(string[] args)
        {
            InitializeComponent();
            nextClipboardViewer = (IntPtr)SetClipboardViewer((int)this.Handle);
           
            comboBox1.SelectedIndex = 0;

            bool success = Form1.RegisterHotKey(this.Handle, this.GetType().GetHashCode(), 0x0001, 0x41);
            do
            {
                ruta = System.Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\imagen" + numImage + ".jpg";
                numImage++;
            } while (File.Exists(ruta));
    
            txtruta.Text = ruta;
            this.args = args;
            

            if(args.Length>0)
            if(args[0]=="/MINIMIZED")
                this.WindowState = FormWindowState.Minimized;

           
        
                
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
        if(File.Exists(ruta))
            System.Diagnostics.Process.Start(ruta);
        }

        [DllImport("User32.dll")]
        protected static extern int
        SetClipboardViewer(int hWndNewViewer);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool
        ChangeClipboardChain(IntPtr hWndRemove,
        IntPtr hWndNewNext);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hwnd, int wMsg,
        IntPtr wParam,
        IntPtr lParam);

        protected override void
WndProc(ref System.Windows.Forms.Message m)
        {

            
                
            
          
                // defined in winuser.h
                const int WM_DRAWCLIPBOARD = 0x308;
                const int WM_CHANGECBCHAIN = 0x030D;
                const int WM_SHORTCUT = 0x0312;

                switch (m.Msg)
                {
                    case WM_SHORTCUT: this.capturaSelecction(); break;

                    case WM_DRAWCLIPBOARD:
                        DisplayClipboardData();
                        SendMessage(nextClipboardViewer, m.Msg, m.WParam,
                        m.LParam);
                        break;

                    case WM_CHANGECBCHAIN:
                        if (m.WParam == nextClipboardViewer)
                            nextClipboardViewer = m.LParam;
                        else
                            SendMessage(nextClipboardViewer, m.Msg, m.WParam,
                            m.LParam);
                        break;

                    default:
                        base.WndProc(ref m);
                        break;
                
            }
            
        }

        private void DisplayClipboardData()
        {

            if (iniciarToolStripMenuItem.Checked)
            {


                try
                {
                    IDataObject iData = new DataObject();
                    iData = Clipboard.GetDataObject();
                    //if (iData.GetDataPresent(DataFormats.Rtf))
                       // richTextBox1.Rtf = (string)iData.GetData(DataFormats.Rtf);
                   // else if (iData.GetDataPresent(DataFormats.Text))
                        //richTextBox1.Text = (string)iData.GetData(DataFormats.Text);
                    //else
                        if (iData.GetDataPresent(DataFormats.Bitmap))
                        {


                            if (original)
                                img = (Image)iData.GetData(DataFormats.Bitmap);
                            else
                                img = Redimensionar((Image)iData.GetData(DataFormats.Bitmap), width, height, 90);
                            
                            //pictureBox1.Image = Redimensionar((Image)iData.GetData(DataFormats.Bitmap), 160, 120, 90);
            

                            if (defaultRoute)
                            {

                                ruta = System.Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\imagen" + numImage + ".jpg";


                                //ruta = Application.StartupPath+"\\imagen" + numImage + ".jpg";
                            

                                img.Save(ruta, formato);
                            }
                            else


                                if (!defaultRoute)
                                {
                                   

                                    do
                                    {
                                      
                                        ruta = folderBrowserDialog1.SelectedPath + @"\imagen" + numImage + ".jpg";
                                        numImage++;
                                    } while (File.Exists(ruta));


                                    FileIOPermission f3 = new FileIOPermission(FileIOPermissionAccess.Write, ruta);

                                    try
                                    {
                                        f3.Demand();
                                    }
                                    catch (SecurityException s)
                                    {
                                        Console.WriteLine(s.Message);
                                    }
                                    img.Save(ruta, formato);
                                }
                            toolStripStatusLabel2.Text = ruta;
                            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                            pictureBox1.ImageLocation = ruta;

                            numImage++;
                            txtruta.Text = ruta;
                            if (notificacionesToolStripMenuItem.Checked)
                            
                            if (!original)
                                notifyIcon1.ShowBalloonTip(1, "Imagen Guardada " + width + "x" + height, ruta, ToolTipIcon.None);
                            else
                                notifyIcon1.ShowBalloonTip(1, "Imagen Guardada " + img.Width + "x" + img.Height, ruta, ToolTipIcon.None);
                            

                        }
                       

                }
                catch (Exception e)
                {

                    MessageBox.Show(e.ToString());
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
            
        
          
        }

      

        public static Image Redimensionar(Image Imagen, int Ancho, int Alto, int resolucion)
        {
            //Bitmap sera donde trabajaremos los cambios
            using (Bitmap imagenBitmap = new Bitmap(Ancho, Alto, PixelFormat.Format32bppRgb))
            {
                imagenBitmap.SetResolution(resolucion, resolucion);
                //Hacemos los cambios a ImagenBitmap usando a ImagenGraphics y la Imagen Original(Imagen)
                //ImagenBitmap se comporta como un objeto de referenciado
                using (Graphics imagenGraphics = Graphics.FromImage(imagenBitmap))
                {
                    imagenGraphics.SmoothingMode = SmoothingMode.AntiAlias;
                    imagenGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    imagenGraphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    imagenGraphics.DrawImage(Imagen, new Rectangle(0, 0, Ancho, Alto), new Rectangle(0, 0, Imagen.Width, Imagen.Height), GraphicsUnit.Pixel);
                    //todos los cambios hechos en imagenBitmap lo llevaremos un Image(Imagen) con nuevos datos a travez de un MemoryStream
                    MemoryStream imagenMemoryStream = new MemoryStream();
                    imagenBitmap.Save(imagenMemoryStream, formato);
                    Imagen = Image.FromStream(imagenMemoryStream);
            
                }
            }
            return Imagen;
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {

                case 0: height = 120;
                    width = 160; original = false;
                    
                    x120ToolStripMenuItem.Checked = true;
                    x160ToolStripMenuItem.Checked = false;
                    x240ToolStripMenuItem.Checked = false;
                    x480ToolStripMenuItem.Checked = false;
                    originalToolStripMenuItem.Checked = false;


                    break;
                case 1: height = 160;
                    width = 180; original = false; 
                    x120ToolStripMenuItem.Checked = true;
                    x160ToolStripMenuItem.Checked = false;
                    x240ToolStripMenuItem.Checked = false;
                    x480ToolStripMenuItem.Checked = false;
                    originalToolStripMenuItem.Checked = false;
                    break;

                case 2: width = 320;
                    height = 240; original = false; 
                    x120ToolStripMenuItem.Checked = false;
                    x160ToolStripMenuItem.Checked = false;
                    x240ToolStripMenuItem.Checked = true;
                    x480ToolStripMenuItem.Checked = false;
                    originalToolStripMenuItem.Checked = false;
                    
                    break;
                case 3: width = 640; 
                    height = 480; original = false; 
                    
                    x120ToolStripMenuItem.Checked = false;
                    x160ToolStripMenuItem.Checked = false;
                    x240ToolStripMenuItem.Checked = false;
                    x480ToolStripMenuItem.Checked = true;
                    originalToolStripMenuItem.Checked = false;
                    break;
                default: original = true;
                    x120ToolStripMenuItem.Checked = false;
                    x160ToolStripMenuItem.Checked = false;
                    x240ToolStripMenuItem.Checked = false;
                    x480ToolStripMenuItem.Checked = false;
                    originalToolStripMenuItem.Checked = true;
                    
                    break;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                ruta = folderBrowserDialog1.SelectedPath;

                ruta = ruta + @"\imagen" + numImage + ".jpg";
                txtruta.Text = ruta;
                defaultRoute = false;
                numImage = 1;
            }


        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.abrirCarpeta();

        }
        private void abrirCarpeta()
        {
            string r = "";

            if (defaultRoute)
                r = System.Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            else
                r = folderBrowserDialog1.SelectedPath;
            System.Diagnostics.Process.Start(r);
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Visible = true;
         
            this.WindowState = FormWindowState.Normal;
          
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1.UnregisterHotKey(this.Handle, this.GetType().GetHashCode());
            this.Dispose();
            
           
        }

        private void detenerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            detenerToolStripMenuItem.Checked = true;
            iniciarToolStripMenuItem.Checked = false;
            notifyIcon1.Text = "ClipSaveImage Detenido";
            if (notificacionesToolStripMenuItem.Checked)
            notifyIcon1.ShowBalloonTip(1, "ClipSaveImage","Detenido", ToolTipIcon.None);
        }

        private void iniciarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            detenerToolStripMenuItem.Checked = false;
            iniciarToolStripMenuItem.Checked = true;
           notifyIcon1.Text = "ClipSaveImage Monitoreando";
           if (notificacionesToolStripMenuItem.Checked)
           notifyIcon1.ShowBalloonTip(1, "ClipSaveImage", "Iniciado", ToolTipIcon.None);
        }

        private void Form1_ResizeBegin(object sender, EventArgs e)
        {
           
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            
           
            if (this.WindowState == FormWindowState.Minimized)
            {
                
                this.Visible = false;
          
           
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {  e.Cancel = true;
            this.Visible = false;
            if (notificacionesToolStripMenuItem.Checked)
            notifyIcon1.ShowBalloonTip(1, "ClipSaveImage", "continua ejecutándose", ToolTipIcon.None);
            
          
        }

        private void notifyIcon1_BalloonTipClicked(object sender, EventArgs e)
        {
            try
            {

              
                System.Diagnostics.Process.Start(ruta);

            }
            catch (Exception) { 
            
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void cambiarRutaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                ruta = folderBrowserDialog1.SelectedPath;

                ruta = ruta + @"\imagen" + numImage + ".jpg";
                txtruta.Text = ruta;
                defaultRoute = false;
                numImage = 1;

                if (notificacionesToolStripMenuItem.Checked)
                notifyIcon1.ShowBalloonTip(1,"Nueva ruta",ruta,ToolTipIcon.None);
             
            }

        }

        private void notificacionesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            notificacionesToolStripMenuItem.Checked = !notificacionesToolStripMenuItem.Checked;
        }

        private void iniciarConToolStripMenuItem_Click(object sender, EventArgs e)
        {

           
            string keyName = "ClipSaveImage";
            string assemblyLocation = "\"" + Assembly.GetExecutingAssembly().Location + "\" /MINIMIZED";  // Or the EXE path.


            iniciarConToolStripMenuItem.Checked = !iniciarConToolStripMenuItem.Checked;


            if (iniciarConToolStripMenuItem.Checked)
            {
               
             
                    Util.SetAutoStart(keyName, assemblyLocation);
               
            }
            else
                
                    Util.UnSetAutoStart(keyName);

        }

        private void pantallaToolStripMenuItem_Click(object sender, EventArgs e)
        {


            if (defaultRoute)
                ruta = System.Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\imagen" + numImage + ".jpg";
            else
                ruta = folderBrowserDialog1.SelectedPath + @"\imagen" + numImage + ".jpg";
            try
            
            {
                //Estas líneas son necesarias para ocultar totalmente el windows form de la captura de pantalla.
                this.Hide();
                System.Threading.Thread.Sleep(250);
                //Obtenemos la resolución de pantalla
                Rectangle escritorio = Screen.GetBounds(this.ClientRectangle);
                //Creamos un Bitmap del tamaño de nuestra pantalla
                using (Bitmap b = new Bitmap(escritorio.Width, escritorio.Height))
                {
                    //Creamos el gráifco en base al Bitmap 
                    using (Graphics g = Graphics.FromImage(b))
                    {
                        //Transferimos la captura al objeto g en base a las mediad del bitmap
                        g.CopyFromScreen(0, 0, 0, 0, b.Size);
                        //Almacenamos la captura
                        if (!original)
                        {
                            Image tmp = Redimensionar(b, width, height, 90);
                            tmp.Save(ruta, formato);
                            Clipboard.SetImage(tmp);    
                        }
                        else
                        {
                            b.Save(ruta, formato);
                            Clipboard.SetImage(b);
                        }

                        //Lo mostramos en el WindowsFom
                         pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                          pictureBox1.ImageLocation = ruta;
                        if(!original)
                        notifyIcon1.ShowBalloonTip(1, "Pantalla capturada "+width+"x"+height, ruta, ToolTipIcon.None);
                        else
                            notifyIcon1.ShowBalloonTip(1, "Pantalla capturada "+b.Width+"x"+b.Height, ruta, ToolTipIcon.None);
                        toolStripStatusLabel2.Text = ruta;
                        
                        numImage++;
                        
                        
                    }
                }
            }
            catch (InvalidEnumArgumentException ieae)
            {
                MessageBox.Show(ieae.ToString());
            }
            catch (Win32Exception we)
            {
                MessageBox.Show(we.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                this.Show();
            }
        }

        private void selecciónToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.capturaSelecction();
        }


        private void capturaSelecction()
        {

            this.Hide();
            System.Threading.Thread.Sleep(250);
            frmFondo _frmFondo = new frmFondo(ruta, formato,original,width,height);
            if (_frmFondo.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox1.ImageLocation = ruta;
                if(original)
                notifyIcon1.ShowBalloonTip(1, "Pantalla capturada ", ruta, ToolTipIcon.None);
                else
                notifyIcon1.ShowBalloonTip(1, "Pantalla capturada "+width+"x"+height, ruta, ToolTipIcon.None);

                toolStripStatusLabel2.Text = ruta;
                numImage++;


            }

            this.Show();
        }
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void capturarToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void x120ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            width = 160;
            height = 120;
            original = false;
            x120ToolStripMenuItem.Checked = true;
            x160ToolStripMenuItem.Checked = false;
            x240ToolStripMenuItem.Checked = false;
            x480ToolStripMenuItem.Checked = false;
            originalToolStripMenuItem.Checked = false;
            comboBox1.SelectedIndex = 0;

        }

        private void x160ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            width = 180;
            height = 160;
            original = false;
            x120ToolStripMenuItem.Checked = false;
            x160ToolStripMenuItem.Checked = true;
            x240ToolStripMenuItem.Checked = false;
            x480ToolStripMenuItem.Checked = false;
            originalToolStripMenuItem.Checked = false;
            comboBox1.SelectedIndex = 1;
        }

        private void x240ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            width = 320;
            height = 240;
            original = false;
            x120ToolStripMenuItem.Checked = false;
            x160ToolStripMenuItem.Checked = false;
            x240ToolStripMenuItem.Checked = true;
            x480ToolStripMenuItem.Checked = false;
            originalToolStripMenuItem.Checked = false;
            comboBox1.SelectedIndex = 2;
        }

        private void x480ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            width = 640;
            height = 480;
            original = false;
            x120ToolStripMenuItem.Checked = false;
            x160ToolStripMenuItem.Checked = false;
            x240ToolStripMenuItem.Checked = false;
            x480ToolStripMenuItem.Checked = true;
            originalToolStripMenuItem.Checked = false;
            comboBox1.SelectedIndex = 3;
        }

        private void originalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            original = true;
            x120ToolStripMenuItem.Checked = false;
            x160ToolStripMenuItem.Checked = false;
            x240ToolStripMenuItem.Checked = false;
            x480ToolStripMenuItem.Checked = false;
            originalToolStripMenuItem.Checked = true;
            comboBox1.SelectedIndex = 4;
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripStatusLabel3_Click(object sender, EventArgs e)
        {

        }

        private void abrirCarpetaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.abrirCarpeta();
        }

       


        

         
    }

}
