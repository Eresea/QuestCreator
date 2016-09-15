using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DialogueCreator3
{
    public partial class Form1 : Form
    {
        private node N;
        public List<node> InactiveNode1s;                //BIGGEST ISSUE RIGHT NOW   Opt1: Use a greater class to get the inactive nodes properly
        public KeyValuePair<int, node> SelectedNode1Answer;
        public KeyValuePair<int, node> SelectedNode1Question;
        public node RightClicked;

        private Point moveInitial = new Point();
        private Size AddedSize = new Size();

        public Form1()
        {
            InitializeComponent();
        }

        public static int Clamp(int val, int min, int max)
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        private Point ArrangePlacing(Point p)
        {
            return new Point(p.X, Clamp(p.Y, 24, int.MaxValue));
        }

        public void Reset()
        {
            SelectedNode1Answer.Value.VisualBlock.Deselect(-1, true);
            SelectedNode1Question.Value.VisualBlock.Deselect(-1, true);
            SelectedNode1Answer = new KeyValuePair<int, node>(-1, null);
            SelectedNode1Question = new KeyValuePair<int, node>(-1, null);
        }

        private PointF Normalize(Point A)
        {
            float length = (float)Math.Sqrt(A.X * A.X + A.Y * A.Y);
            return new PointF((A.X / length), (A.Y / length));
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            N = new node(0);
            N.Form = this;
            Block tmp = N.SetBlock(new Point(0,24),new Size(new Point(100,100)),N.ID);
            tmp.ID = 0;
            tmp.SelectedAnswer += new EventHandler(SelectedAnswer);
            tmp.Delink += new EventHandler(Delink);
            tmp.Click += new EventHandler(BlockClick);
            N.Question = "Begin";
            N.ID = 0;

            InactiveNode1s = new List<node>();

        }

        public void SelectedAnswer(object sender, EventArgs e)
        {
            Block tmp = (Block)sender;

            node temp = (N.Find(tmp.ID));
            if(temp == null)
            {
                foreach(node n in InactiveNode1s)
                {
                    if(n.ID == tmp.ID)
                    {
                        temp = n;
                        break;
                    }
                }
            }
            if(tmp.SelectedNode1 == 0) // Input (Question)
            {
                SelectedNode1Question = new KeyValuePair<int, node>(tmp.selected, temp);
                if (SelectedNode1Answer.Value != null)
                {
                    node a = SelectedNode1Answer.Value;
                    a.Link(SelectedNode1Answer.Key, temp);
                    Reset();
                }
            }
            else // Outputs (Answers)
            {
                SelectedNode1Answer = new KeyValuePair<int, node>(tmp.SelectedNode1, temp);
                if (SelectedNode1Question.Value != null)
                {
                    temp.Link(SelectedNode1Question.Key, temp);
                    Reset();
                }
            }
        }

        public void SelectedQuestion(object sender, EventArgs e) // Change structure (to make sure we still have SelectedNode1Question & Answer but not 2 events)
        {
            Block tmp = (Block)sender;
            node temp = (N.Find(tmp.ID));
            if(temp == null)
            {
                foreach(node n in InactiveNode1s)
                {
                    if(n.ID == tmp.ID)
                    {
                        temp = n;
                        break;
                    }
                }
            }
            SelectedNode1Question = new KeyValuePair<int, node>(tmp.selected, temp);
            if (SelectedNode1Answer.Value != null)
            {
                node a = SelectedNode1Answer.Value;
                a.Link(SelectedNode1Answer.Key, temp);
                Reset();
            }
        }

        public void BlockClick(object sender, EventArgs e)
        {
            MouseEventArgs a = (MouseEventArgs)e;
            Block b = (Block)sender;
            if(a.Button == MouseButtons.Right)
            {
                RightClicked = N.Find(b.ID);
                if(RightClicked == null)
                {
                    foreach(node n in InactiveNode1s)
                    {
                        if(n.ID == b.ID) { RightClicked = n; break; }
                    }
                }
                Point p = new Point(0, b.Height-50);
                p = b.PointToScreen(p);
                contextMenuStrip1.Show(p);
            }

        }

        public void Delink(object sender, EventArgs e)
        {
            Block b = (Block)sender;
            b.Deselect(-1, false);
            node tmp = N.Find(b.ID);
            if(tmp == null)
            {
                foreach (node n in InactiveNode1s)
                {
                    if (n.ID == b.ID) { tmp = n; break; }
                }
            }
            if(b.selected >= 0) // Si c'est un output qui a été delink
            {
                if(tmp.NextNode1s.Count() >= b.selected+1)
                {
                    if (tmp.NextNode1s[b.selected + 1] != null)
                    {
                        tmp.NextNode1s.RemoveAt(b.selected + 1); // Check back the link() later (possible issue with Add() when you choose not to link the first output in order)
                        tmp.Answers.RemoveAt(b.selected + 1);
                    }
                }
            }
            else
            {
                MessageBox.Show(b.selected.ToString());
                for(int i=0;i<tmp.Parent.NextNode1s.Count();i++)
                {
                    if(tmp.Parent.NextNode1s[i] == tmp) { tmp.Parent.NextNode1s.RemoveAt(i);tmp.Parent.Answers.RemoveAt(i); break; }
                }
            }
        }

        private void Form1_Scroll(object sender, ScrollEventArgs e)
        {
            //AutoScrollMargin += new Size(10, 10);
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                node n = new node(0);
                InactiveNode1s.Add(n);
                InactiveNode1s.Last().Form = this;
                Point p = ArrangePlacing(new Point(e.X-50, e.Y-50));
                Block tmp = InactiveNode1s.Last().SetBlock(p, new Size(new Point(100, 100)),n.ID);
                tmp.ID = n.ID;
                tmp.SelectedAnswer += new EventHandler(SelectedAnswer);
                tmp.SelectedQuestion += new EventHandler(SelectedQuestion);
                tmp.Click += new EventHandler(BlockClick);
                tmp.Delink += new EventHandler(Delink);
                n.Question = "Test" + n.ID.ToString();
                // If time clicking > 0.2 (MouseUp -> Reset)
            }
        }

        private void runToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Clipboard.SetText("(" + N.Run());
        }

        private void closeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
            if(saveFileDialog1.FileName != "")
            {
                System.IO.StreamWriter file = new System.IO.StreamWriter(saveFileDialog1.FileName);
                file.WriteLine("(" + N.Run());
                file.Close();
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
            if(System.IO.File.Exists(saveFileDialog1.FileName))
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(saveFileDialog1.FileName);
                OpenFile(sr.ReadLine());
            }
        }

        private void RemoveAll()
        {
            N.Delete();
            foreach (node n in InactiveNode1s)
            {
                n.Delete();
            }
        }

        private void OpenFile(string s)
        {
            bool done = false;
            bool Set = false;
            List<KeyValuePair<KeyValuePair<int, int>, int>> NewLinks = new List<KeyValuePair<KeyValuePair<int, int>, int>>(); // List((first node ID, first node answer),second node ID)

            string temp = s;
            string Question;
            Point p = new Point(0, 50);
            while(!done)
            {
                int pos1 = temp.IndexOf("Id=") + 3;
                node n = new node(0);
                if(!Set) { RemoveAll(); N = n; }
                n.ID = int.Parse((temp.Substring(pos1, pos1 - temp.IndexOf(",")+2)));
                pos1 = temp.IndexOf("QuestionText=\"")+14;

                Question = temp.Substring(pos1, temp.Substring(pos1).IndexOf("\""));
                InactiveNode1s.Add(n);
                InactiveNode1s.Last().Form = this;
                bool done1 = !(temp.Contains("Answers"));
                if(done1) { pos1 = temp.IndexOf(")") + 1; temp = temp.Substring(pos1); }
                else { pos1 = temp.IndexOf("Answers = (") + 11; temp = temp.Substring(pos1); }

                while(!done1)
                {
                    pos1 = temp.IndexOf("AnswerText=\"")+12;
                    n.Answers.Add(temp.Substring(pos1, temp.Substring(pos1).IndexOf("\""))); // Answer
                    pos1 = temp.IndexOf("NextQuestionId=")+15;
                    NewLinks.Add(new KeyValuePair<KeyValuePair<int, int>, int>(new KeyValuePair<int, int>(n.ID, n.Answers.Count()-1),int.Parse(temp.Substring(pos1, temp.Substring(pos1).IndexOf(")")))));
                    temp = temp.Substring(pos1+1);
                    done1 = (temp.Substring(temp.IndexOf(")")+1,1) == ")"); // Si la lettre suivant la prochaine paranthèse fermée (fin de answer) est une fin de answerS
                    temp = temp.Substring(temp.IndexOf(")") + 2);
                }
                temp = temp.Substring(1);

                Block tmp = InactiveNode1s.Last().SetBlock(p, new Size(new Point(100, 100)), n.ID);
                tmp.ID = n.ID;
                tmp.SelectedAnswer += new EventHandler(SelectedAnswer);
                tmp.SelectedQuestion += new EventHandler(SelectedQuestion);
                tmp.Click += new EventHandler(BlockClick);
                tmp.Delink += new EventHandler(Delink);
                n.Question = Question;

                p = new Point(p.X + 150, p.Y);

                done = temp == ")" || temp == "" || temp == ",";
                if (!done) temp = temp.Substring(1);
                Set = true;
            }

            foreach(KeyValuePair<KeyValuePair<int,int>,int> a in NewLinks)
            {
                for(int i=0; i<InactiveNode1s.Count();i++)
                {
                    if(a.Key.Key == InactiveNode1s[i].ID)
                    {
                        for(int j=0;j<InactiveNode1s.Count();j++)
                        {
                            if(a.Value == InactiveNode1s[j].ID)
                            {
                                InactiveNode1s[i].InlineLink(a.Key.Value+1, InactiveNode1s[j]);
                                SelectedNode1Answer = new KeyValuePair<int,node>(a.Key.Value+1,InactiveNode1s[i]);
                                SelectedNode1Question = new KeyValuePair<int,node>(-1,InactiveNode1s[j]);
                                Reset();
                                break;
                            }
                        }
                        break;
                    }
                }
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int Id = RightClicked.ID;
            if (Id > 0)
            {
                RightClicked.Delete();
                for (int i = 0; i < InactiveNode1s.Count(); i++)
                {
                    if (InactiveNode1s[i].ID == Id) { InactiveNode1s.RemoveAt(i); break; }
                }
            }
            else MessageBox.Show("Begin node cannot be deleted !");
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RightClicked.Question = Prompt.ShowDialog("Question", RightClicked.Question);
            for (int i=0;i<RightClicked.Answers.Count();i++)
            {
                RightClicked.Answers[i] = (Prompt.ShowDialog("Answer" + i, RightClicked.Answers[i]));
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddedSize += new Size(0, 500);
            //AutoScrollMinSize = this.Size + AddedSize - new Size(250, 0);
            AutoScrollMargin = this.Size + AddedSize - new Size(250, 0);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AddedSize += new Size(500, 0);
            AutoScrollMinSize = this.Size + AddedSize - new Size(0,250);
            //AutoScrollMargin = this.Size + AddedSize - new Size(0, 250);
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (moveInitial != Point.Empty)
            {
                PointF normal = Normalize(new Point(e.X - moveInitial.X, e.Y - moveInitial.Y));

                HorizontalScroll.Value = Clamp(HorizontalScroll.Value + (int)(normal.X*8), HorizontalScroll.Minimum, HorizontalScroll.Maximum);
                VerticalScroll.Value = Clamp(VerticalScroll.Value + (int)(normal.Y*8), VerticalScroll.Minimum, VerticalScroll.Maximum);
                //this.PerformLayout();

                //MessageBox.Show(Clamp(HorizontalScroll.Value - e.X, 0, this.Size.Width).ToString());
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Middle) moveInitial = Point.Empty;
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                moveInitial = e.Location;
            }
        }
    }

    public class node
    {
        static int LastID;
        static List<int> FreeIDs = new List<int>();

        public int ID;
        public int nbAnswers;
        public List<node> NextNode1s;
        
        public List<String> Answers;
        public Block VisualBlock;
        public Form1 Form;
        public node Parent;

        private Point BlockLocation;
        private Size BlockSize;
        private string _Question;
        private bool done;

        public node(int nb)
        {
            nbAnswers = nb;
            if (FreeIDs.Count() > 0)
            {
                ID = FreeIDs.First();
                FreeIDs.RemoveAt(0);
            }
            else { ID = LastID; LastID++; }

            NextNode1s = new List<node>();
            Answers = new List<String>(nb);
            
            done = false;
        }

        public string Question
        {
            get { return _Question; }
            set
            {
                if (_Question == value) return;
                _Question = value;
                VisualBlock.Text = value;
            }
        }

        public Block SetBlock(Point p, Size s, int ID)
        {
            VisualBlock = new Block();
            VisualBlock.Location = p;
            VisualBlock.Size = s;
            VisualBlock.ID = ID;
            VisualBlock.Init();
            Form.Controls.Add(VisualBlock);
            return VisualBlock;

        }

        public void SetNode1(int index, string Answer, int NextId)
        {
            NextNode1s[index] = new node(0);
            NextNode1s[index].Form = Form;
        }

        public node Find(int nbID)
        {
            if (ID == nbID) return this;
            else
            {
                for(int i=0;i<nbAnswers;i++)
                {
                    return NextNode1s[i].Find(nbID);
                }
                return null;
            }
        }

        public void Link(int Answer, node Second) // Make sure that when the link is over, if(nbAnswers<5) nbAnswers++; and all that goes with it
        {

            if (NextNode1s.Count() <= Answer)
            {
                NextNode1s.Add(Second);
                Answers.Add(Prompt.ShowDialog("Answer", ""));
            }
            else
            {
                NextNode1s[Answer - 1].VisualBlock.Deselect(Answer-1, false);
                NextNode1s[Answer - 1] = Second; // Check if it already has a question, if yes add the old one to the Inactive nodes
                Answers[Answer - 1] = Prompt.ShowDialog("Answer", "");
            }
            Second.Parent = this;
            //Some graphics to link the 2 (If we can move the nodes, we should probably refresh the link as well) (Talking to myself as 'we', great)
        }

        public void InlineLink(int Answer, node Second)
        {
            if (NextNode1s.Count() < Answer)
            {
                NextNode1s.Add(Second);
            }
            else
            {
                NextNode1s[Answer - 1].VisualBlock.Deselect(Answer-1, false);
                NextNode1s[Answer - 1] = Second; // Check if it already has a question, if yes add the old one to the Inactive nodes
            }
            Second.Parent = this;
        }

        public string Run()
        {
            /*if (done) { done = false; return Question+" : "; } // Check back later in BP (Basically, this allows an answer to back back to previous Question)
            else
            {
                done = true;*/
                string tmp = "(Id=" + ID.ToString() + "," + "QuestionText=\""+ Question + "\",Answers=(";
            if (Answers.Count() == 0) { tmp = tmp.Remove(tmp.Length - 9); }
                for (int i = 0; i < Answers.Count(); i++)
                {
                    tmp += "(AnswerText=\"";
                    tmp += Answers[i] + "\",NextQuestionId=" + NextNode1s[i].ID.ToString() + "),";
                    //tmp += NextNode1s[i].Run();
                    //tmp += ",";
                }
                tmp = tmp.Remove(tmp.Length - 1) + "))";
                for (int i = 0; i < Answers.Count(); i++)
                {
                    tmp += "," + NextNode1s[i].Run();
                }
                //if(Answers.Count() != 0) tmp = tmp.Remove(tmp.Length - 1);
                return tmp;
            //}
        }

        public void Delete()
        {
            FreeIDs.Add(ID);
            FreeIDs.Sort();
            foreach(node n in NextNode1s)
            {
                //Should add them back to the InactiveNode1s
                
            }
            if(Parent != null)
            {
                for (int i = 0; i < Parent.NextNode1s.Count(); i++)
                {
                    if (Parent.NextNode1s[i].ID == ID) { Parent.NextNode1s.RemoveAt(i); Parent.Answers.RemoveAt(i); Parent.VisualBlock.Deselect(i, false); break; }
                }
            }
            VisualBlock.Delete();
        }
    }

    public static class Prompt
    {
        public static string ShowDialog(string caption, string text)
        {
            Form prompt = new Form()
            {
                Width = 500,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            Label textLabel = new Label() { Left = 50, Top = 20, Text = text };
            TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 400, Text = text };
            Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 70, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }
    }
}
