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
    public enum Node1State
    {
        None,
        Hovered,
        Clicked,
        ClickState
    }

    public class Node1
    {
        public bool bCondition = false;
        public string Condition;

        private Node1State _State;
        public Node1State State // State of the Node1 (clicked etc... (basically, color))
        {
            get { return _State; }
            set
            {
                if (value == _State) return;
                _State = value;
                //ReRun draw
            }
        }

        public bool Input;

        private int _index;
        private int index // Index in order of (input/output)
        {
            get { return _index; }
            set
            {
                if (value == _index) return;
                _index = value;
            }
        }

        public Block parent;
        public int Count = 1;

        public Node1(int nb, bool Answer, Block b)
        {
            index = nb;
            Input = !Answer;
            State = Node1State.None;
            parent = b;
            Count = b.Node1s.Count()-1;
        }

        public void Hover(bool IsHovered)
        {
            if(IsHovered) // Making it hover now
            {
                if (State != Node1State.ClickState && State != Node1State.Clicked) State = Node1State.Hovered;
            }
            else // Stopping the hover now
            {
                if (State == Node1State.Hovered) State = Node1State.None;
            }
        }

        public KeyValuePair<Point,Color> Run(ref bool hasCondition)
        {
            //Returns if it is hovered etc...
            //parent.Bounds
            Count = parent.Node1s.Count()-2;
            // Find index of THIS ? (Not necessary if we don't switch orders)

            Rectangle Node1Point = parent.ClientRectangle;
            Node1Point.X = 0;
            Node1Point.Y += parent.ClientRectangle.Height / 2 - 5;
            Node1Point.Size = new Size(10, 10);
            Color penColor = Color.Red;

            hasCondition = bCondition;
            switch(State)
            {
                case Node1State.Clicked:
                    penColor = Color.Blue; // Or yellow
                    break;
                case Node1State.ClickState:
                    penColor = Color.Yellow;
                    break;
                case Node1State.Hovered:
                    penColor = Color.Green;
                    break;
                case Node1State.None:
                    penColor = Color.Red;
                    break;
            }
                switch (index+1)
                {
                case 0:
                    return new KeyValuePair<Point, Color>(Node1Point.Location, penColor);
                    case 1:
                        Node1Point.X = parent.ClientRectangle.Width - 10;
                        Node1Point.Y = parent.ClientRectangle.Height - 55 + ((Count- (index + (index - 1))) * 10);
                    return new KeyValuePair<Point, Color>(Node1Point.Location, penColor);
                    case 2:
                        Node1Point.Y = parent.ClientRectangle.Height - 55 + ((Count - (index + (index - 1))) * 10);
                    return new KeyValuePair<Point, Color>(Node1Point.Location, penColor);
                    case 3:
                        Node1Point.Y = parent.ClientRectangle.Height - 55 + ((Count - (index + (index - 1))) * 10);
                    return new KeyValuePair<Point, Color>(Node1Point.Location, penColor);
                    case 4:
                        Node1Point.Y = parent.ClientRectangle.Height - 55 + ((Count - (index + (index - 1))) * 10);
                    return new KeyValuePair<Point, Color>(Node1Point.Location, penColor);
                    case 5:
                        Node1Point.Y = parent.ClientRectangle.Height - 55 + ((Count - (index + (index - 1))) * 10);
                    return new KeyValuePair<Point, Color>(Node1Point.Location, penColor);
                }
            return new KeyValuePair<Point, Color>(new Point(), Color.Black);
        }

    }

    public partial class Block : Control
    {

        private bool _Hovering = false;
        private bool Hovering
        {
            get { return _Hovering; }
            set
            {
                if (value == _Hovering) return;
                _Hovering = value;
                Invalidate();
            }
        }

        private bool _Pressed = false;
        private bool Pressed
        {
            get { return _Pressed; }
            set
            {
                if (value == _Pressed) return;
                _Pressed = value;
                Invalidate();
            }
        }

        private Point _startPoint = new Point(0,0);
        private Point startPoint
        {
            get { return _startPoint; }
            set
            {
                if (value == _startPoint) return;
                _startPoint = value;
                Invalidate();
            }
        }

        private Point _endPoint = new Point(0,0);
        private Point endPoint
        {
            get { return _endPoint; }
            set
            {
                if (value == _endPoint) return;
                _endPoint = value;
                Invalidate();
            }
        }

        private int _ID = 0;
        public int ID
        {
            get { return _ID; }
            set
            {
                if (_ID == value) return;
                _ID = value;
                Invalidate();
            }
        }

        private int _SetOutputs = 0; // With % (1 , 2 , 4 , 8 )
        private int SetOutputs
        {
            get { return _SetOutputs; }
            set
            {
                if (value == _SetOutputs) return;
                _SetOutputs = value;
                Invalidate();
            }
        }

        public Block()
        {
            InitializeComponent();
        }

        public new String Text
        {
            get { return base.Text; }
            set
            {
                if (value == base.Text) return;

                base.Text = value;
                Invalidate();
            }
        }

        public enum Shape
        {
            Oval,
            Rectangular
        }

        private Shape _ShapeType = Shape.Rectangular;

        /// <summary>
        /// Modifies the Shape style of the block
        /// </summary>
        [Description("Modifies the Shape style of the block"),
            Category("Appearance"),
            DefaultValue(typeof(Shape), "Rectangular"),
            Browsable(true)]

        public Shape ShapeType
        {
            get { return _ShapeType; }
            set
            {
                if (_ShapeType == value) return;
                _ShapeType = value;
                Invalidate();
            }
        }

        private bool _BlockType = false;
        [Description("Question or Answer"),
            Category("Functionality"),
            DefaultValue(typeof(bool), "false"),
            Browsable(true)]

        public bool BlockType
        {
            get { return _BlockType; }
            set
            {
                if (_BlockType == value) return;
                _BlockType = value;
                Invalidate();
            }
        }

        private bool _Begin = false;
        [Description("Begin Node1"),
            Category("Functionality"),
            DefaultValue(typeof(bool), "false"),
            Browsable(true)]

        public bool Begin
        {
            get { return _Begin; }
            set
            {
                if (_Begin == value) return;
                _Begin = value;
                Invalidate();
            }
        }

        public List<Node1> Node1s;
        private List<bool> Conditions;
        public Node1 InputNode1;
        private Node1State InputNode1State;
        private List<Node1State> Node1States;

        private int _nbAnswers = 0;
        [Description("Number of Answer"),
            Category("Functionality"),
            DefaultValue(typeof(int), "1"),
            Browsable(true)]

        public int nbAnswers
        {
            get { return _nbAnswers; }
            set
            {
                if (_nbAnswers == value) return;
                _nbAnswers = value;
                updateNode1s(value);
                Invalidate();
            }
        }

        private void updateNode1s(int number)
        {
            if (number != Node1s.Count())
            {

                for(int i=Node1s.Count();i<number;i++)
                {
                    Node1s.Add(new Node1(i, true, this)); // Insert the new Node1 //ISSUE HERE (NO Node1S BY DEFAULT)
                    Node1States.Add(Node1State.None);
                    bool Condition = false;
                    PosColors.Add(Node1s.Last().Run(ref Condition));
                    Conditions.Add(Condition);
                }

                if(number < Node1s.Count()) Node1s.RemoveRange(number, Node1s.Count - number); //Remove Node1s that aren't used anymore

                foreach(Node1 n in Node1s)
                {
                    n.Count = number;
                }
            }
        }

        private int selectedTemp = -1;
        public int SelectedNode1 = -1;
        private int _selected = 0; // Add a selected input (maybe just -selected ?)
        public int selected
        {
            get { return _selected; }
            set
            {
                if (_selected == value) return;
                _selected = value;
                Invalidate();
            }
        }

        private bool moveable = false;

        public void Deselect(int index, bool Linked)
        {
            if (Linked)
            {
                if(index == -1)
                {
                    if (SelectedNode1 == 0) InputNode1.State = Node1State.ClickState;
                    else
                    {
                        Node1s[SelectedNode1 - 1].State = Node1State.ClickState;
                    }
                }
                else if (index == 0) InputNode1.State = Node1State.ClickState;
                else Node1s[index - 1].State = Node1State.ClickState;
            }
            else
            {
                if(index == -1)
                {
                    if(InputNode1 != null) InputNode1.State = Node1State.None;
                    foreach(Node1 n in Node1s)
                    {
                        n.State = Node1State.None;
                    }
                }
                else if (index == 0) InputNode1.State = Node1State.None;
                else Node1s[index - 1].State = Node1State.None;
            }
            //Invalidate();
        }

        public event EventHandler SelectedAnswer; //Event of selected Answer
        public event EventHandler SelectedQuestion;
        public event EventHandler Delink;

        private int OverNode1(MouseEventArgs e)
        {                   // ISSUES with X and Y not coordinated properly (Might be due to Paint Location != HoverLocation)
            for (int i = 0; i < nbAnswers + 1; i++)
            {
                int X = 0;
                int Y = ClientRectangle.Height / 2 - 5;
                if(ID != 0)
                {
                    X = InputPosColor.Key.X;
                    Y = InputPosColor.Key.Y;
                    if (e.X > X && e.X < X + 9 && e.Y > Y - 10 && e.Y < Y + 10) return 0; // Over input
                }

                for(int j=0;j<PosColors.Count();j++)
                {
                    X = PosColors[j].Key.X;
                    Y = PosColors[j].Key.Y;
                    if (e.X > X && e.X < X + 9 && e.Y > Y - 10 && e.Y < Y + 10) return j+1; // Over output
                }
            }

            return -1;
        }

        private List<KeyValuePair<Point,Color>> PosColors;
        private KeyValuePair<Point, Color> InputPosColor;

        protected override void OnMouseMove(MouseEventArgs e)
        {
            Hovering = true;

            int temp = OverNode1(e); // Returns hovered element 'index' (or -1)
            if (selectedTemp > -1)//On hover déjà un Node1
            {
                if (selectedTemp != temp) //Différent de celui d'avant
                {
                    if (selectedTemp == 0) InputNode1.Hover(false);
                    else
                    {
                        Node1s[selectedTemp - 1].Hover(false);
                    }
                    //Removed last hovered (Handle clickedState ?) (Good idea to change to a ChangeState function)
                }
            }

            TTip.Show(TipText, this);

            if (temp == 0) InputNode1.Hover(true); //Hovering Input
            else if(temp > 0) // Hovering an output
            {
                Node1s[temp - 1].Hover(true);
            }
            if (selectedTemp != temp) Invalidate();
            selectedTemp = temp;

            bool Condition = false;

            if (ID != 0 && InputNode1 != null)
            {
                InputPosColor = InputNode1.Run(ref Condition);
            }

            if (PosColors != null)
            {
                for (int i = 0; i < nbAnswers; i++) // Returns the colors and positions to draw
                {
                    Condition = false;
                    PosColors[i] = Node1s[i].Run(ref Condition);
                    Conditions[i] = Condition;
                }
            }

            if (moveable) this.Location = new Point(Location.X + e.X-50, Location.Y + e.Y-50);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            Hovering = false;
            if (!Pressed)
            {
                //If only hovered, set to None
            }
            if(selectedTemp > -1) // Smthg hovered (NOT WORKING) (PBBLY because move is called later, use bHovering to regulate this)
            {
                TTip.Hide(this); // Hides the Tooltip whatever its state
                bool Condition = false;
                if(selectedTemp == 0)
                {
                    InputNode1.Hover(false);
                    InputNode1State = InputNode1.State;
                    InputPosColor = InputNode1.Run(ref Condition);
                }
                else
                {
                    Node1s[selectedTemp - 1].Hover(false);
                    Node1States[selectedTemp - 1] = Node1s[selectedTemp -1].State;
                    PosColors[selectedTemp - 1] = Node1s[selectedTemp - 1].Run(ref Condition);
                    Conditions[selectedTemp - 1] = Condition;
                }

                Invalidate();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) 
            {
                //Replace followinig by this

                if (selectedTemp > -1) // If a Node1 is hovered
                {

                    //SelectedNode1 = -1;
                    if (selectedTemp == 0) InputNode1State = Node1State.Clicked;
                    else Node1States[selectedTemp - 1] = Node1State.Clicked;
                    SelectedNode1 = selectedTemp;

                    selectedTemp = -1;

                    if (Control.ModifierKeys == Keys.Alt) this.Delink(this, e);
                    else
                    {
                        this.SelectedAnswer(this, e);
                    }
                }
                else
                {
                    moveable = true;
                    Pressed = true;
                }
            }
        }

        private ToolTip TTip;
        private string TipText;

        public void Init()
        {
            PosColors = new List<KeyValuePair<Point, Color>>();
            Node1s = new List<Node1>();
            Node1States = new List<Node1State>();
            Conditions = new List<bool>();
            TTip = new ToolTip();
            TipText = "Testing";
            if (ID != 0)
            {
                InputNode1 = new Node1(-1, false, this);
                InputNode1State = Node1State.None;
            }
            nbAnswers = 1;
            
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) Pressed = false;
            //startPoint = new Point(0, 0);
            moveable = false;
        }

        public void Delete()
        {
            Controls.Remove(this);
            this.Dispose();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            Graphics gfx = pe.Graphics;
            Rectangle rc = ClientRectangle;
            rc.Width -= 1;
            rc.Height -= 1;
            
            //Node1 output drawing
            

            gfx.FillRectangle(new SolidBrush(Parent.BackColor), ClientRectangle);

            Color fill;
            if (Pressed) fill = Color.Orange;
            else if (Hovering) fill = Color.DarkRed;
            else fill = Color.Black;

            if(ShapeType == Shape.Oval)
            {
                gfx.FillEllipse(new SolidBrush(fill), rc);

                gfx.DrawEllipse(new Pen(Color.Blue, 1.0f), rc);
            }
            else
            {
                gfx.FillRectangle(new SolidBrush(fill), rc);

                gfx.DrawRectangle(new Pen(Color.Blue, 1.0f), rc);
            }

            if(ID != 0)
            {
                gfx.FillRectangle(new SolidBrush(InputPosColor.Value), new Rectangle(InputPosColor.Key, new Size(10, 10)));
            }
            if(PosColors != null)
            {
                foreach (KeyValuePair<Point, Color> a in PosColors)
                {
                    gfx.FillRectangle(new SolidBrush(a.Value), new Rectangle(a.Key, new Size(10, 10)));
                }
            }


            //gfx.DrawEllipse(new Pen(Color.White, 1.0f), Node1Point);

            Font fnt = new Font("Verdana", (float)rc.Height * 0.125f, FontStyle.Bold, GraphicsUnit.Pixel);

            StringFormat sf = new StringFormat();

            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            gfx.DrawString(Text, fnt, new SolidBrush(Color.White), new RectangleF((float)rc.Left, (float)rc.Top, (float)rc.Width, (float)rc.Height), sf);

            gfx.DrawLine(new Pen(Color.Black), startPoint, endPoint);

        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            
        }
    }
}
