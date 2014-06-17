using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RefineCalculator
{
    public partial class MainForm : Form
    {
        private const string DefaultMessage = "请在左侧输入单个五行石的成功几率";

        private const int MaxSlots = 16;

        private const int VeryLargeCost = 10000;

        private const string TopMostOffString = "总在最前（关）";

        private const string TopMostOnString = "总在最前（开）";

        private static readonly string[] StoneLevels = new string[] { "一", "二", "三", "四", "五", "六" };

        private static readonly int[] StoneCosts = new int[] { 1, 4, 13, 29, 146, 377 };

        private TextBox[] inputBoxes = new TextBox[6];

        private int?[,] costTable;

        private int[,] pathTable = new int[10001, MaxSlots + 1];

        private int?[] chances = new int?[6];

        public MainForm()
        {
            InitializeComponent();
            this.inputBoxes[0] = this.textBox1;
            this.inputBoxes[1] = this.textBox2;
            this.inputBoxes[2] = this.textBox3;
            this.inputBoxes[3] = this.textBox4;
            this.inputBoxes[4] = this.textBox5;
            this.inputBoxes[5] = this.textBox6;
        }

        private void OnTextChanged(object sender, EventArgs e)
        {
            Array.Clear(this.chances, 0, this.chances.Length);
            this.costTable = new int?[10001, MaxSlots + 1];

            try
            {
                for (int i = 0; i < 6; i++)
                {
                    if (!string.IsNullOrWhiteSpace(this.inputBoxes[i].Text))
                    {
                        this.chances[i] = (int)(Convert.ToDouble(this.inputBoxes[i].Text) * 100);
                    }
                }
            }
            catch (Exception)
            {
                labelResult.Text = DefaultMessage;
                return;
            }

            int cost = this.Calculate(10000, MaxSlots);

            if (cost >= VeryLargeCost)
            {
                labelResult.Text = "当前输入的等级的石头不足以达到100%成功率。请输入更高等级的石头。";
            }
            else
            {
                int[] numberOfStones = new int[6];
                int remainChance = 10000;
                for (int i = MaxSlots; i > 0 && remainChance > 0; i--)
                {
                    int usedStone = this.pathTable[remainChance, i];
                    numberOfStones[usedStone]++;
                    remainChance -= this.chances[usedStone].Value;
                }

                StringBuilder result = new StringBuilder();
                for (int i = 0; i < 6; i++)
                {
                    if (numberOfStones[i] > 0)
                    {
                        result.AppendFormat("{0}级五行石{1}颗\n", StoneLevels[i], numberOfStones[i]);
                    }
                }

                labelResult.Text = result.ToString();
            }
        }

        private int Calculate(int remainChance, int remainSlots)
        {
            if (remainChance <= 0)
            {
                return 0;
            }

            if (remainSlots <= 0)
            {
                return VeryLargeCost;
            }

            if (this.costTable[remainChance, remainSlots].HasValue)
            {
                return this.costTable[remainChance, remainSlots].Value;
            }

            int min = VeryLargeCost;
            for (int i = 0; i < 6; i++)
            {
                if (this.chances[i].HasValue)
                {
                    int cost = this.Calculate(remainChance - this.chances[i].Value, remainSlots - 1) + StoneCosts[i];
                    if (cost < min)
                    {
                        min = cost;
                        this.pathTable[remainChance, remainSlots] = i;
                    }
                }
            }

            this.costTable[remainChance, remainSlots] = min;
            return min;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.TopMost = !this.TopMost;
            button1.Text = this.TopMost ? TopMostOnString : TopMostOffString;
        }
    }
}
