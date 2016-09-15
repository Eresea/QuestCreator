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
    public partial class OpenOnline : Form
    {
        public string selectedName;
        private ListViewItem selectedItem;
        ExampleForm parent;
        List<string> Names;

        public OpenOnline(ExampleForm Example)
        {
            parent = Example;
            InitializeComponent();
        }
        public OpenOnline()
        {
            InitializeComponent();
        }

        private void OpenOnline_Load(object sender, EventArgs e)
        {
            string url = "http://saoproject.livehost.fr/SAOProject/Dialogs.php?Function=ListNames";
            using (var Client = new System.Net.WebClient())
            {
                var response = Client.DownloadString(url);
                if (response.Substring(0, 5) == "LIST:")
                {
                    response = response.Substring(5).Remove(response.Substring(5).Length-1);
                    Names = response.Split('|').ToList();
                }
            }

            foreach(string s in Names)
            {
                listView1.Items.Add(s);
            }

            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (listView1.SelectedItems.Count > 0) { selectedName = listView1.SelectedItems[0].Text; selectedItem = listView1.SelectedItems[0]; }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Return("");
        }

        private void Return(string s)
        {
            parent.openFromOnline(s);
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Return(selectedName);
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar ==(char)Keys.Enter)
            {
                filter(textBox1.Text);
            }
            else filter(textBox1.Text);
        }

        private void filter(string s)
        {
            listView1.Items.Clear();
            foreach(string name in Names)
            {
                if(name.CaseInsensitiveContains(s))
                {
                    listView1.Items.Add(name);
                }
            }
        }

        private void OpenOnline_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)Keys.Escape) // Doesn't seem to have focus (pbbly because of style of the form)
            {
                Return("");
            }
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newName = Prompt.ShowDialog("New name", "");

            string url = "http://saoproject.livehost.fr/SAOProject/Dialogs.php?Function=Rename&Data=" + selectedName + "|" + newName;
            using (var Client = new System.Net.WebClient())
            {
                var response = Client.DownloadString(url);
                if (response == "Success")
                {
                    Names[Names.IndexOf(selectedName)] = newName;
                    selectedName = newName;
                    selectedItem.Text = newName;
                }
                else MessageBox.Show("Error");
            }
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                if(listView1.FocusedItem.Bounds.Contains(e.Location) == true)
                {
                    contextMenuStrip1.Show(Cursor.Position);
                }
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string url = "http://saoproject.livehost.fr/SAOProject/Dialogs.php?Function=Remove&Data=" + selectedName;
            using (var Client = new System.Net.WebClient())
            {
                var response = Client.DownloadString(url);
                if (response == "Success")
                {
                    Names.RemoveAt(Names.IndexOf(selectedName));
                    listView1.Items.RemoveAt(listView1.Items.IndexOf(selectedItem));
                    selectedName = "";
                    selectedItem = null;
                }
                else MessageBox.Show("Error");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            selectedName = "";
            foreach(ListViewItem lvi in listView1.Items)
            {
                selectedName += lvi.Text + "|";
            }
            selectedName = selectedName.Remove(selectedName.Length - 1);

            Return(selectedName);
        }
    }

    public static class Extensions
    {
        public static bool CaseInsensitiveContains(this string text, string value,
            StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase)
        {
            return text.IndexOf(value, stringComparison) >= 0;
        }
    }
}
