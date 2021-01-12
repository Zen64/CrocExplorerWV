﻿using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Media;
using System.Threading.Tasks;
using System.Windows.Forms;
using Be.Windows.Forms;

namespace CrocExplorerWV
{
    public partial class Form1 : Form
    {
        PIXFile currentPix;
        MODFile currentMod;
        Engine3D engine;
        SoundPlayer sp;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Log.box = rtb1;
            OpenFileDialog d = new OpenFileDialog();
            d.Filter = "croc.exe|croc.exe";
            if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FileSystem.Init(Path.GetDirectoryName(d.FileName) + "\\");
                this.TopMost = true;
                Application.DoEvents();
                this.TopMost = false;
                RefreshTree();
                RefreshList();
                engine = new Engine3D(pb2);
                timer1.Enabled = true;
            }
            else
                this.Close();
        }

        public void RefreshTree()
        {
            tv1.Nodes.Clear();
            foreach (IdxFile idx in FileSystem.idxFiles)
            {
                TreeNode nt = idx.ToTree();
                bool found = false;
                foreach(TreeNode t in tv1.Nodes)
                    if (t.Text == nt.Text)
                    {
                        t.Nodes.Add(nt.Nodes[0]);
                        found = true;
                        break;
                    }
                if(!found)
                    tv1.Nodes.Add(nt);
            }
        }

        public void RefreshList()
        {
            string[] files = Directory.GetFiles(FileSystem.basePath, "*.pix", SearchOption.AllDirectories);
            List<string> fileList = new List<string>();
            foreach (string file in files)
                fileList.Add(file);
            foreach (IdxFile idx in FileSystem.idxFiles)
                foreach (FileReference r in idx.refs)
                    if (r.name.ToLower().EndsWith(".pix"))
                        fileList.Add(idx.basepath + idx.filename + ">" + r.name);
            listBox1.Items.Clear();
            string filter = textBox1.Text.ToLower();
            foreach (string s in fileList)
                if (s.ToLower().Contains(filter))
                    listBox1.Items.Add(s);


            files = Directory.GetFiles(FileSystem.basePath, "*.mod", SearchOption.AllDirectories);
            fileList = new List<string>();
            foreach (string file in files)
                fileList.Add(file);
            foreach (IdxFile idx in FileSystem.idxFiles)
                foreach (FileReference r in idx.refs)
                    if (r.name.ToLower().EndsWith(".mod"))
                        fileList.Add(idx.basepath + idx.filename + ">" + r.name);
            listBox2.Items.Clear();
            filter = textBox2.Text.ToLower();
            foreach (string s in fileList)
                if (s.ToLower().Contains(filter))
                    listBox2.Items.Add(s);


            fileList = new List<string>();
            foreach (IdxFile idx in FileSystem.idxFiles)
                foreach (FileReference r in idx.refs)
                    if (r.name.ToLower().EndsWith(".wav"))
                        fileList.Add(idx.basepath + idx.filename + ">" + r.name);
            listBox3.Items.Clear();
            filter = textBox3.Text.ToLower();
            foreach (string s in fileList)
                if (s.ToLower().Contains(filter))
                    listBox3.Items.Add(s);
        }

        private void tv1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode sel = e.Node;
            IdxFile idx = null;
            FileReference fr = null;
            GetFileReference(sel, out idx, out fr);
            if (fr != null)
            {
                byte[] data = idx.LoadEntry(fr);
                hb1.ByteProvider = new DynamicByteProvider(data);
            }
        }

        private void exportRAWToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog d = new SaveFileDialog();
            d.Filter = "*.*|*.*";
            if (tv1.SelectedNode != null)
                d.FileName = tv1.SelectedNode.Text;
            if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                MemoryStream m = new MemoryStream();
                for (int i = 0; i < hb1.ByteProvider.Length; i++)
                    m.WriteByte(hb1.ByteProvider.ReadByte(i));
                File.WriteAllBytes(d.FileName, m.ToArray());
                Log.WriteLine("Saved to " + d.FileName);
            }
        }

        private void importRAWToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IdxFile idx = null;
            FileReference fr = null;
            GetFileReference(tv1.SelectedNode, out idx, out fr);
            if (fr != null)
            {
                OpenFileDialog d = new OpenFileDialog();
                d.Filter = "*.*|*.*";
                if (tv1.SelectedNode != null)
                    d.FileName = tv1.SelectedNode.Text;
                if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    byte[] buff = File.ReadAllBytes(d.FileName);
                    uint ucsize = (uint)buff.Length;
                    if (fr.compression != 0)
                        buff = Helper.CompressRLE(buff, fr.compression);
                    idx.SaveEntry(fr, buff, ucsize);
                }
            }
        }

        public void GetFileReference(TreeNode sel, out IdxFile idxFile, out FileReference fileRef)
        {
            idxFile = null;
            fileRef = null;
            bool found = false;
            if (sel != null && sel.Parent != null && sel.Parent.Text.EndsWith(".idx"))
            {
                string idxpath = "";
                if (sel.Parent.Parent == null)
                    idxpath = sel.Parent.Text;
                else
                    idxpath = sel.Parent.Parent.Text.Substring(1) + "\\" + sel.Parent.Text;
                foreach (IdxFile idx in FileSystem.idxFiles)
                {
                    if (idx.filename == idxpath)
                        foreach (FileReference r in idx.refs)
                            if (r.name == sel.Text)
                            {
                                found = true;
                                fileRef = r;
                                break;
                            }
                    if (found)
                    {
                        idxFile = idx;
                        break;
                    }
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {            
            try
            {
                int n = comboBox1.SelectedIndex;
                if (n == -1)
                    return;
                PIXFile.PIXHeader h = currentPix.headers[n];
                PIXFile.PIXData d = currentPix.images[n];
                pb1.Image = PIXFile.MakeBitmap(h, d);
            }
            catch { }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            RefreshList();
        }

        private void listBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            try
            {
                string s = listBox1.SelectedItem.ToString();
                currentPix = new PIXFile(LoadFile(s));
                comboBox1.Items.Clear();
                int count = 1;
                foreach (PIXFile.PIXHeader h in currentPix.headers)
                    comboBox1.Items.Add(count++ + "/" + currentPix.headers.Count + " " + h.name);
                if (comboBox1.Items.Count != 0)
                    comboBox1.SelectedIndex = 0;
            }
            catch { }
        }

        private byte[] LoadFile(string s)
        {
            byte[] result = null;
            if (s.Contains(">"))
            {
                IdxFile idf;
                FileReference r;
                FileSystem.FindFile(s, out idf, out r);
                if (idf == null || r == null)
                    return result;
                result = idf.LoadEntry(r);
            }
            else
                result = File.ReadAllBytes(s);
            return result;
        }

        private void exportBMPToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (pb1.Image == null)
                return;
            SaveFileDialog d = new SaveFileDialog();
            d.Filter = "*.png|*.png";
            if (currentPix != null && comboBox1.SelectedIndex != -1 && currentPix.headers.Count > 0 && currentPix.images.Count > 0)
            {
                d.FileName = currentPix.headers[comboBox1.SelectedIndex].name;
                if (!d.FileName.ToLower().EndsWith(".png"))
                    d.FileName += ".png";
            }
            if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                pb1.Image.Save(d.FileName);
                Log.WriteLine("Saved to " + d.FileName);
            }
        }

        private void importPNGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1 && comboBox1.SelectedIndex != -1)
            {
                string s = listBox1.SelectedItem.ToString();
                byte[] pxData = LoadFile(s);
                PIXFile px = new PIXFile(pxData);
                if (px == null)
                    return;
                OpenFileDialog d = new OpenFileDialog();
                d.Filter = "*.png|*.png";
                if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Bitmap bmp = new Bitmap(d.FileName);
                    PIXFile.PIXHeader h = px.headers[comboBox1.SelectedIndex];
                    if (bmp.Width != h.Width || bmp.Height != h.Height)
                    {
                        MessageBox.Show("Imported image size doesnt match! (" + bmp.Width + "x" + bmp.Height + " vs " + h.Width + "x" + h.Height + ")");
                        return;
                    }
                    byte[] data = PIXFile.MakePixdata(h, bmp);
                    int start = (int)px.images[comboBox1.SelectedIndex]._fileOffset + 12;
                    for (int i = 0; i < data.Length; i++)
                        pxData[start + i] = data[i];
                    if (s.Contains(">"))
                    {

                        IdxFile idf;
                        FileReference r;
                        FileSystem.FindFile(s, out idf, out r);
                        if (idf == null || r == null)
                            return;
                        uint ucsize = (uint)pxData.Length;
                        if (r.compression != 0)
                            pxData = Helper.CompressRLE(pxData, r.compression);
                        idf.SaveEntry(r, pxData, ucsize);
                    }
                    else
                        File.WriteAllBytes(s, pxData);
                    Log.WriteLine("Saved to " + s);
                }
            }
        }

        private void exportAllPIXAsBMPToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            FolderBrowserDialog d = new FolderBrowserDialog();
            if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string output = d.SelectedPath + "\\";
                int count = 0;
                prog.Minimum = 0;
                prog.Maximum = listBox1.Items.Count;
                foreach (string entry in listBox1.Items)
                {
                    if ((count % 10) == 0)
                    {
                        prog.Value = count;
                        Application.DoEvents();                        
                    }
                    if (entry.Contains(">"))
                        ExportPNGfromPIX(LoadFile(entry), Path.GetFileName(entry.Split('>')[1]), output + Path.GetFileName(entry.Split('>')[0]) + "\\");
                    else
                        ExportPNGfromPIX(File.ReadAllBytes(entry), Path.GetFileName(entry), output);
                    count++;
                }
                prog.Value = 0;
                Log.WriteLine("Done.");
            }
        }

        private void ExportPNGfromPIX(byte[] data, string pixname, string output)
        {
            if (data == null)
                return;
            PIXFile px = new PIXFile(data);
            string dir = output;
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            if (px.headers.Count > 1)
            {
                dir += pixname;
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                dir += "\\";
            }
            for (int i = 0; i < px.headers.Count; i++)
            {
                PIXFile.PIXHeader h = px.headers[i];
                PIXFile.PIXData d = px.images[i];
                Bitmap bmp = PIXFile.MakeBitmap(h, d);
                string name = MakeName(dir + Sanitize(h.name));
                bmp.Save(name);
                Log.WriteLine("Saved " + name);
            }
        }

        private string MakeName(string name)
        {
            string result = name;
            if (File.Exists(result + ".png"))
            {
                int count = 0;
                while (File.Exists(result + "_" + count + ".png"))
                    count++;
                result += "_" + count + ".png";
            }
            else
                result += ".png";
            return result;
        }

        private string Sanitize(string s)
        {
            return s.Replace("?", "_QUESTIONMARK_")
                    .Replace(":", "_COLON_")
                    .Replace(";", "_SEMICOLON_")
                    .Replace(".", "_DOT_")
                    .Replace(",", "_COMMA_")
                    .Replace("<", "_LESS_")
                    .Replace(">", "_GREATER_")
                    .Replace("[", "_BRACKETOPEN_")
                    .Replace("]", "_BRACKETCLOSED_")
                    .Replace("%", "_PERCENT_")
                    .Replace("/", "_SLASH_")
                    .Replace("\\", "_BACKSLASH_")
                    .Replace("'", "_QUOTE_")
                    .Replace("\"", "_DBLQUOTE_")
                    .Replace("*", "_STAR_");
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string s = listBox2.SelectedItem.ToString();
                MODFile m = new MODFile(LoadFile(s));
                engine.ClearScene();
                foreach (MODFile.MODObject ob in m.obj)
                {
                    RenderObject o = new RenderObject(engine.device, RenderObject.RenderType.TriListWired, engine.defaultTexture, engine);
                    List<RenderObject.VertexWired> tmp = new List<RenderObject.VertexWired>();
                    foreach (MODFile.MODFace f in ob.faces)
                    {
                        tmp.Add(MakeV(ob.vertices[f.f1 - 1]));
                        tmp.Add(MakeV(ob.vertices[f.f2 - 1]));
                        tmp.Add(MakeV(ob.vertices[f.f3 - 1]));
                        if ((f.flags & 0x8) != 0)
                        {
                            tmp.Add(MakeV(ob.vertices[f.f2 - 1]));
                            tmp.Add(MakeV(ob.vertices[f.f3 - 1]));
                            tmp.Add(MakeV(ob.vertices[f.f4 - 1]));
                        }
                    }
                    o.verticesWired = tmp.ToArray();
                    o.InitGeometry();
                    engine.objects.Add(o);
                }
                engine.ResetCameraDistance();

            }
            catch { }
        }

        private RenderObject.VertexWired MakeV(MODFile.MODVector3 mv)
        {
            RenderObject.VertexWired v = new RenderObject.VertexWired();
            v.Position.X = mv.X;
            v.Position.Y = mv.Y;
            v.Position.Z = mv.Z;
            v.Position.W = 1;
            v.Color.Alpha = 255;
            return v;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            RefreshList();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (engine != null)
                engine.Render();
        }

        bool mouseUp = true;
        Point mouseLast = new Point(0, 0);

        private void pb2_MouseDown(object sender, MouseEventArgs e)
        {
            mouseUp = false;
            mouseLast = e.Location;
        }

        private void pb2_MouseMove(object sender, MouseEventArgs e)
        {
            if (!mouseUp)
            {
                int dx = e.X - mouseLast.X;
                int dy = e.Y - mouseLast.Y;
                engine.CamDis *= 1 + (dy * 0.01f);
                engine.CamRot += dx * 0.01f;
                mouseLast = e.Location;
            }

        }

        private void pb2_MouseUp(object sender, MouseEventArgs e)
        {
            mouseUp = true;
        }

        private void pb2_SizeChanged(object sender, EventArgs e)
        {
            if (engine != null)
                engine.Resize(pb2);
        }

        private void exportAsOBJToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int n = listBox2.SelectedIndex;
            if (n == -1)
                return;
            try
            {
                string s = listBox2.SelectedItem.ToString();
                MODFile m = new MODFile(LoadFile(s));
                SaveFileDialog d = new SaveFileDialog();
                d.Filter = "*.obj|*.obj";
                if (s.Contains(">"))
                    d.FileName = Path.GetFileNameWithoutExtension(s.Split('>')[1]) + ".obj";
                else
                    d.FileName = Path.GetFileNameWithoutExtension(s) + ".obj";
                if(d.ShowDialog() == DialogResult.OK)
                {
                    m.SaveToObj(d.FileName);
                    Log.WriteLine("Saved to " + d.FileName);
                }
            }
            catch { }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            RefreshList();
        }

        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            int n = listBox3.SelectedIndex;
            if (n == -1)
                return;
            MemoryStream m = new MemoryStream(LoadFile(listBox3.SelectedItem.ToString()));
            sp = new SoundPlayer(m);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (sp != null)
                sp.Play();
        }

        private void exportAsWAVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int n = listBox3.SelectedIndex;
            if (n == -1)
                return;
            string s = listBox3.SelectedItem.ToString();
            SaveFileDialog d = new SaveFileDialog();
            d.Filter = "*.wav|*.wav";
            d.FileName = Path.GetFileNameWithoutExtension(s.Split('>')[1]);
            if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                File.WriteAllBytes(d.FileName, LoadFile(s));
                Log.WriteLine("Saved to " + d.FileName);
            }
        }

        private void importWAVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int n = listBox3.SelectedIndex;
            if (n == -1)
                return;
            string s = listBox3.SelectedItem.ToString();
            OpenFileDialog d = new OpenFileDialog();
            d.Filter = "*.wav|*.wav";
            if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                byte[] data = File.ReadAllBytes(d.FileName);
                IdxFile idf;
                FileReference r;
                FileSystem.FindFile(s, out idf, out r);
                if (idf == null || r == null)
                    return;
                uint ucsize = (uint)data.Length;
                if (r.compression != 0)
                    data = Helper.CompressRLE(data, r.compression);
                idf.SaveEntry(r, data, ucsize);
                Log.WriteLine("Saved to " + s);
            }
        }

        private void exportAllAsWAVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog d = new FolderBrowserDialog();
            if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string output = d.SelectedPath + "\\";
                int count = 0;
                prog.Minimum = 0;
                prog.Maximum = listBox3.Items.Count;
                foreach (string entry in listBox3.Items)
                {
                    if ((count % 10) == 0)
                    {
                        prog.Value = count;
                        Application.DoEvents();
                    }

                    string splitEntry = entry.Split('>')[1];
                    File.WriteAllBytes(d.SelectedPath + "\\" + splitEntry, LoadFile(entry));
                    Log.WriteLine("Saved as " + splitEntry);
                    
                    count++;
                }
                prog.Value = 0;
                Log.WriteLine("Done.");
            }
        }
    }
}
