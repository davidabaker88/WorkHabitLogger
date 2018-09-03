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
    public partial class Form1 : Form
    {
        List<String> studentNameList = new List<String>();
        List<String> userNameList = new List<String>();
        List<EnhancedButton> buttonList = new List<EnhancedButton>();
        Dictionary<String, Student> studentDictionary= new Dictionary<string, Student>();
        private bool snapMode;
        private bool reportMode;
        private bool amClassMode;
        private string amConfigSheet;
        private string pmConfigSheet;
        private string courseName;
        private string suffix;
        private String spreadsheetId; //fixme

        public Form1()
        {
            InitializeComponent();
            //open manage book
            

            courseName = "Programming";//temp for now fixme
            suffix = "_18_19";//temp for now fixme
            spreadsheetId = "1qLpKVRoHulDbstC_BOE9X5X81iJY-uQoerFtBO6IhE8";//Utils.GetSheetIdByTitle(courseName + suffix);
            amConfigSheet = "StudentInfoAM";
            pmConfigSheet = "StudentInfoPM";
            amClassMode = true;
            snapMode = false;
            reportMode = false;
            this.Text = courseName + " AM";
            CreatingNewButtons(amConfigSheet);
        }
   

        private void CreatingNewButtons(string studentCfgFileName)
        {
            int userNameCol = 1;
            int displayNameCol = 0;
            int sheetIDCol = 2; //need to create function
            //read section from CfgSheet
            IList<IList<Object>> studentConfigData =Utils.GetSpreadData(spreadsheetId, studentCfgFileName, "A1","C30");//fix range
            if (studentConfigData != null && studentConfigData.Count > 0)
            {
                int rowNum = 0;//skip header row
                foreach (var row in studentConfigData)
                {
                    rowNum++;
                    if(rowNum == 1){continue;}//skip first row
                    //check if tab exists, if not add tab, create sheet and share
                    //create new student and add in information.  Add student to studentDictionary
                    if (row.Count ==2)
                    {
                        studentDictionary.Add((string)row[userNameCol], new Student((string)row[userNameCol], (string)row[displayNameCol], courseName+suffix,spreadsheetId));
                        //write new sheet ID to cell
                        Utils.WriteCellData(spreadsheetId, studentDictionary[(string)row[userNameCol]].getSheetID(), studentCfgFileName, "C" + rowNum);
                        //create new tab
                        Utils.AddTabToSpreadSheet(spreadsheetId, studentDictionary[(string)row[userNameCol]].getSheetName());
                        //fill in header information
                        List<object> cellDataList = new List<object> { "dateTime","username","observer","observation","comments","ontask(0 for offtask)","countable" };
                        Utils.WriteData(spreadsheetId, cellDataList, studentDictionary[(string)row[userNameCol]].getSheetName(), "A1", "G1");
                    }
                    else if (row.Count ==3)
                    {
                        studentDictionary.Add((string)row[userNameCol], new Student((string)row[userNameCol], (string)row[displayNameCol], courseName+suffix, (string)row[sheetIDCol],spreadsheetId));
                    }
                    else
                    {
                        Console.WriteLine("could not find username and display name for student on row."+rowNum);
                        Console.ReadKey();
                    }
                }
            }
            else
            {
                Console.WriteLine("No data found.");
            }
            int horizotal = 30;
            int vertical = 70;
            int rowSize = 7;
            int seatsLeftAisle = 8;
            int seatsRightAisle = 8;
            //System.Windows.SystemParameters.PrimaryScreenWidth 
            //System.Windows.SystemParameters.PrimaryScreenHeight

            //clear out existing information
            foreach (EnhancedButton btnToRemove in buttonList)
            {
                this.Controls.Remove(btnToRemove);
            }
            buttonList.Clear();

            int i = 0;
            foreach (String userName in studentDictionary.Keys)
            {
                
                EnhancedButton myButton = new EnhancedButton();
                myButton.Size = new Size(85, 85);
                myButton.Location = new Point(horizotal, vertical);
                myButton.Text = studentDictionary[userName].getDisplayName();
                myButton.userName = userName;
                myButton.Click += new EventHandler(button_Click);

                horizotal += 90;
                if ((i % rowSize) == seatsLeftAisle-1)
                {
                    horizotal += 20;
                }
                if ((i % rowSize) == seatsRightAisle - 1)
                {
                    horizotal += 20;
                }
                if ((i % rowSize) == rowSize-1)
                {
                    horizotal = 30;
                    vertical += 100;
                }
                this.Controls.Add(myButton);
                buttonList.Add(myButton);
                i++;
            }
        }
        

        protected void button_Click (object sender, EventArgs e)
        {
            EnhancedButton studentBtn = sender as EnhancedButton;

            if (!snapMode && !reportMode)
            {
                //open new form pass in student name
               /* using (Observation observationPopup = new Observation(courseName, suffix))
                {
                    observationPopup.Text = studentBtn.Text;
                    observationPopup.userName = studentBtn.userName;
                    if (observationPopup.ShowDialog() == DialogResult.OK)
                    {
                    }
                }*/ //disable for now to prevent crash
            }
            else
            {
                if (studentBtn.BackColor == Color.Green)
                {
                    studentBtn.BackColor = Color.Red;
                }
                else if (studentBtn.BackColor == Color.Red)
                {
                    studentBtn.BackColor = Color.Yellow;
                }
                else if (studentBtn.BackColor == Color.Yellow)
                {
                    studentBtn.BackColor = Color.Transparent;
                }
                else
                {
                    studentBtn.BackColor = Color.Green;
                }
            }

        }

        private void backBtn_Click(object sender, EventArgs e)
        {
            //toggle between classes
            amClassMode = amClassMode ^ true;
            if (amClassMode)
            {
                this.Text = courseName+" AM";
                studentDictionary.Clear();
                CreatingNewButtons(amConfigSheet);
                backBtn.Text = "Switch to PM";
            }
            else
            {
                this.Text = courseName + " PM";
                studentDictionary.Clear();
                CreatingNewButtons(pmConfigSheet);
                backBtn.Text = "Switch to AM";
            }
        }

        private void snapshotBtn_Click(object sender, EventArgs e)
        {
            //swap between modes on click of button
            snapMode = snapMode ^ true;
            reportMode = false;
            
            submitBtn.Visible = snapMode;
            if (snapMode)
            {
                snapshotBtn.BackColor = Color.Green;
            }
            else
            {
                snapshotBtn.BackColor = Color.Transparent;
            }
           
            foreach (EnhancedButton tempBtn in buttonList)
            {
                if (tempBtn.Text != "")
                {
                    if (snapMode)
                    {
                        tempBtn.BackColor = Color.Green;
                    }
                    else
                    {
                        tempBtn.BackColor = Color.Transparent;
                    }
                }
            }
            

        }

        private void submitBtn_Click(object sender, EventArgs e)
        {
            //needs to write to google sheet.
            snapMode = false;
            submitBtn.Visible = false;

            

           
            int posNeg;
           
            foreach (EnhancedButton tempBtn in buttonList)
            {
                if (tempBtn.Text != "")
                {
                    string comments = "";
                    String[] wrkHabitsArray = { "4", "5", "7" };
                    List<int> workHabitsList = new List<int>();
                    if (tempBtn.BackColor == Color.Green)
                    {
                        posNeg = 1;
                        //write good
                        foreach (string wrkHabit in wrkHabitsArray)
                        {
                            workHabitsList.Add(Int32.Parse(wrkHabit));
                        }
                        WriteToCSV(tempBtn.userName, posNeg, comments, workHabitsList);
                        //WriteData(String spreadSheetID, List<object> cellData, string SheetName, String startCell, String endCell)

                    }
                    else if (tempBtn.BackColor == Color.Red || tempBtn.BackColor == Color.Yellow)
                    {
                        posNeg = 0;
                        //write bad
                        if (tempBtn.BackColor == Color.Yellow)
                        {
                            using (ConfirmationForm confirmPopup = new ConfirmationForm(wrkHabitsArray))
                            {
                                confirmPopup.Text = this.Text + "" + tempBtn.userName;
                                if (confirmPopup.ShowDialog() == DialogResult.OK)
                                {
                                    //Create a property in ConfirmationForm to return the input of user.
                                    comments = confirmPopup.comments;
                                    for (int i = 0; i < 10; i++)
                                    {
                                        if (confirmPopup.whButtonList[i].Checked)
                                        {
                                            workHabitsList.Add(i + 1);
                                        }
                                        string posNegTemp = confirmPopup.posNeg.Text;
                                        if (posNegTemp == "+")
                                        {
                                            posNeg = 1;
                                        }
                                        else
                                        {
                                            posNeg = 0;
                                        }

                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (string wrkHabit in wrkHabitsArray)
                            {
                                workHabitsList.Add(Int32.Parse(wrkHabit));
                            }
                        }
                        WriteToCSV(tempBtn.userName, posNeg, comments, workHabitsList);
                    }
                  
                    tempBtn.BackColor = Color.Transparent;
                }
            }
            snapshotBtn.BackColor = Color.Transparent;
        }

        private void WriteToCSV(string userName, int onTask, string comments, List<int> workHabitsList)
        {
            //get last row written
            Student test = studentDictionary[userName];
            string sheetName = studentDictionary[userName].getSheetName();
            int rowNum = Utils.GetNextRowNum(spreadsheetId, sheetName, "A1", "B");
            
            string[] defaultComments = new string[2];
            defaultComments[0] = "Student was not on task when doing scan of the class";
            defaultComments[1] = "Student was on task when doing scan of the class";
            //write the information to sheet, set up cell data list(date,name,observation,comments,workhabits1-10,countableObservation

            //write date
            string dateTime = DateTime.Now.ToString("yyyy'-'MM'-'dd HH':'mm':'ss");
            Utils.WriteCellData(spreadsheetId, dateTime, sheetName, "A" + rowNum);
            //write student name for debug to make sure it works
            Utils.WriteCellData(spreadsheetId, userName, sheetName, "B" + rowNum);
            //write observer
            Utils.WriteCellData(spreadsheetId, "David Baker", sheetName, "C"+rowNum); //fix me replace hardcode with text box
            //write observation
            Utils.WriteCellData(spreadsheetId, defaultComments[onTask], sheetName, "D"+rowNum);
            //write comments
            Utils.WriteCellData(spreadsheetId, comments, sheetName, "E" + rowNum);
            //write 1 or zero (positive or negative)
            Utils.WriteCellData(spreadsheetId, onTask, sheetName, "F" + rowNum);
            //write a 1 for countable
            Utils.WriteCellData(spreadsheetId, 1, sheetName, "G" + rowNum);

        }

        private void pullReportBtn_Click(object sender, EventArgs e)
        {
            reportMode = reportMode ^ true;
            snapMode = false;
            snapshotBtn.BackColor = Color.Transparent;

            dateLabel1.Visible = reportMode;
            dateLabel2.Visible = reportMode;
            fromLabel.Visible = reportMode;
            toLabel.Visible = reportMode;
            fromTextBox.Visible = reportMode;
            toTextBox.Visible = reportMode;

            if (reportMode)
            {
                pullReportBtn.BackColor = Color.Blue;
            }
            else
            {
                pullReportBtn.BackColor = Color.Transparent;
            }

            foreach (EnhancedButton tempBtn in buttonList)
            {
                if (tempBtn.Text != "")
                {
                    if (reportMode)
                    {
                        tempBtn.BackColor = Color.Transparent; // need to decide if allow for full class report
                    }
                    else
                    {
                        tempBtn.BackColor = Color.Transparent;
                    }
                }
            }
        }

      

    }
}
