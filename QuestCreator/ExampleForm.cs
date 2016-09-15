using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Graph;
using System.Drawing.Drawing2D;
using Graph.Compatibility;
using Graph.Items;
using System.Text.RegularExpressions;

namespace DialogueCreator3
{
	public partial class ExampleForm : Form
	{
        string[] Events = { "Close", "AcceptQuest", "AdvanceQuest", "FinishQuest", "CustomEvent" };
        List<string> Elements = new List<string>() { "SpawnActor", "Teleport", "SpawnParticle", "CustomCode" };
        Node BeginNode;
        bool SaveOnline = false;
		public ExampleForm()
		{
			InitializeComponent();

			graphControl.CompatibilityStrategy = new TagTypeCompatibility();

			BeginNode = new Node("Begin");
            BeginNode.Location = new Point(0, 150);
            BeginNode.AddItem(new NodeTextBoxItem("BeginQuestion", false, false) { Tag = 31337 });
            BeginNode.AddItem(new NodeTextBoxItem("1", false, true) { Tag = 31337 });
            BeginNode.AddItem(new NodeTextBoxItem("2", false, true) { Tag = 31337 });
            BeginNode.AddItem(new NodeTextBoxItem("3", false, true) { Tag = 31337 });
            BeginNode.AddItem(new NodeTextBoxItem("4", false, true) { Tag = 31337 });
            BeginNode.AddItem(new NodeTextBoxItem("5", false, true) { Tag = 31337 });

            graphControl.AddNode(BeginNode);

			graphControl.ConnectionAdded	+= new EventHandler<AcceptNodeConnectionEventArgs>(OnConnectionAdded);
			graphControl.ConnectionAdding	+= new EventHandler<AcceptNodeConnectionEventArgs>(OnConnectionAdding);
			graphControl.ConnectionRemoving += new EventHandler<AcceptNodeConnectionEventArgs>(OnConnectionRemoved);
			graphControl.ShowElementMenu	+= new EventHandler<AcceptElementLocationEventArgs>(OnShowElementMenu);
		}

		void OnImgClicked(object sender, NodeItemEventArgs e)
		{
			MessageBox.Show("IMAGE");
		}

		void OnColClicked(object sender, NodeItemEventArgs e)
		{
			MessageBox.Show("Color");
		}

		void OnConnectionRemoved(object sender, AcceptNodeConnectionEventArgs e)
		{
			//e.Cancel = true;
		}

        private void SearchTextChanged_AddNode(object sender, EventArgs e)
        {
            foreach(ToolStripItem i in nodeMenu.Items)
            {
                i.Visible = i.Text.CaseInsensitiveContains(((ToolStripTextBox)sender).Text);
            }
        }

        private void Validate_AddNode(object sender, EventArgs e)
        {
            if(((KeyEventArgs)e).KeyCode == Keys.Enter)
            {
                foreach (ToolStripItem i in nodeMenu.Items)
                {
                    if (i.Visible == true && i != nodeMenu.Items[0]) { i.PerformClick(); return; }
                }
            }
        }


        void OnShowElementMenu(object sender, AcceptElementLocationEventArgs e)
		{
            nodeMenu.Items.Clear();
            if (e.Element == null)
			{
                nodeMenu.Items.Add(new ToolStripTextBox() { BackColor = Color.LightGray });
                ((ToolStripTextBox)nodeMenu.Items[0]).TextChanged += SearchTextChanged_AddNode;
                ((ToolStripTextBox)nodeMenu.Items[0]).KeyDown += Validate_AddNode;
                // Show a test menu for when you click on nothing
                foreach (string s in Elements)
                {
                    nodeMenu.Items.Add(s);
                }
                nodeMenu.MaximumSize = new Size(1000000, 200);
                nodeMenu.Show(e.Position);
                ((ToolStripTextBox)nodeMenu.Items[0]).Focus(); //Focus

                //tb.MaximumSize = new Size(nodeMenu.Size.Width/10, tb.Height);

                e.Cancel = false;
			} else
			if (e.Element is Node)
			{
				// Show a test menu for a node
				nodeMenu.Items.Add(((Node)e.Element).Title);
				nodeMenu.Show(e.Position);
				e.Cancel = false;
			} else
			if (e.Element is NodeItem)
			{
                // Show a test menu for a nodeItem
                nodeMenu.Items.Add(e.Element.GetType().Name);
				nodeMenu.Show(e.Position);
				e.Cancel = false;
			} else
			{
				// if you don't want to show a menu for this item (but perhaps show a menu for something more higher up) 
				// then you can cancel the event
				e.Cancel = true;
			}
		}

		void OnConnectionAdding(object sender, AcceptNodeConnectionEventArgs e)
		{
			//e.Cancel = true;
		}

		static int counter = 1;
		void OnConnectionAdded(object sender, AcceptNodeConnectionEventArgs e)
		{
			//e.Cancel = true;
			e.Connection.Name = "Connection " + counter ++;
			e.Connection.DoubleClick += new EventHandler<NodeConnectionEventArgs>(OnConnectionDoubleClick);
		}

