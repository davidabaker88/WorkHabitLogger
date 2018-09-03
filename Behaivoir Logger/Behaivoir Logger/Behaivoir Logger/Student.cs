using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Behaivoir_Logger
{
    class Student
    {
        private String userName;
        private String displayName;
        private String sheetID;
        private String courseName;

        public Student(String UserName, String DisplayName,String CourseName)
        {
            userName = UserName;
            displayName = DisplayName;
            courseName = CourseName;
            sheetID = CreateNewStudentSheet(userName + courseName);
        }
        public Student(String UserName, String DisplayName, String CourseName, String SheetID)
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
        private String CreateNewStudentSheet(String SheetName)
        {
            //create sheet
            String newSheetID =  Utils.CreateNewSheet(userName +"_"+ courseName);
            //add headers to student sheet
            //lock cells in student sheet
            //share student sheets

            
            return newSheetID;
        }


        
    }
}
