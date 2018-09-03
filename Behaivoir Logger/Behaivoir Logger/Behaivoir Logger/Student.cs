using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace Behaivoir_Logger
{
    class Student
    {
        private String userName;
        private String displayName;
        private String sheetID;
        private String courseName;

        public Student(String UserName, String DisplayName,String CourseName,String mainSheetID)
        {
            userName = UserName;
            displayName = DisplayName;
            courseName = CourseName;
            sheetID = CreateNewStudentSheet(userName + courseName,mainSheetID);
        }
        public Student(String UserName, String DisplayName, String CourseName, String SheetID,String mainSheetID)
        {
            userName = UserName;
            displayName = DisplayName;
            courseName = CourseName;
            sheetID = SheetID;
        }
        public String getUserName() { return userName; }
        public String getDisplayName() { return displayName; }
        public String getSheetID() { return sheetID; }
        public String getSheetName() { return userName +"_"+ courseName; }
        private String CreateNewStudentSheet(String SheetName,String mainSheetID)
        {
            //create sheet
            IList<Sheet> sheets = new Sheet[1];
            sheets[0] = Utils.MakeNewSheetObject("participation");
            String newSheetID =  Utils.CreateNewSheet(userName +"_"+ courseName,sheets);
            //import cells line
            //=importrange("sheetID","sheetName!A2:B3")
            var formula = "=importrange(\"" + mainSheetID + "\",\"" + getSheetName() + "!A1:K\")";
            Utils.WriteCellData(newSheetID, formula, "participation", "A1");
           
            //lock cells in student sheet
            //share student sheets


            return newSheetID;
        }


        
    }
}