		void OnConnectionDoubleClick(object sender, NodeConnectionEventArgs e)
		{
			e.Connection.Name = "Connection " + counter++;
		}

		private void SomeNode_MouseDown(object sender, MouseEventArgs e)
		{
			var node = new Node("");
            node.AddItem(new NodeTextBoxItem("Question", true, false) { Tag = 31337 });
            node.AddItem(new NodeTextBoxItem("1", false, true) { Tag = 31337 });
            node.AddItem(new NodeTextBoxItem("2", false, true) { Tag = 31337 });
            node.AddItem(new NodeTextBoxItem("3", false, true) { Tag = 31337 });
            node.AddItem(new NodeTextBoxItem("4", false, true) { Tag = 31337 });
            node.AddItem(new NodeTextBoxItem("5", false, true) { Tag = 31337 });
			this.DoDragDrop(node, DragDropEffects.Copy);
		}

		private void TextureNode_MouseDown(object sender, MouseEventArgs e)
		{
            var node = new Node("Event");
            node.AddItem(new NodeDropDownItem(Events, 0, true, true) { Tag = 31337 });
            this.DoDragDrop(node, DragDropEffects.Copy);
        }

		private void ColorNode_MouseDown(object sender, MouseEventArgs e)
		{
            var node = new Node("If");
            node.AddItem(new NodeTextBoxItem("IsValid?", true, false) { Tag = 31337 });
            node.AddItem(new NodeTextBoxItem("True", false, true) { Tag = 31337 });
            node.AddItem(new NodeTextBoxItem("False", false, true) { Tag = 31337 });
            this.DoDragDrop(node, DragDropEffects.Copy);
        }

		private void OnShowLabelsChanged(object sender, EventArgs e)
		{
			graphControl.ShowLabels = showLabelsCheckBox.Checked;
		}

        private void button1_Click(object sender, EventArgs e)
        {
            TreeNode Tree = Run3(BeginNode, null, -1);

            Clipboard.SetText("(" + Tree.process() + ")"); // Pb avec les events (ordre et profondeur de retour)
        }

        struct nodeStructure // Structure to serialize the data from nodes to text
        {
            public int ID;
            public string Data;
            public List<string> Answers;
            public List<int> AnswersIDs;
            public List<int> itemIDs;

            public nodeStructure(int a, string b, List<string> c, List<int> d, List<int> e)
            {
                ID = a;
                Data = b;
                Answers = c;
                AnswersIDs = d;
                itemIDs = e;
            }
        }

        private Node prevNextNode(NodeConnection nc, bool next, ref int id)
        {
            if(next)
            {
                id = 0;
                
                if(nc.To.Node.Title == "") return nc.To.Node;
                else
                {
                    if (nc.To.Node.Title == "Condition") return prevNextNode(nc.To.Node.Connections.ElementAt(1), next,ref id);
                    return prevNextNode(nc.To.Node.Connections.ElementAt(0), next,ref id);
                }
            }
            else
            {
                id = nc.From.Node.Connections.ToList().IndexOf(nc);
                if (nc.To.Node.Title == "") return nc.From.Node;
                else
                {
                    return prevNextNode(nc.From.Node.Connections.ElementAt(0), next,ref id);
                }
            }
        }

        private List<Node> PreviousQuestion(NodeConnector nc, ref List<int> i)
        {
            List<Node> tmp = new List<Node>();

            //i = nc.Node.Connections.ToList().IndexOf(nc.Connectors.ElementAt(0)); //Not sure at all
            if (nc.Node.Title != "If" && nc.Node.Title != "Event") // Si Le node est une question
            {
                    tmp.Add(nc.Node);//Ajoute le node à la liste
                    foreach (NodeItem ni in nc.Node.Items) // Pour chaque item
                    {
                        if (ni.Output == nc) // Si l'item est connecté à notre Connector
                        {
                            i.Add(nc.Node.Items.ToList().IndexOf(ni)); //Ajoute l'index de l'item aux indexs
                            break;
                        }
                    }

                foreach(Node n in tmp)
                {
                    MessageBox.Show(((NodeTextBoxItem)n.Items.ElementAt(0)).Text + " is linked with " + i.ElementAt(tmp.IndexOf(n)).ToString());
                }
                return tmp;
            }
            //Sinon, pour chaque connectors
            for(int j=0;j<nc.Node.Connections.Count();j++) // Doesn't actually take the previous Node's from connection
            {
                if (nc.Node.Connections.ElementAt(j).From != nc)
                {
                    List<Node> tmp2 = PreviousQuestion(nc.Node.Connections.ElementAt(j).From, ref i);
                    foreach(Node n2 in tmp2)
                    {
                        if(!(tmp.Contains(n2)))
                        {
                            tmp.Add(n2);
                        }
                    }
                }
            }
            return tmp;
        }

        private string addEvent(string e,string S)
        {
            if(S.Substring(S.IndexOf("Events=")+7,1) == ",") return S.Insert(S.IndexOf("Events=") + 7, "(\"" + e + "\")");
            return S.Insert(S.IndexOf("Events=(") + 8, "\"" + e + "\",");
        }

