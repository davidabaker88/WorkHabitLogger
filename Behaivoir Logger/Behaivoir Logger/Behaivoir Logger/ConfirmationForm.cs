using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Behaivoir_Logger
{
    public partial class ConfirmationForm : Form
    {
        internal string comments;
        public List<CheckBox> whButtonList = new List<CheckBox>();
        public Button posNeg = new Button();
        public ConfirmationForm(String[] wrkHabitsArray)
        {
            InitializeComponent();
            CreatingNewButtons(wrkHabitsArray);
        }
        private void CreatingNewButtons(String[] wrkHabitsArray)
        {
            //set up the buttons for workhabits and + -
            int horizotal = 10;
            int vertical = 155;
            int wrkHabitArrIdx = 0;
            for (int i = 1; i <= 10; i++)
            {
                if (wrkHabitArrIdx >= wrkHabitsArray.Length) 
                {
                    // this should allow to finish writing buttons but should never match
                    wrkHabitArrIdx--; 
                }
                CheckBox myButton = new CheckBox();
                myButton.Size = new Size(50, 50);
                myButton.Location = new Point(horizotal, vertical);
                myButton.Text = i.ToString();
                myButton.Name = i.ToString();
                myButton.Appearance = Appearance.Button;
                if (wrkHabitsArray[wrkHabitArrIdx].Trim() == i.ToString())
                {
                    myButton.Checked = true;
                    wrkHabitArrIdx++;
                }

                horizotal += 60;
                this.Controls.Add(myButton);
                whButtonList.Add(myButton);
            }
            posNeg.Size = new Size(50, 50);
            posNeg.Location = new Point(horizotal, vertical);
            posNeg.Text = "-";
            posNeg.Name = "posNeg";
            this.Controls.Add(posNeg);
            posNeg.Click += new EventHandler(posNeg_Click);
        }

        private void SubmitBtn_Click(object sender, EventArgs e)
        {
            comments = commentBox.Text;
        }
        private void posNeg_Click(object sender, EventArgs e)
        {
            if(posNeg.Text == "-"){
                posNeg.Text = "+";
            }else{
                posNeg.Text = "-";
            }
            
        }

    }
}
