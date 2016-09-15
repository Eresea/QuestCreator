using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace DialogueCreator3
{
    public partial class Preview : Form
    {
        public string Code;
        public Question First;
        public Question Current;

        public TreeNode Tree;
        public TreeNode CurrentTree;

        public Preview()
        {
            InitializeComponent();
        }

        private void RefreshQuestion()
        {
            /*QuestionrTxtBox.Text = Current.QuestionText;

            for (int i = 0; i < 5; i++)
            {
                switch (i)
                {
                    case 0:
                        if (Current.QAnswers.Count() < i + 1) { Answer1Button.Text = ""; Answer1Button.Enabled = false; }
                        else { Answer1Button.Text = Current.QAnswers[i]; Answer1Button.Enabled = true; }
                        break;
                    case 1:
                        if (Current.QAnswers.Count() < i + 1) { Answer2Button.Text = ""; Answer2Button.Enabled = false; }
                        else { Answer2Button.Text = Current.QAnswers[i]; Answer2Button.Enabled = true; }
                        break;
                    case 2:
                        if (Current.QAnswers.Count() < i+1) { Answer3Button.Text = ""; Answer3Button.Enabled = false; }
                        else { Answer3Button.Text = Current.QAnswers[i]; Answer3Button.Enabled = true; }
                        break;
                    case 3:
                        if (Current.QAnswers.Count() < i+1) { Answer4Button.Text = ""; Answer4Button.Enabled = false; }
                        else { Answer4Button.Text = Current.QAnswers[i]; Answer4Button.Enabled = true; }
                        break;
                    case 4:
                        if (Current.QAnswers.Count() < i+1) { Answer5Button.Text = ""; Answer5Button.Enabled = false; }
                        else { Answer5Button.Text = Current.QAnswers[i]; Answer5Button.Enabled = true; }
                        break;
                }
            }*/

            QuestionrTxtBox.Text = CurrentTree.Question;

            for (int i = 0; i < CurrentTree.Answers.Count(); i++)
            {
                switch (i)
                {
                    case 0:
                        if (CurrentTree.Answers.Count() < i + 1) { Answer1Button.Text = ""; Answer1Button.Enabled = false; }
                        else { Answer1Button.Text = CurrentTree.Answers.ElementAt(i); Answer1Button.Enabled = true; }
                        break;
                    case 1:
                        if (CurrentTree.Answers.Count() < i + 1) { Answer2Button.Text = ""; Answer2Button.Enabled = false; }
                        else { Answer2Button.Text = CurrentTree.Answers.ElementAt(i); Answer2Button.Enabled = true; }
                        break;
                    case 2:
                        if (CurrentTree.Answers.Count() < i + 1) { Answer3Button.Text = ""; Answer3Button.Enabled = false; }
                        else { Answer3Button.Text = CurrentTree.Answers.ElementAt(i); Answer3Button.Enabled = true; }
                        break;
                    case 3:
                        if (CurrentTree.Answers.Count() < i + 1) { Answer4Button.Text = ""; Answer4Button.Enabled = false; }
                        else { Answer4Button.Text = CurrentTree.Answers.ElementAt(i); Answer4Button.Enabled = true; }
                        break;
                    case 4:
                        if (CurrentTree.Answers.Count() < i + 1) { Answer5Button.Text = ""; Answer5Button.Enabled = false; }
                        else { Answer5Button.Text = CurrentTree.Answers.ElementAt(i); Answer5Button.Enabled = true; }
                        break;
                }
            }

            SetEvents();


        }

        private void SetEvents()
        {
            /*foreach(string s in Current.Events)
            {
                MessageBox.Show(s);
            }*/
        }

        private void Preview_Load(object sender, EventArgs e)
        {
            /*QuestionrTxtBox.Text = Code;

            First = Current = new Question(Code);
            RefreshQuestion();*/

            QuestionrTxtBox.Text = Tree.Question;
            CurrentTree = Tree;

            RefreshQuestion();
        }

        private TreeNode Select(int i)
        {
            if (CurrentTree.Events.Count() > i)
            {
                foreach (string s in CurrentTree.Events.ElementAt(i))
                {
                    MessageBox.Show(s);
                }
            }

            if (CurrentTree.AnswersNodes.Count() > i) // Si l'élément existe
            {
                return CurrentTree.AnswersNodes.ElementAt(i);
            }
            return null;
        }

        private void Answer1Button_Click(object sender, EventArgs e)
        {
            //Current = Current.Select(0);
            CurrentTree = Select(0);
            if (CurrentTree != null) RefreshQuestion();
            else this.Close();
        }

        private void Answer2Button_Click(object sender, EventArgs e)
        {
            CurrentTree = Select(1);
            if (CurrentTree != null) RefreshQuestion();
            else this.Close();
        }

        private void Answer3Button_Click(object sender, EventArgs e)
        {
            CurrentTree = Select(2);
            if (CurrentTree != null) RefreshQuestion();
            else this.Close();
        }

        private void Answer4Button_Click(object sender, EventArgs e)
        {
            CurrentTree = Select(3);
            if (CurrentTree != null) RefreshQuestion();
            else this.Close();
        }

        private void Answer5Button_Click(object sender, EventArgs e)
        {
            CurrentTree = Select(4);
            if (CurrentTree != null) RefreshQuestion();
            else this.Close();
        }
    }

    public class Question
    {
        private string QText;
        public string QuestionText
        {
            get { return QText; }
            set
            {
                if (value == QText) return;
                QText = value;
            }
        }
        public List<string> QAnswers;
        private List<Question> NextQuestions;
        public List<string> Events;

        static private Regex regex = new Regex(@"
    \{                    # Match (
    (
        [^{}]+            # all chars except ()
        | (?<Level>\{)    # or if ( then Level += 1
        | (?<-Level>\})   # or if ) then Level -= 1
    )+                    # Repeat (to go from inside to outside)
    (?(Level)(?!))        # zero-width negative lookahead assertion
    \}                    # Match )", RegexOptions.IgnorePatternWhitespace);
        public Question(string s)
        {
            QAnswers = new List<string>();
            NextQuestions = new List<Question>();
            Events = new List<string>();

            foreach (Match c in regex.Matches(s))
            {
                string tmp = c.Value.Substring(1, c.Value.Length - 2);
                int index = tmp.IndexOf(":");
                QText = tmp.Substring(0, index); // Here bug when wrong check for error before Preview
                tmp = tmp.Substring(index+1);

                //processEvents(tmp); // Wrong idea here

                if (tmp.Contains("{") || tmp.Contains("["))
                {
                    processAnswers(tmp);
                }
            }

            
        }

        private void processEvents(string s)
        {
            var pattern = @"\[(.*?)\]";
            var matches = Regex.Matches(s, pattern);
            foreach(Match m in matches)
            {
                Events.Add(m.Value.Trim('[').Trim(']'));
            }
        }

        private void processAnswers(string s)
        {
            foreach (Match d in regex.Matches(s)) // PB when it's an event next
            {
                NextQuestions.Add(new Question(d.Value));
                //NextQuestions.Last().processEvents(s);
                if (s[0] == ',') s = s.Substring(1);
                if(s.IndexOf("[") < s.IndexOf("{")) QAnswers.Add(s.Substring(0, s.IndexOf("[")));
                else QAnswers.Add(s.Substring(0, s.IndexOf("{")));
                s = s.Substring(s.IndexOf(d.Value)+d.Value.Length);
            }
            if(s.Length > 0)
            {
                var pattern = @"\[(.*?)\]";
                var matches = Regex.Matches(s, pattern);
                foreach (Match m in matches)
                {
                    NextQuestions.Add(new Question(m.Value));
                    NextQuestions.Last().processEvents(s);
                    QAnswers.Add(s.Substring(0, s.IndexOf("[")));
                    s = s.Substring(s.IndexOf(m.Value) + m.Value.Length);
                }
            }
        }

        public Question Select(int index)
        {
            return NextQuestions[index];
        }
    }
}