        private string addCondition(string c, string S)
        {
            if (S.Substring(S.IndexOf("Conditions=") + 11, 1) == ",") return S.Insert(S.IndexOf("Conditions=") + 11, "(\"" + c + "\")");
            return S.Insert(S.IndexOf("Conditions=(") + 12,"\"" + c + "\",");
        }

        private string Run2(Node n)
        {
            Regex regex = new Regex(@"
    \{                    # Match (
    (
        [^{}]+            # all chars except ()
        | (?<Level>\{)    # or if ( then Level += 1
        | (?<-Level>\})   # or if ) then Level -= 1
    )+                    # Repeat (to go from inside to outside)
    (?(Level)(?!))        # zero-width negative lookahead assertion
    \}                    # Match )", RegexOptions.IgnorePatternWhitespace);
            List<nodeStructure> L = new List<nodeStructure>();
            List<KeyValuePair<int, KeyValuePair<int, string>>> Events = new List<KeyValuePair<int, KeyValuePair<int, string>>>();
            List<KeyValuePair<int, KeyValuePair<int, string>>> Conditions = new List<KeyValuePair<int, KeyValuePair<int, string>>>();

            string tmp = Run(n, ref L, ref Events, ref Conditions);
            string tmp2 = " ";
            List<string> arrayOfQuestions = new List<string>();
            List<int> indexTable = new List<int>();


            foreach(nodeStructure l in L.Reverse<nodeStructure>()) // First pass for Questions
            {
                switch(l.Data)
                {
                    case "|If":
                        break;
                    case "|Event":
                        break;
                    default:
                        arrayOfQuestions.Add("Id=" + l.ID.ToString() + ",QuestionText=\"" + l.Data + "\",Answers=((");
                        indexTable.Add(l.ID);
                        for(int i=0;i<l.Answers.Count();i++)
                        {
                            if (arrayOfQuestions[arrayOfQuestions.Count - 1].Last() == ')') arrayOfQuestions[arrayOfQuestions.Count - 1] += ",(";
                            arrayOfQuestions[arrayOfQuestions.Count - 1] += "AnswerText=\"" + l.Answers.ElementAt(i) + "\",NextQuestionId=" + l.itemIDs.ElementAt(i).ToString();
                            if(l.itemIDs.ElementAt(i) != l.AnswersIDs.ElementAt(i)) arrayOfQuestions[arrayOfQuestions.Count()-1] += "|" + l.AnswersIDs.ElementAt(i).ToString() + ",Events=,Conditions=)";
                            else arrayOfQuestions[arrayOfQuestions.Count()-1] += ",Events=,Conditions=)";
                        }
                        arrayOfQuestions[arrayOfQuestions.Count - 1] += ")";
                        L.Remove(l);
                        break;
                }                
            }
            
           foreach(KeyValuePair<int,KeyValuePair<int,string>> a in Events) //Pour touts events
            {
                string tmpS = arrayOfQuestions[indexTable.IndexOf(a.Key)]; // tmpS = Target Question Data 'Gets the NODE'

                string tmpS2 = tmpS.Substring(0, tmpS.IndexOf("NextQuestionId=" + a.Value.Key));
                tmpS = addEvent(a.Value.Value, tmpS.Substring(tmpS.IndexOf("NextQuestionId=" + a.Value.Key)));
                tmpS2 += tmpS.Substring(0, tmpS.IndexOf("=" + a.Value.Key)+1) + tmpS.Substring(tmpS.IndexOf("|")+1);
                arrayOfQuestions[indexTable.IndexOf(a.Key)] = tmpS2;
            }

            tmp2 = "(";
            foreach(string s in arrayOfQuestions)
            {
                if (tmp2 != "(") tmp2 += ",";
                tmp2 += "(" + s + ")";
            }
            tmp2 += ")";

            return tmp2;
        }

        private int handleEventsConditions(ref List<KeyValuePair<int,KeyValuePair<int,string>>> Events, ref List<KeyValuePair<int,KeyValuePair<int, string>>> Conditions, Node N,int a,int b)
        {
            KeyValuePair<int, string> First;
            switch(N.Title)
            {
                case "If":
                    First = new KeyValuePair<int, string>(b, ((NodeTextBoxItem)N.Items.ElementAt(0)).Text);
                    Conditions.Add(new KeyValuePair<int, KeyValuePair<int, string>>(a, First)); // Added the Condition
                    break;
                case "Event":
                    First = new KeyValuePair<int, string>(b, ((NodeDropDownItem)N.Items.ElementAt(0)).Items.ElementAt(((NodeDropDownItem)N.Items.ElementAt(0)).SelectedIndex));

                    if(!(Events.Contains(new KeyValuePair<int, KeyValuePair<int, string>>(a,First)))) Events.Add(new KeyValuePair<int, KeyValuePair<int, string>>(a, First)); // Added the Event
                    break;
                default:
                    return graphControl.Nodes.ToList().IndexOf(N); // End when you find a question
            }

            if (N.Connections.Last().To.Node != N) // S'il existe une 'suite' (TEST INCORRECT)
            {
                handleEventsConditions(ref Events, ref Conditions, N.Connections.Last().To.Node, a, b); //Continue la génération
            }

            return -1;
        }

        // Remake Run
        public List<KeyValuePair<int, KeyValuePair<int, string>>> nodeEvents;
        public List<KeyValuePair<int, KeyValuePair<int, string>>> nodeConditions;

        private TreeNode Run3(Node n,TreeNode previousQuestion,int previousAnswerID)
        {
            TreeNode N = new TreeNode();

            switch(n.Title)
            {
                case "If":

                    break;
                case "Event":
                    string e = ((NodeDropDownItem)n.Items.ElementAt(0)).Items.ElementAt(((NodeDropDownItem)n.Items.ElementAt(0)).SelectedIndex);
                    if(n.Items.Count() > 1)
                    {
                        NodeTextBoxItem nti = ((NodeTextBoxItem)n.Items.ElementAt(1));
                        if (nti != null) e += "(" + nti.Text + ")";
                        else
                        {
                            NodeDropDownItem nddi = ((NodeDropDownItem)n.Items.ElementAt(1));
                            if (nddi != null) e += "("+ nddi.Items.ElementAt(nddi.SelectedIndex) + ")";
                        }
                    }
                    previousQuestion.AddEvent(e,previousAnswerID);
                    if (n.Connections.Last().To.Node != n) // If it continues
                    {
                        return (Run3(n.Connections.Last().To.Node, previousQuestion, previousAnswerID));
                    }
                    else return null;
                    break;
                default:
                    N.Question = ((NodeTextBoxItem)n.Items.ElementAt(0)).Text;
                    for(int i=0;i<n.Connections.Count();i++)//Foreach right connections
                    {
                        if(n.Connections.ElementAt(i).To.Node != n) // S'il y a une connection
                        {
                            N.ID = graphControl.Nodes.ToList().IndexOf(n);
                            N.Answers.Add(((NodeTextBoxItem)n.Connections.ElementAt(i).From.Item).Text);    //Adds the answer text
                            N.AnswersNodes.Add(Run3(n.Connections.ElementAt(i).To.Node,N,n.Items.ToList().IndexOf(n.Connections.ElementAt(i).From.Item)-1));                   //Adds the answer TreeNode
                        }
                    }
                    break;
            }

            return N;
        }

        private string Run(Node n, ref List<nodeStructure> L,ref List<KeyValuePair<int, KeyValuePair<int, string>>> Events, ref List<KeyValuePair<int, KeyValuePair<int, string>>> Conditions)
        {
            string tmp = "";
            int close = 0;
            bool noConnections = true;
            nodeStructure nS = new nodeStructure();
            nS.Answers = new List<string>();
            nS.AnswersIDs = new List<int>();
            List<int> index = new List<int>();
            List<Node> previousNodes = new List<Node>();

            bool alreadyRegistered = false;
            //CHECK if the node isn't already registered
            foreach (nodeStructure l in L)
            {
                if (graphControl.Nodes.ToList().IndexOf(n) == l.ID)
                {
                    alreadyRegistered = true;
                    break;
                }
            }

            switch (n.Title)
            {
                case "If":
                    break;
                case "Event":
                    break;
                default:
                    nS.ID = graphControl.Nodes.ToList().IndexOf(n);
                    nS.Data = ((NodeTextBoxItem)n.Items.ElementAt(0).Node.Items.ElementAt(0)).Text;
                    nS.Answers = new List<string>();
                    nS.AnswersIDs = new List<int>();
                    nS.itemIDs = new List<int>();
                    for(int i=0;i<n.Connections.Count();i++) // Pour chaque connection
                    {
                        var con = n.Connections.ElementAt(i);
                        if(con.To.Node != n) // Si une connection existe vraiment
                        {
                            nS.Answers.Add(((NodeTextBoxItem)con.From.Item).Text);
                            nS.AnswersIDs.Add(handleEventsConditions(ref Events, ref Conditions, con.To.Node, graphControl.Nodes.ToList().IndexOf(n), n.Items.ToList().IndexOf(con.From.Item)));
                            nS.itemIDs.Add(n.Items.ToList().IndexOf(con.From.Item));
                        }
                    }
                    close = 1;
                    break;
            }
            if(!alreadyRegistered) L.Add(nS);

            foreach (KeyValuePair<int,KeyValuePair<int,string>> a in Events)
            {
                MessageBox.Show(a.Key.ToString() + "/" + a.Value.Key.ToString() + " : " + a.Value.Value);
            }

            foreach (KeyValuePair<int, KeyValuePair<int, string>> a in Conditions)
            {
                MessageBox.Show(a.Key.ToString() + "/" + a.Value.Key.ToString() + " : " + a.Value.Value);
            }

            for (int i = 0; i < n.Connections.Count(); i++)
            {
                var con = n.Connections.ElementAt(i);
                if (con.To.Node != n)
                {
                    noConnections = false;
                    if (close == 1) tmp += ((NodeTextBoxItem)con.From.Item).Text;
                    tmp += Run(con.To.Node, ref L,ref Events,ref Conditions);
                    tmp += ","; // N'est pas le dernier
                }
            }

            /*for(int i=0;i<n.Connections.Count();i++)
            {
                var con = n.Connections.ElementAt(i);
                if (con.To.Node != n)
                {
                    noConnections = false;
                    if (close == 1) tmp += ((NodeTextBoxItem)con.From.Item).Text;
                    tmp += Run(con.To.Node);
                    tmp += ","; // N'est pas le dernier
                }
            }*/

            //TEMPORARY
            /*if (tmp.Substring(tmp.Length - 1) == ",") tmp = tmp.Substring(0, tmp.Length - 1);
            if (close == 1)
            {
                if (noConnections) tmp = tmp.Substring(0, tmp.Length - 1); // If it didn't have Elements inside, remove the last char
                tmp += "}";
            }*/
            return tmp;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Preview Prev = new Preview();
            Prev.Tree = Run3(BeginNode, null, -1);
            //Prev.Code = Run2(BeginNode);
            Prev.Show();
        }

        private void OpenFile(string url)
        {
            graphControl.ClearBoard();
            string line;
            List<KeyValuePair<NodeConnector, KeyValuePair<int, int>>> Connections = new List<KeyValuePair<NodeConnector, KeyValuePair<int, int>>>();
            //CLEAR CURRENT BOARD
            if (System.IO.File.Exists(url))
            {
                System.IO.StreamReader file = new System.IO.StreamReader(url);
                while ((line = file.ReadLine()) != null)
                {
                    processLine(line, ref Connections);
                }
            }
            else
            {
                foreach(string s in url.Split('\n'))
                {
                    if(s != "") processLine(s, ref Connections);
                }
            }

            foreach(KeyValuePair<NodeConnector,KeyValuePair<int,int>> N in Connections)
            {
                graphControl.Connect(N.Key, graphControl.Nodes.ElementAt(N.Value.Key).Items.ElementAt(N.Value.Value).Input);
            }
        }

        private void processLine(string line, ref List<KeyValuePair<NodeConnector,KeyValuePair<int,int>>> Connections)
        {
            string nodeType = line.Substring(0, line.IndexOf(':'));

            PointF nodeLocation = new Point();

            line = line.Substring(line.IndexOf("{X=") + 3);
            nodeLocation.X = float.Parse(line.Substring(0, line.IndexOf(", ")));
            line = line.Substring(line.IndexOf("Y=") + 2);
            nodeLocation.Y = float.Parse(line.Substring(0, line.IndexOf('}')));

            Node NewNode = new Node(nodeType);
            if(nodeType == "Begin") BeginNode = NewNode;
            NewNode.Location = nodeLocation;

            line = line.Substring(line.IndexOf('|'));
            Regex regex = new Regex(@"\((?>[^()]+|\((?<P>)|(?<C-P>)\))*(?(P)(?!))\)", RegexOptions.IgnorePatternWhitespace);

            List<string> nodesInfo = regex.Matches(line).Cast<Match>().Select(p => p.Groups[0].Value).ToList();

            foreach (string s in nodesInfo) // Foreach items in the node
            {
                switch (s.Substring(1, s.IndexOf(':') - 1)) // Add the item
                {
                    case "Label":
                        NewNode.AddItem(new NodeTextBoxItem(s.Substring(s.IndexOf(':') + 1, (s.IndexOf('|') - s.IndexOf(':') - 1)), (s.Substring(s.IndexOf('|') + 1, 1) == "T"), (s.Substring(s.LastIndexOf(',') + 1, 1) == "T")) { Tag = 31337 });
                        break;
                    case "DropDown":
                        NewNode.AddItem(new NodeDropDownItem(Events, Events.ToList().IndexOf(s.Substring(s.IndexOf(':') + 1, (s.IndexOf('|') - s.IndexOf(':') - 1))), (s.Substring(s.IndexOf('|') + 1, 1) == "T"), (s.Substring(s.LastIndexOf(',') + 1, 1) == "T")) { Tag = 31337 });
                        break;
                }
                NodeConnector Nc;
                List<string> test = s.Substring(s.IndexOf('|')).Split(',').ToList();
                if (test[0].Contains(":"))
                {
                    Nc = NewNode.Items.Last().Input;
                    //Connections.Add(new KeyValuePair<NodeConnector, KeyValuePair<int, int>>(Nc, new KeyValuePair<int, int>(Int32.Parse(test[1].Substring(test[1].IndexOf(':')+1,test[1].IndexOf('/')- test[1].IndexOf(':')-1)),Int32.Parse(test[1].Substring(test[1].IndexOf('/') + 1)))));
                }
                if (test[1].Contains(":"))
                {
                    Nc = NewNode.Items.Last().Output;
                    Connections.Add(new KeyValuePair<NodeConnector, KeyValuePair<int, int>>(Nc, new KeyValuePair<int, int>(Int32.Parse(test[1].Substring(test[1].IndexOf(':') + 1, test[1].IndexOf('/') - test[1].IndexOf(':') - 1)), Int32.Parse(test[1].Substring(test[1].IndexOf('/') + 1).Trim(')')))));
                }
            }

            graphControl.AddNode(NewNode);
        }

        private List<string> SaveProcess()
        {
            List<string> Lines = new List<string>();
            string tmp, tmp1;
            foreach (Node n in graphControl.Nodes)
            {
                tmp = tmp1 = "";
                foreach (NodeItem ni in n.Items)
                {
                    if (tmp != "") tmp += ","; // Si ce n'est pas le premier item, virgule
                    NodeTextBoxItem Label = ni as NodeTextBoxItem; // Type 0
                    NodeDropDownItem DropDown = ni as NodeDropDownItem;
                    if (Label != null) // If the item is a label
                    {
                        tmp += "(Label:" + Label.Text + "|" + ni.Input.Enabled;
                        if (ni.Input.HasConnection)
                        {
                            tmp += ":" + graphControl.Nodes.ToList().IndexOf(ni.Input.Connectors.ElementAt(0).From.Node).ToString() + "/" + ni.Input.Connectors.ElementAt(0).From.Node.Items.ToList().IndexOf(ni.Input.Connectors.ElementAt(0).From.Item).ToString();
                        }
                        tmp += "," + ni.Output.Enabled;
                        if (ni.Output.HasConnection)
                        {
                            tmp += ":" + graphControl.Nodes.ToList().IndexOf(ni.Output.Connectors.ElementAt(0).To.Node).ToString() + "/" + ni.Output.Connectors.ElementAt(0).To.Node.Items.ToList().IndexOf(ni.Output.Connectors.ElementAt(0).To.Item).ToString();
                        }
                        tmp += ")";
                    }
                    if (DropDown != null)
                    {
                        tmp += "(DropDown:" + DropDown.Items[DropDown.SelectedIndex] + "|" + ni.Input.Enabled;
                        if (ni.Input.HasConnection)
                        {
                            tmp += ":" + graphControl.Nodes.ToList().IndexOf(ni.Input.Connectors.ElementAt(0).From.Node).ToString() + "/" + ni.Input.Connectors.ElementAt(0).From.Node.Items.ToList().IndexOf(ni.Input.Connectors.ElementAt(0).From.Item).ToString();
                        }
                        tmp += "," + ni.Output.Enabled;
                        if (ni.Output.HasConnection)
                        {
                            tmp += ":" + graphControl.Nodes.ToList().IndexOf(ni.Output.Connectors.ElementAt(0).To.Node).ToString() + "/" + ni.Output.Connectors.ElementAt(0).To.Node.Items.ToList().IndexOf(ni.Output.Connectors.ElementAt(0).To.Item).ToString();
                        }
                        tmp += ")";
                    }


                    tmp1 = n.Title + ":" + n.Location;
                    foreach (NodeConnection con in n.Connections)
                    {
                        tmp1 += ",";
                        if (con.To != null) tmp1 += con.To.Node.ToString();
                        else tmp1 += con.To.Node.ToString();
                    }
                    tmp1 += "|";
                }
                Lines.Add(tmp1 + tmp);
            }
            return Lines;
        }

        private void toDiskToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != "")
            {
                System.IO.StreamWriter file = new System.IO.StreamWriter(saveFileDialog1.FileName);
                List<string> Lines = SaveProcess();
                foreach(string s in Lines)
                {
                    file.WriteLine(s);
                }
                file.Close();
                FadeTimer.Start();
                SaveOnline = false;
            }
        }

