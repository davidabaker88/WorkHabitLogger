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
        private bool snapMode;
        private bool reportMode;
        private bool amClassMode;
        private string amConfigSheet;
        private string pmConfigSheet;
        private string courseName;
        private string suffix;
        private String spreadsheetId = "160fIGNBuzud5JJ4Cd6cjLiUrVF3looxxw-gd1G3dc_s"; //fix me

        public Form1()
        {
            InitializeComponent();
            //open manage book
            

            courseName = "Programming";//temp for now
            suffix = "";

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
            //read section from CfgSheet
            IList<IList<Object>> studentConfigData =Utils.GetSpreadData(spreadsheetId, studentCfgFileName, "A1","C30");//fix range
            if (studentConfigData != null && studentConfigData.Count > 0)
            {
                foreach (var row in studentConfigData)
                {
                    //create new student and add in information.  Add student to studentDictionary
                    foreach (var col in row)
                    {
                        Console.Write(col + ",");
                    }
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("No data found.");
            }
            int horizotal = 30;
            int vertical = 70;
            int rowSize = 7;
            int seatsLeftAsle = 3;
            //System.Windows.SystemParameters.PrimaryScreenWidth 
            //System.Windows.SystemParameters.PrimaryScreenHeight

            //clear out existing information
            foreach (EnhancedButton btnToRemove in buttonList)
            {
                this.Controls.Remove(btnToRemove);
            }
            buttonList.Clear();
            studentNameList.Clear();
            userNameList.Clear();

            //read in button information from cfg file

            string line;
            //read in config file to set up all of the buttons and corresponding data
            StreamReader inputFile;
            inputFile = File.OpenText(studentCfgFileName);
            while (!inputFile.EndOfStream)
            {
                line = inputFile.ReadLine();
                if (line.StartsWith("//"))
                {
                    continue;
                }
                else
                {
                    String[] subStrings = line.Split(',');
                    studentNameList.Add(subStrings[0]);
                    if (subStrings.Length >= 2) 
                    {
                        userNameList.Add(subStrings[1]); 
                    }
                    else
                    {
                        userNameList.Add(subStrings[0]);//temp fix to be first inital+lastname
                    }

                      
                }
            }
            inputFile.Close();

            for (int i = 0; i < studentNameList.Count; i++)
            {
                EnhancedButton myButton = new EnhancedButton();
                myButton.Size = new Size(85, 85);
                myButton.Location = new Point(horizotal, vertical);
                myButton.Text = studentNameList[i];
                myButton.userName = userNameList[i];
                myButton.Click += new EventHandler(button_Click);

                horizotal += 90;
                if ((i % rowSize) == seatsLeftAsle-1)
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
            }
        }
        

        protected void button_Click (object sender, EventArgs e)
        {
            EnhancedButton studentBtn = sender as EnhancedButton;

            if (!snapMode && !reportMode)
            {
                //open new form pass in student name
                using (Observation observationPopup = new Observation(courseName, suffix))
                {
                    observationPopup.Text = studentBtn.Text;
                    observationPopup.userName = studentBtn.userName;
                    if (observationPopup.ShowDialog() == DialogResult.OK)
                    {
                    }
                }
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
                CreatingNewButtons(amConfigSheet);
                backBtn.Text = "Switch to PM";
            }
            else
            {
                this.Text = courseName + " PM";
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
            snapMode = false;
            submitBtn.Visible = false;
            string basePath = "I:/"+courseName+"/";
            string defaultFolder = "BehaviorLogger/";
            string baseName = "BehaviorLog"+suffix+".csv";
            int posNeg;
           
            foreach (EnhancedButton tempBtn in buttonList)
            {
                if (tempBtn.Text != "")
                {
                    string path = basePath + defaultFolder;
                    string backupPath = basePath + defaultFolder + "backup/";
                    string filename = baseName;
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
                        openAndWriteToCSV(tempBtn.Text, path, backupPath, filename, posNeg,comments,workHabitsList);
                        path = basePath + tempBtn.userName + "/" + defaultFolder;
                        filename = tempBtn.userName + baseName;
                        openAndWriteToCSV(tempBtn.Text, path, backupPath, filename, posNeg,comments,workHabitsList);

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
                        openAndWriteToCSV(tempBtn.Text, path, backupPath, filename, posNeg, comments, workHabitsList);
                        path = basePath + tempBtn.userName + "/" + defaultFolder;
                        filename = tempBtn.userName + baseName;
                        openAndWriteToCSV(tempBtn.Text,  path, backupPath, filename, posNeg, comments, workHabitsList);
                    }
                  
                    tempBtn.BackColor = Color.Transparent;
                }
            }
            snapshotBtn.BackColor = Color.Transparent;
        }
        private void openAndWriteToCSV(string studentName, string path, string backupPath, string filename, int onTask, string comments, List<int> workHabitsList)
        {
            string outfileName = path + filename;
            string backOutFile = backupPath + filename;
            Directory.CreateDirectory(path);
            if (!File.Exists(outfileName))
            {
                using (StreamWriter sw = new StreamWriter(outfileName))
                {
                    sw.WriteLine("Time,Name,Observation Text,Comments,Attendance,Safety,Care of Work Area,Good Judgement,Effort,Cooperation,Self Discipline,Quality of Work,Quanity of Work,Dress");
                }
            }
            try
            {
                if (File.Exists(backOutFile))
                {
                    //file is not in use otherwise throws exception
                    using (StreamWriter sw = File.AppendText(outfileName)) { }
                    //move file over to local area
                    File.Copy(backOutFile, outfileName, true);
                    File.Delete(backOutFile);
                }
                using (StreamWriter sw = File.AppendText(outfileName))
                {
                    WriteToCSV(studentName, onTask, comments, sw, workHabitsList);
                }
            }
            catch
            {
                //File is already open need to write to backup file
                Directory.CreateDirectory(backupPath);
                if (!File.Exists(backOutFile))
                {
                    File.Copy(outfileName, backOutFile);
                }
                using (StreamWriter sw = File.AppendText(backOutFile))
                {
                    WriteToCSV(studentName, onTask, comments, sw, workHabitsList);
                }
            }
        }
        private void WriteToCSV(string studentName, int onTask, string comments, StreamWriter sw, List<int> workHabitsList)
        {
            string[] defaultComments = new string[2];
            defaultComments[0] = "Student was not on task when doing scan of the class";
            defaultComments[1] = "Student was on task when doing scan of the class";
            //write the information to csv file
            string dateTime = DateTime.Now.ToString("yyyy'-'MM'-'dd HH':'mm':'ss");
            sw.Write(dateTime + "," + studentName + "," + defaultComments[onTask].Replace(",", ";").Replace(System.Environment.NewLine, ".  ") + "," + comments.Replace(",", ";").Replace(System.Environment.NewLine, ".  ") + ",");
            //write the information regarding workhabit categories to csv file
            int index = 0;
            for (int i = 1; i <= 10; i++)
            {
                //this compares to see if item in list in config file matches this list
                if (workHabitsList[index] == i)
                {
                    if (onTask == 1)
                    {
                        sw.Write("+");
                    }
                    else
                    {
                        sw.Write("-");
                    }
                    index++;
                    if (index >= workHabitsList.Count)
                    {
                        break;
                    }
                }
                sw.Write(",");
            }
            sw.Write("\n");
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
