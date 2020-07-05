
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;
using System.Windows.Resources;

using Application = System.Windows.Application;
using MessageBox = System.Windows.Forms.MessageBox;

namespace WindowsFormsApp29
{
    public partial class Form1 : Form
    {
        string[] existtxt;
        (int, int) MatrizSize1;
        (int, int) MatrizSize2;
        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            Cursor customCursor = new Cursor(@"..\..\..\Matrix\Resources\Handwriting.cur");
            this.Cursor = customCursor;
        }
        async ValueTask<string> ReadAllFileAsync(string filename)
        {
            byte[] buffer = new byte[131072];
            using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                buffer = new byte[file.Length];
                while (true)
                {
                    if ((await file.ReadAsync(buffer, 0, buffer.Length)) <= 0)
                        break;
                }
            }
            return Encoding.UTF7.GetString(buffer);
        }
        private async void button1_Click(object sender = null, EventArgs e = null)
        {
            try
            {
                button2_Click();
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.InitialDirectory =  Directory.GetCurrentDirectory();
                openFileDialog.Multiselect = true;
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";

                if (DialogResult.OK == openFileDialog.ShowDialog())
                {
                    string[] pathfile = openFileDialog.FileNames;
                    if (pathfile.Length != 2)
                    {
                        throw new Exception("Please select 2 files!!!");
                    }
                    var t = Task.Run(() => StartProcessAsync(pathfile));
                    await t.ContinueWith((antecedent) => Task.Run(async () => await ShowBtn()), TaskContinuationOptions.OnlyOnRanToCompletion);
                }
            }
            catch (Exception er)
            { 
                if (MessageBox.Show(er.Message, "Eror!", MessageBoxButtons.RetryCancel, MessageBoxIcon.Information) == DialogResult.Retry) 
                { 
                    button1_Click();
                }
            }

        }

        public async Task StartProcessAsync(string[] pathfile)
        {
            int lenhts = 2;
            existtxt = new string[lenhts];
            string[] filename = new string[lenhts];
            var outer = Task.Factory.StartNew(() =>
            {
                var inner = Task.Factory.StartNew(async () =>
                {
                    Action<int> action = delegate (int ii)
                    {
                        existtxt[ii] = ReadAllFileAsync(pathfile[ii]).Result;
                        filename[ii] = Path.GetFileName(pathfile[ii]);
                    };
                    Parallel.For(0, lenhts, action);
                    await Task.Delay(1);
                }, TaskCreationOptions.AttachedToParent);
            });
            outer.Wait();
            textBox1.Text = filename[0];
            textBox2.Text = filename[1];
            var t = Task.Run(async () => MatrizSize1= await ShowMatrixAsync(MatrizSize1, existtxt[0], richTextBox1, label4));
            t= Task.Run(async () => MatrizSize2=await ShowMatrixAsync(MatrizSize2, existtxt[1], richTextBox2, label5));
            await t.ContinueWith((antecedent) => Task.Run(async() => await ShowBtn()), TaskContinuationOptions.OnlyOnRanToCompletion);
        }
        async Task ShowBtn()
        {
            button4.Enabled = true;
            button5.Enabled = true;
            button6.Enabled = true;
            button7.Enabled = true;
            button8.Enabled = true;
            button9.Enabled = true;
            await Task.Delay(100);
        }
        async ValueTask<(int,int)> ShowMatrixAsync((int,int) MatrixSize, string txt, RichTextBox richTextBox, Label label)
        {

            int i = 0;
            MatrixSize.Item1 = int.Parse(txt.Split()[0]);
            MatrixSize.Item2 = int.Parse(txt.Split()[1]);
            label.Text = $"{MatrixSize.Item1}×{MatrixSize.Item2}";
            int row = 0;
            int counter = 0;
            foreach (var el in txt.Trim().Split())
            {
                if (counter==2)
                {
                    if (i - 1 == MatrixSize.Item2)
                    {
                        row++;
                        richTextBox.Text += Environment.NewLine;
                        i = 0;
                    }
                    i++;
                    richTextBox.Text += el + " ";
                    await Task.Delay(50);
                }
                else
                {
                    counter++;
                }
            }
            return MatrixSize;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender=null, EventArgs e=null)
        {
            richTextBox1.Text = "";
            richTextBox2.Text = "";
            richTextBox3.Text = "";
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            label4.Text = "";
            label5.Text = "";
            button4.Enabled = false;
            button5.Enabled = false;
            button6.Enabled = false;
            button7.Enabled = false;
            button8.Enabled = false;
            button9.Enabled = false;

        }
        public async Task PushIntoRichTextBoxAsync(int[][] MatrixArray, RichTextBox richTextBox)
        {
            richTextBox.Text = "";

            for (int i=0;i<MatrixArray.Length;i++)
            {
                if (i != 0) {
                    richTextBox.Text += Environment.NewLine;
                }
                for (int ii = 0; ii < MatrixArray[i].Length; ii++)
                {
                    richTextBox.Text += MatrixArray[i][ii]+" ";
                    await Task.Delay(5);
                }
            }

        }
        int[][] Matrix1Array1;
        int[][] Matrix1Array2;
        async Task GetdataAsync()
        {
            Matrix1Array1 = new int[MatrizSize1.Item1][];
            for (int i = 0; i < Matrix1Array1.Length; i++)
            {
                Matrix1Array1[i] = new int[MatrizSize1.Item2];
            }
            int ii = 0;
            int row = 0;
            foreach (var el in richTextBox1.Text.Trim().Split())
            {
                if (el != "")
                {
                    if (ii == Matrix1Array1[row].Length)
                    {
                        row++;
                        ii = 0;
                    }
                    Matrix1Array1[row][ii] = int.Parse(el);
                    ii++;
                    await Task.Delay(1);
                }
                textBox3.Text = "Result:";

            }
            Matrix1Array2 = new int[MatrizSize2.Item1][];
            for (int i = 0; i < Matrix1Array2.Length; i++)
            {
                Matrix1Array2[i] = new int[MatrizSize2.Item2];
            }
            ii = 0;
            row = 0;
            foreach (var el in richTextBox2.Text.Trim().Split())
            {
                if (el != "")
                {
                    if (ii == Matrix1Array2[row].Length)
                    {
                        row++;
                        ii = 0;
                    }
                    Matrix1Array2[row][ii] = int.Parse(el);
                    ii++;
                    await Task.Delay(1);
                }
            }
        }
        int[][] Matrix1Array3;
        private async  void button4_ClickAsync(object sender, EventArgs e)
        {
            await GetdataAsync();
            Matrix1Array3 = new int[MatrizSize1.Item1][];
            for (int i = 0; i < Matrix1Array3.Length; i++)
            {
                Matrix1Array3[i] = new int[MatrizSize2.Item2];
            }
            label7.Text = $"{Matrix1Array3.Length}×{Matrix1Array3[0].Length}";
            int p = 0;
            if (Matrix1Array2.Length == Matrix1Array1[0].Length)
            {
                for (int i = 0; i < Matrix1Array3.Length; i++)
                {
                    for (int j = 0; j < Matrix1Array3[0].Length; j++)
                    {
                        for (int k = 0; k < Matrix1Array1[0].Length; k++)
                        {
                            p = p + Convert.ToInt32(Matrix1Array1[i][k]) * Convert.ToInt32(Matrix1Array2[k][j]);
                        }
                        Matrix1Array3[i][j] = Convert.ToInt32(p);
                        p = 0;
                    }
                }
                await PushIntoRichTextBoxAsync(Matrix1Array3, richTextBox3);
            }
            else
            {
                 MessageBox.Show("Количество столбцов 1-ой матрицы не равно количеству строк 2-ой матрицы.", "Eror", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
         void MakeTranspositionAsync(int[][] Matrix1Arr, RichTextBox richTextBox)
        {

            int[][] temp = new int[Matrix1Arr[0].Length - 1][];
            for (int i = 0; i < Matrix1Arr.Length; i++)
            {
                temp[i] = new int[Matrix1Arr[i].Length];
            }
            if (temp.Length < temp[0].Length)
            {
                for (int i = 0; i < temp.Length; i++)
                {
                    for (int j = 0; j < temp[0].Length - 1; j++)
                    {
                        temp[i][j] = Matrix1Arr[j][i];
                    }
                }
                int[] lastArr = Matrix1Arr[Matrix1Arr.GetLength(0) - 1];
                for (int i = 0; i < temp.Length; i++)
                {
                    temp[i][temp.Length] = lastArr[i];
                }
            }
            else
            {

            }
            Task.Run(async () => await PushIntoRichTextBoxAsync(temp, richTextBox));
        }
        private async void button6_ClickAsync(object sender, EventArgs e)
        {
            await GetdataAsync();
            await Task.Delay(10);
            await Task.Run(() => MakeTranspositionAsync(Matrix1Array1, richTextBox1));
        }

        private async void button7_ClickAsync(object sender, EventArgs e)
        {
            await GetdataAsync();
            await Task.Delay(10);
            await Task.Run(() => MakeTranspositionAsync(Matrix1Array2, richTextBox2));
        }

        private async void button5_Click(object sender, EventArgs e)
        {
            await GetdataAsync();
             Matrix1Array3 = new int[MatrizSize1.Item1][];
            for (int i = 0; i < Matrix1Array3.Length; i++)
            {
                Matrix1Array3[i] = new int[MatrizSize2.Item2];
            }
            if (Matrix1Array2.Length == Matrix1Array1.Length && Matrix1Array1[0].Length==Matrix1Array2[0].Length)
            {
                for (int i = 0; i < Matrix1Array3.Length; i++)
                {
                    for(int j = 0; j < Matrix1Array3[i].Length; j++)
                    {
                        Matrix1Array3[i][j] = Matrix1Array1[i][j] + Matrix1Array2[i][j];
                    }
                }
                await PushIntoRichTextBoxAsync(Matrix1Array3, richTextBox3);
                label7.Text = $"{Matrix1Array3.Length}×{Matrix1Array3[0].Length}";
            }
            else
            {
                MessageBox.Show("Матрицы имеют разную размерность!!!", "Eror!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private async void button8_ClickAsync(object sender, EventArgs e)
        {
            await GetdataAsync();
            await Task.Delay(10);
            await Task.Run(() => MakeTranspositionAsync(Matrix1Array3, richTextBox3));
        }

        private void groupBox5_Enter(object sender, EventArgs e)
        {

        }

        private async void button9_Click(object sender, EventArgs e)
        {
            await GetdataAsync();
            int[][] Matrix1Array3 = new int[MatrizSize1.Item1][];
            for (int i = 0; i < Matrix1Array3.Length; i++)
            {
                Matrix1Array3[i] = new int[MatrizSize2.Item2];
            }
            if (Matrix1Array2.Length == Matrix1Array1.Length && Matrix1Array1[0].Length == Matrix1Array2[0].Length)
            {
                for (int i = 0; i < Matrix1Array3.Length; i++)
                {
                    for (int j = 0; j < Matrix1Array3[i].Length; j++)
                    {
                        Matrix1Array3[i][j] = Matrix1Array1[i][j] - Matrix1Array2[i][j];
                    }
                }
                await PushIntoRichTextBoxAsync(Matrix1Array3, richTextBox3);
                label7.Text = $"{Matrix1Array3.Length}×{Matrix1Array3[0].Length}";
            }
            else
            {
                MessageBox.Show("Матрицы имеют разную размерность!!!", "Eror!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