        private void fromDiskToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
            if (System.IO.File.Exists(saveFileDialog1.FileName))
            {
                OpenFile(saveFileDialog1.FileName);
                textBox1.Text = saveFileDialog1.FileName.Substring(saveFileDialog1.FileName.LastIndexOf('\\')+1);
                textBox1.Text = textBox1.Text.Remove(textBox1.Text.IndexOf('.'));
                SaveOnline = false;
            }
        }

        private void onlineToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            List<string> Lines = SaveProcess();
            string s = "";
            foreach(string line in Lines)
            {
                s += line + Environment.NewLine;
            }
            string DialogName = textBox1.Text;
            while (DialogName == "")
            {
                textBox1.Text = Prompt.ShowDialog("Dialog Name", "Enter a Dialog Name : ");
                DialogName = textBox1.Text;
            }
            if(OnlineFunction(DialogName, "Insert", s))
            {
                SaveOnline = true;
                FadeTimer.Start();
            }
        }

        private bool OnlineFunction(string DialogName, string Function, string Data)
        {
            string url = "http://saoproject.livehost.fr/SAOProject/Dialogs.php?Function=" + Function + "&Data=" + DialogName + "|" + Data;
            if (Data == "") url = url.Remove(url.Length - 1);

            using (var Client = new System.Net.WebClient())
            {
                var response = Client.DownloadString(url);
                if (response == "Failure") { MessageBox.Show("Failure !"); return false; } // Replace these messagebox by something like a notif later
                if (response == "Success") { if(Function == "Update") FadeTimer.Start(); return true; }
                if (response == "Name already taken !")
                {
                    DialogResult dr = MessageBox.Show("This name is already taken !" + Environment.NewLine + "Do you want to overwrite it ?", "Name taken", MessageBoxButtons.YesNo);
                    if (dr == DialogResult.Yes)
                    {
                        return OnlineFunction(DialogName, "Update", Data); // Recall this function but with Update function
                    }
                    return false;
                }
                if(response.Length>3)
                {
                    if (response.Substring(0, 4) == "Get:")
                    {
                        response = response.Substring(4);
                        OpenFile(response);
                    }
                }
                if(response.Length>4)
                {
                    if (response.Substring(0, 5) == "LIST:")
                    {
                        response = response.Substring(5);
                    }
                }
                return true;
            }
        }

        private void onlineToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            OpenOnline f1 = new OpenOnline(this);
            f1.Show();
        }

        public void openFromOnline(string name)
        {
            if (name.Contains("|")) // PB with IDs (Might want to create a custom Run3()
            {
                List<string> names = name.Split('|').ToList();
                string tmpAll = "(";
                TreeNode Tree;
                int currentHighestID = 0; // Used to add to each ID sections (and NextQuestionID...) while adding new dialogues
                foreach (string s in names)
                {
                    OnlineFunction(s.Trim('|'), "Get", "");
                    if (tmpAll != "(") tmpAll += ",";
                    Tree = Run3(BeginNode, null, -1);

                    tmpAll += Tree.process();
                }
                tmpAll = tmpAll + ")";

                Clipboard.SetText(tmpAll);
            }
            else if (name != "") OnlineFunction(name, "Get", "");
            textBox1.Text = name;
            SaveOnline = true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            SaveOnline = false;
            saveFileDialog1.FileName = textBox1.Text;
        }

        private void ExampleForm_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Control && e.KeyCode == Keys.S)
            {
                if(SaveOnline)
                {
                    onlineToolStripMenuItem2.PerformClick();
                }
                else
                {
                    toDiskToolStripMenuItem1.PerformClick();
                }
            }
        }

        private void nodeMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch(e.ClickedItem.Text)
            {
                case "SpawnActor":
                    var node = new Node("");
                    node.AddItem(new NodeTextBoxItem("SpawnActor", true, false) { Tag = 31337 });
                    node.AddItem(new NodeTextBoxItem("1", false, true) { Tag = 31337 });
                    node.AddItem(new NodeTextBoxItem("2", false, true) { Tag = 31337 });
                    node.AddItem(new NodeTextBoxItem("3", false, true) { Tag = 31337 });
                    node.AddItem(new NodeTextBoxItem("4", false, true) { Tag = 31337 });
                    node.AddItem(new NodeTextBoxItem("5", false, true) { Tag = 31337 });
                    graphControl.AddNode(node);
                    node.Location = graphControl.PointToClient(Cursor.Position);

                    var points = new PointF[] { node.Location };
                    graphControl.inverse_transformation.TransformPoints(points);
                    node.Location = points[0];

                    break;
                case "Teleport":
                    var TreeNode = new Node("Teleport");
                    TreeNode.AddItem(new NodeDropDownItem(Events, 0, true, true) { Tag = 31337 });
                    graphControl.AddNode(TreeNode);
                    TreeNode.Location = graphControl.PointToClient(Cursor.Position);

                    var points1 = new PointF[] { TreeNode.Location };
                    graphControl.inverse_transformation.TransformPoints(points1);
                    TreeNode.Location = points1[0];
                    break;
                case "SpawnParticle":
                    var node2 = new Node("SpawnParticle");
                    node2.AddItem(new NodeTextBoxItem("IsValid?", true, false) { Tag = 31337 });
                    node2.AddItem(new NodeTextBoxItem("True", false, true) { Tag = 31337 });
                    node2.AddItem(new NodeTextBoxItem("False", false, true) { Tag = 31337 });
                    graphControl.AddNode(node2);
                    node2.Location = graphControl.PointToClient(Cursor.Position);

                    var points2 = new PointF[] { node2.Location };
                    graphControl.inverse_transformation.TransformPoints(points2);
                    node2.Location = points2[0];
                    break;
                case "CustomCode": // Make a custom NodeItem for richTextBox (Or even custom IDE)
                    var node3 = new Node("CustomCode");
                    node3.AddItem(new NodeRichTextBoxItem("", true, true) { Tag = 31337 });
                    graphControl.AddNode(node3);
                    node3.Location = graphControl.PointToClient(Cursor.Position);
                    break;
            }
        }

        string RemoveBetween(string s, int begin, int end)
        {
            Regex regex = new Regex(string.Format("\\{0}.*?\\{1}", s.ElementAt(begin), s.ElementAt(end)));
            return regex.Replace(s, string.Empty);
        }

        int opacity = -1;

        private void FadeTimer_Tick(object sender, EventArgs e)
        {
            if(opacity < 50)
            {
                
                opacity++;
                if (opacity == 0)
                {
                    FadeTimer.Stop();
                    panel1.Visible = false;
                }
                else panel1.Visible = true;
            }
            else
            {
                opacity = -1;
            }
        }

        private void ExampleForm_Load(object sender, EventArgs e)
        {
            FadeTimer.Start();
            graphControl.NodeAdded += new EventHandler<AcceptNodeEventArgs>(AddedNode);
        }

        public void AddedNode(object sender, EventArgs e)
        {
            Graph.AcceptNodeEventArgs E = (AcceptNodeEventArgs)e;
            Node n = E.Node;
            if(n.Title == "Event")
            {
                ((NodeDropDownItem)n.Items.ElementAt(0)).SelectionChanged += new EventHandler<AcceptNodeSelectionChangedEventArgs>(DropDownItemChanged);
            }
        }

        public void DropDownItemChanged(object sender, EventArgs e)
        {
            AcceptNodeSelectionChangedEventArgs E = (AcceptNodeSelectionChangedEventArgs)e;
            NodeDropDownItem Item = (NodeDropDownItem)sender;
            Node n = Item.Node;

            switch(Events.ElementAt(E.Index))
            {
                case "AcceptQuest":
                    if(n.Items.Count() == 1) n.AddItem(new NodeTextBoxItem("Quest1"));
                    break;
                case "AdvanceQuest":
                    if (n.Items.Count() == 1) n.AddItem(new NodeTextBoxItem("Quest1"));
                    break;
                case "FinishQuest":
                    if (n.Items.Count() == 1) n.AddItem(new NodeTextBoxItem("Quest1"));
                    break;
                case "CustomEvent":
                    if (n.Items.Count() == 1) n.AddItem(new NodeTextBoxItem("Detail"));
                    else ((NodeTextBoxItem)n.Items.ElementAt(1)).Text = "Detail";
                    break;
                default:
                    if(n.Items.Count() > 1)
                    {
                        n.RemoveItem(n.Items.ElementAt(1));
                    }
                    break;
            }
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode Tree = Run3(BeginNode, null, -1);

            Clipboard.SetText("(" + Tree.process() + ")"); // Pb avec les events (ordre et profondeur de retour)
        }
    }



    public class TreeNode
    {
        public int ID;
        public string Question;
        public List<string> Answers;
        public List<TreeNode> AnswersNodes;
        public List<List<string>> Events;
        public List<List<string>> Conditions;

        public TreeNode()
        {
            Answers = new List<string>(); AnswersNodes = new List<TreeNode>(); Events = new List<List<string>>(); Conditions = new List<List<string>>();
        }

        public TreeNode(int i, string Q, List<string> A, List<TreeNode> N, List<List<string>> E, List<List<string>> C)
        {
            ID = i; Question = Q; Answers = A; AnswersNodes = N; Events = E; Conditions = C;
        }

        public void AddEvent(string e, int index)
        {
            while (Events.Count() - 1 < index)
            {
                Events.Add(new List<string>());
            }
            Events.Last().Add(e);
        }

        public string process()
        {
            string ret = "(Id=" + ID.ToString() + ",QuestionText=\"" + Question + "\",Answers=(";

            for (int i = 0; i < Answers.Count(); i++)
            {
                ret += "(AnswerText=\"" + Answers.ElementAt(i) + "\",NextQuestionId=";
                if (AnswersNodes.ElementAt(i) != null) ret += AnswersNodes.ElementAt(i).ID.ToString();
                ret += ",Events=";
                if (Events.Count > i)
                {
                    ret += "(";
                    foreach (string s in Events.ElementAt(i))
                    {
                        ret += "\"" + s + "\"" + ",";
                    }
                    if (ret.ElementAt(ret.Length - 1) == '(') ret = ret.Remove(ret.Length - 1);
                    else ret = ret.Remove(ret.Length - 1) + ")";
                }
                ret += ",Conditions=";
                if (Conditions.Count > i)
                {
                    ret += "(";
                    foreach (string s in Conditions.ElementAt(i))
                    {
                        ret += "\"" + s + "\"" + ",";
                    }
                    if (ret.ElementAt(ret.Length - 1) == '(') ret = ret.Remove(ret.Length - 1);
                    else ret = ret.Remove(ret.Length - 1) + ")";
                }
                ret += "),"; // Fin de la réponse
            }
            ret = ret.Remove(ret.Length - 1) + ")),";

            foreach (TreeNode n1 in AnswersNodes)
            {
                if (n1 != null) ret += n1.process();
            }
            if (ret.ElementAt(ret.Length - 1) == ',') ret = ret.Remove(ret.Length - 1);
            return ret;
        }

        public void Show()
        {
            MessageBox.Show("ID:" + ID.ToString() + ", Question : " + Question);
            for (int i = 0; i < Answers.Count(); i++)
            {
                string tmp = "Answer= " + Answers.ElementAt(i) + " : Events(";
                if (Events.Count > i)
                {
                    foreach (string s in Events.ElementAt(i))
                    {
                        tmp += s + ",";
                    }
                    tmp = tmp.Substring(0, tmp.Length - 1);
                }
                tmp += ")";
                MessageBox.Show(tmp);
            }

            foreach (TreeNode n1 in AnswersNodes)
            {
                if (n1 != null) n1.Show();
            }
        }

    }
}
