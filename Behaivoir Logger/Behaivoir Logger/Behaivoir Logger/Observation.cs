using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Behaivoir_Logger
{
    public partial class Observation : Form
    {
        List<String> obsBtnTextList = new List<String>();
        List<String> obsLogTextList = new List<String>();
        List<String[]> obsWHabitsList = new List<String[]>();
        List<String> obsPosNegList = new List<String>();
        private string courseName;
        private string suffix;
        public string userName
        {
            get;
            set;
        }
        public Observation(string className,string fileSuffix)
        {
            suffix = fileSuffix;
            courseName = className;
            InitializeComponent();
            CreatingNewButtons();
        }

        /*
         * This method will read in the Observation.cfg file and parse the data to set up the Observation form
         */
        private void CreatingNewButtons()
        {
            //move to config file
            int horizotal = 30;
            int vertical = 30;
            int rowSize = 5;
            int seatsLeftAsle = 5;

            //read in button information from cfg file

            string line;
            bool foundButton = false;
            //read in config file to set up all of the buttons and corresponding data
            StreamReader inputFile;
            inputFile = File.OpenText("Observation.txt");
            while (!inputFile.EndOfStream){
	            line = inputFile.ReadLine();
                //find start of a button - allow to exclude comments etc in cfg file
                if (!foundButton)
                {
                    if (line.Contains("startButton"))
                    {
                        foundButton = true;
                    }
                    continue;
                }
                else
                {
                    
                    if (line.Contains("endButton"))
                    {
                        foundButton = false;
                        continue;
                    }
                    else
                    {
                        //should be inside start/end Button in config file
                        if (line.Contains("Name="))
                        {
                            //add name of observation to the list
                            String[] subStrings = line.Split('=');
                            obsBtnTextList.Add(subStrings[1]);
                        }
                        else if (line.Contains("Text="))
                        {
                            //add default comment to list of observation to the list
                            String[] subStrings = line.Split('=');
                            obsLogTextList.Add(subStrings[1]);
                        }
                        else if (line.Contains("Work Habits="))
                        {
                            //add list of workhabits to be affected to list
                            String[] tmpSubStrings = line.Split('=');
                            String[] subStrings = tmpSubStrings[1].Split(',');
                            Array.Sort(subStrings);
                            obsWHabitsList.Add(subStrings);
                        }
                        else if (line.Contains("PosNeg="))
                        {
                            //set default posNeg value
                            String[] subStrings = line.Split('=');
                            obsPosNegList.Add(subStrings[1]);
                        }
                    }
                }
            }
            inputFile.Close();

            for (int i = 0; i < obsBtnTextList.Count; i++)
            {
                Button myButton = new Button();
                myButton.Size = new Size(85, 85);
                myButton.Location = new Point(horizotal, vertical);
                myButton.Text = obsBtnTextList[i];
                myButton.Name = i.ToString();
                myButton.Click += new EventHandler(button_Click);
                myButton.DialogResult = DialogResult.OK;

                horizotal += 90;
                if ((i % rowSize) == seatsLeftAsle - 1)
                {
                    horizotal += 20;
                }
                if ((i % rowSize) == rowSize - 1)
                {
                    horizotal = 30;
                    vertical += 100;
                }
                this.Controls.Add(myButton);
            }
        }
        protected void button_Click(object sender, EventArgs e)
        {
            Button observationType = sender as Button;
                //show popup with text entry to write to file
            string comments;
            using (ConfirmationForm confirmPopup = new ConfirmationForm(obsWHabitsList[Int32.Parse(observationType.Name)]))
            {
                confirmPopup.Text = this.Text + "" + observationType.Text;
                if (confirmPopup.ShowDialog() == DialogResult.OK)
                {
                    //Create a property in ConfirmationForm to return the input of user.
                    comments = confirmPopup.comments;
                    List<int> workHabitsList = new List<int>();
                    for (int i = 0; i < 10; i++)
                    {
                        if (confirmPopup.whButtonList[i].Checked)
                        {
                            workHabitsList.Add(i+1);
                        }
                    }
                   WriteToGoogleSheets(comments, observationType, workHabitsList);
                   
                }
            }
         

        }


        //fixme make it write to Google Sheets
        private void WriteToGoogleSheets(string comments, Button observationType, List<int> workHabitsList)
        {
   
            //write datetime,username,observer, obsLogTextList,comments,partPoints,countable,workhabit stuff
            string dateTime = DateTime.Now.ToString("yyyy'-'MM'-'dd HH':'mm':'ss");
            string defaultComment = obsLogTextList[Int32.Parse(observationType.Name)];
        
            List<object> writeData = new List<object>();
            writeData.Add(dateTime);
            writeData.Add(userName);
            writeData.Add("David Baker");
            writeData.Add(defaultComment);
            writeData.Add(comments);
            writeData.Add(obsPosNegList[Int32.Parse(observationType.Name)]);
            writeData.Add(1);//countable
            int obsWHabitsIndex = 0;
            for (int i = 1; i <= 10; i++)
            {
                //this compares to see if item in list in config file matches this list
                if (workHabitsList[obsWHabitsIndex] == i)
                {

                    writeData.Add(obsPosNegList[Int32.Parse(observationType.Name)]);
                    obsWHabitsIndex++;
                }

                if (obsWHabitsIndex >= workHabitsList.Count)
                {
                    break;
                }
            }

            string sheetName = Form1.studentDictionary[userName].getSheetName();
            int rowNum = Utils.GetNextRowNum(Form1.spreadsheetId, sheetName, "A1", "B");
            Utils.WriteData(Form1.spreadsheetId, writeData, sheetName, "A" + rowNum, "R" + rowNum);
           
        }
    }
}
