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
        public static Dictionary<String, Student> studentDictionary;
        private bool snapMode;
        private bool reportMode;
        private bool attendanceMode;
        private bool amClassMode;
        private string amConfigSheet;
        private string pmConfigSheet;
        private string amPowerSchoolSheet;
        private string pmPowerSchoolSheet;
        private string courseName;
        private string suffix;
        public static String spreadsheetId; //fixme
        const int powerSchoolStartRow = 10;
        const int canvasStartRow = 4;

        public Form1()
        {
            InitializeComponent();
            //open manage book

            studentDictionary = new Dictionary<string, Student>();
            courseName = "Programming";//temp for now fixme
            suffix = "_18_19";//temp for now fixme
            spreadsheetId = "1qLpKVRoHulDbstC_BOE9X5X81iJY-uQoerFtBO6IhE8";//Utils.GetSheetIdByTitle(courseName + suffix);
            amConfigSheet = "StudentInfoAM";
            pmConfigSheet = "StudentInfoPM";
            amPowerSchoolSheet = "PowerSchoolAM";
            pmPowerSchoolSheet = "PowerSchoolPM";
            amClassMode = true;
            snapMode = false;
            attendanceMode = false;
            reportMode = false;
            this.Text = courseName + " AM";
            CreatingNewButtons(amConfigSheet,amPowerSchoolSheet);
        }
   

        private void CreatingNewButtons(string studentCfgFileName,string powerSchoolSheet)
        {
            int userNameCol = 1;
            int displayNameCol = 0;
            int sheetIDCol = 2; //need to create function
            //read section from CfgSheet
            IList<IList<Object>> studentConfigData =Utils.GetSpreadData(spreadsheetId, studentCfgFileName, "A1","C");//fix range
            IList<IList<Object>> powerSchoolRowData = Utils.GetSpreadData(spreadsheetId, powerSchoolSheet, "E"+powerSchoolStartRow, "E");//fix range
            IList<IList<Object>> canvasRowData = Utils.GetSpreadData(spreadsheetId, "CanvasGrades", "B"+canvasStartRow, "B");//fix range
            if (studentConfigData != null && studentConfigData.Count > 0)
            {
                int rowNum = powerSchoolStartRow;
             /*   foreach (var row in powerSchoolRowData)
                {
                    try
                    {
                        studentDictionary[(string)row[0]].powerSchoolRow = rowNum; //add try catch to handle student not found.
                    }
                    catch (Exception ex) { }
                    rowNum++;
                }
                rowNum = canvasStartRow;
                foreach (var row in canvasRowData)
                {
                    try
                    {
                        studentDictionary[(string)row[0]].canvasRow = rowNum;
                    }
                    catch (Exception ex) { } //ignore and move on if not found
                    rowNum++;
                }*/
                rowNum = 0;//skip header row
                foreach (var row in studentConfigData)
                {
                    rowNum++;
                    if(rowNum == 1){continue;}//skip first row
                    //check if tab exists, if not add tab, create sheet and share
                    //create new student and add in information.  Add student to studentDictionary
                    if (row.Count ==2)
                    {
                        userNameList.Add((string)row[userNameCol]);
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
                        userNameList.Add((string)row[userNameCol]);
                        if (row[userNameCol].Equals("x")) { continue; }
                        studentDictionary.Add((string)row[userNameCol], new Student((string)row[userNameCol], (string)row[displayNameCol], courseName+suffix, (string)row[sheetIDCol],spreadsheetId));
                    }
                    else
                    {
                        Console.WriteLine("could not find username and display name for student on row."+rowNum);
                        //Console.ReadKey();
                    }
                }
     
             }
            else
            {
                Console.WriteLine("No data found.");
            }
            int horizotal = 30;
            int vertical = 70;
            int rowSize = 5;
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
            foreach (String userName in userNameList)
            {
                
                EnhancedButton myButton = new EnhancedButton(userName);
                myButton.Size = new Size(85, 85);
                myButton.Location = new Point(horizotal, vertical);
                myButton.Text = "x";
                if (!userName.Equals("x"))
                {
                    myButton.Text = studentDictionary[userName].getDisplayName();
                }
         
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
            if (studentBtn.Text != "x")
            {
                if (!snapMode && !attendanceMode)
                {
                    //open new form pass in student name
                    using (Observation observationPopup = new Observation(courseName, suffix))
                    {
                        observationPopup.Text = studentBtn.Text;
                        observationPopup.userName = studentBtn.userName;
                        if (observationPopup.ShowDialog() == DialogResult.OK)
                        {
                        }
                    } //disable for now to prevent crash
                }
                else if (snapMode)
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
                else if (attendanceMode)
                {
                    if (studentBtn.BackColor == Color.Green)
                    {
                        studentBtn.BackColor = Color.Transparent;
                    }
                    else
                    {
                        studentBtn.BackColor = Color.Green;
                    }

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
                userNameList.Clear();
                CreatingNewButtons(amConfigSheet,"");
                backBtn.Text = "Switch to PM";
            }
            else
            {
                this.Text = courseName + " PM";
                studentDictionary.Clear();
                userNameList.Clear();
                CreatingNewButtons(pmConfigSheet,"");
                backBtn.Text = "Switch to AM";
            }
        }

        private void snapshotBtn_Click(object sender, EventArgs e)
        {
            //swap between modes on click of button
            snapMode = !snapMode;
            reportMode = false;
            attendanceMode = false;
            
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
                if (tempBtn.Text != "x")
                {
                    if (snapMode)
                    {
                        if (tempBtn.absent)
                        {
                            tempBtn.BackColor = Color.Transparent;
                        }
                        else
                        {
                            tempBtn.BackColor = Color.Green;
                        }
                        
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
            submitBtn.Visible = false;

            int posNeg;

            foreach (EnhancedButton tempBtn in buttonList)
            {
                if (tempBtn.Text != "x")
                {
                    string comments = "";
                    String[] wrkHabitsArray = { "4", "5", "7" };
                    List<int> workHabitsList = new List<int>();
                    if (tempBtn.BackColor == Color.Green)
                    {
                        posNeg = 0;
                        //write good
                        foreach (string wrkHabit in wrkHabitsArray)
                        {
                            workHabitsList.Add(Int32.Parse(wrkHabit));
                        }
                        if (snapMode)
                        {
                            WriteToCSV(tempBtn.userName, posNeg, comments, workHabitsList);
                        }
                        else if (attendanceMode)
                        {
                            WriteToSheetAttendance(tempBtn.userName);
                            
                        }
                        //WriteData(String spreadSheetID, List<object> cellData, string SheetName, String startCell, String endCell)

                    }
                    else if (tempBtn.BackColor == Color.Red || tempBtn.BackColor == Color.Yellow)
                    {
                        posNeg = -1;
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
            snapMode = false;
            attendanceMode = false;
            snapshotBtn.BackColor = Color.Transparent;
            attendanceBtn.BackColor = Color.Transparent;

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
            List<object> writeData = new List<object>();
            writeData.Add(dateTime);
            writeData.Add(userName);
            writeData.Add("David Baker");
            writeData.Add(defaultComments[onTask]);
            writeData.Add(comments);
            writeData.Add(onTask);
            writeData.Add(0);

            Utils.WriteData(spreadsheetId, writeData, sheetName, "A" + rowNum, "H" + rowNum);
          

        }

        private void WriteToSheetAttendance(string userName)
        {
            //get last row written
            Student test = studentDictionary[userName];
            string sheetName = studentDictionary[userName].getSheetName();
            int rowNum = Utils.GetNextRowNum(spreadsheetId, sheetName, "A1", "B");

            string defaultComments;
            defaultComments = "Daily WorkHabit and Participation Totals";
            //write the information to sheet, set up cell data list(date,name,observation,comments,workhabits1-10,countableObservation

            //write date
            string dateTime = DateTime.Now.ToString("MM/dd/yyyy");
            List<object> writeData = new List<object>();
            writeData.Add(dateTime);
            writeData.Add(userName);
            writeData.Add("David Baker");
            writeData.Add(defaultComments);
            writeData.Add("");
            int prevRow = rowNum - 1;
            string formula = "=IF(SUM((filter(F2:F" + prevRow + ",$A2:$A" + prevRow + ">$A" + rowNum + ")))+5<0,0,5+SUM((filter(F2:F" + prevRow + ",$A2:$A" + prevRow + ">$A" + rowNum + "))))";
            writeData.Add(formula);
            //countable
            formula = "=IF(SUM((filter(G2:G" + prevRow + ",$A2:$A" + prevRow + ">$A" + rowNum + ")))+5<0,0,5+SUM((filter(G2:G" + prevRow + ",$A2:$A" + prevRow + ">$A" + rowNum + "))))";
            writeData.Add(formula);
            //WH1
            formula = "=IF(SUM((filter(H2:H" + prevRow + ",$A2:$A" + prevRow + ">$A" + rowNum + ")))+5<0,0,5+SUM((filter(H2:H" + prevRow + ",$A2:$A" + prevRow + ">$A" + rowNum + "))))";
            writeData.Add(formula);
            //WH2
            formula = "=IF(SUM((filter(I2:I" + prevRow + ",$A2:$A" + prevRow + ">$A" + rowNum + ")))+5<0,0,5+SUM((filter(I2:I" + prevRow + ",$A2:$A" + prevRow + ">$A" + rowNum + "))))";
            writeData.Add(formula);
            //WH3
            formula = "=IF(SUM((filter(J2:J" + prevRow + ",$A2:$A" + prevRow + ">$A" + rowNum + ")))+5<0,0,5+SUM((filter(J2:J" + prevRow + ",$A2:$A" + prevRow + ">$A" + rowNum + "))))";
            writeData.Add(formula);
            //WH4
            formula = "=IF(SUM((filter(K2:K" + prevRow + ",$A2:$A" + prevRow + ">$A" + rowNum + ")))+5<0,0,5+SUM((filter(K2:K" + prevRow + ",$A2:$A" + prevRow + ">$A" + rowNum + "))))";
            writeData.Add(formula);
            //WH5
            formula = "=IF(SUM((filter(L2:L" + prevRow + ",$A2:$A" + prevRow + ">$A" + rowNum + ")))+5<0,0,5+SUM((filter(L2:L" + prevRow + ",$A2:$A" + prevRow + ">$A" + rowNum + "))))";
            writeData.Add(formula);
            //WH6
            formula = "=IF(SUM((filter(M2:M" + prevRow + ",$A2:$A" + prevRow + ">$A" + rowNum + ")))+5<0,0,5+SUM((filter(M2:M" + prevRow + ",$A2:$A" + prevRow + ">$A" + rowNum + "))))";
            writeData.Add(formula);
            //WH7
            formula = "=IF(SUM((filter(N2:N" + prevRow + ",$A2:$A" + prevRow + ">$A" + rowNum + ")))+5<0,0,5+SUM((filter(N2:N" + prevRow + ",$A2:$A" + prevRow + ">$A" + rowNum + "))))";
            writeData.Add(formula);
            //WH8
            formula = "=IF(SUM((filter(O2:O" + prevRow + ",$A2:$A" + prevRow + ">$A" + rowNum + ")))+5<0,0,5+SUM((filter(O2:O" + prevRow + ",$A2:$A" + prevRow + ">$A" + rowNum + "))))";
            writeData.Add(formula);
            //WH9
            formula = "=IF(SUM((filter(P2:P" + prevRow + ",$A2:$A" + prevRow + ">$A" + rowNum + ")))+5<0,0,5+SUM((filter(P2:P" + prevRow + ",$A2:$A" + prevRow + ">$A" + rowNum + "))))";
            writeData.Add(formula);
            //WH10
            formula = "=IF(SUM((filter(Q2:Q" + prevRow + ",$A2:$A" + prevRow + ">$A" + rowNum + ")))+5<0,0,5+SUM((filter(Q2:Q" + prevRow + ",$A2:$A" + prevRow + ">$A" + rowNum + "))))";
            writeData.Add(formula);
            
      
            Utils.WriteData(spreadsheetId, writeData, sheetName, "A" + rowNum, "R" + rowNum);


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

        private void attendanceBtn_Click(object sender, EventArgs e)
        {

            attendanceMode = !attendanceMode;
            snapMode = false;
            reportMode = false;
            submitBtn.Visible = attendanceMode;
            if (attendanceMode)
            {
                attendanceBtn.BackColor = Color.Green;
                foreach (EnhancedButton tempBtn in buttonList)
                {
                        if (tempBtn.absent)
                        {
                            tempBtn.BackColor = Color.Transparent;
                        }
                        else
                        {
                            tempBtn.BackColor = Color.Green;
                        }
                }
            }
            else
            {
                attendanceBtn.BackColor = Color.Transparent;
                foreach (EnhancedButton tempBtn in buttonList)
                {
                        if (tempBtn.BackColor == Color.Transparent)
                        {
                            tempBtn.absent = true;
                        }
                        else
                        {
                            tempBtn.absent = false;
                        }
                    tempBtn.BackColor = Color.Transparent;
                }
            }

           

        }
    }
}
