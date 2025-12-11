using System;
using System.Collections.Generic;
using PRN232_GradingSystem_Repositories.Models;

namespace PRN232_GradingSystem_API.Models.ResponseModel
{
    public class GroupResponse
    {
        public int Groupid { get; set; }
        public string Groupname { get; set; }
        public int? Semesterid { get; set; }
        public DateTime? Createat { get; set; }
        public DateTime? Updateat { get; set; }
        public int? Createby { get; set; }
        public int? Updateby { get; set; }

        // Navigation properties
        //public User CreatebyNavigation { get; set; }
        //public Semester Semester { get; set; }
        //public User UpdatebyNavigation { get; set; }

        //public IReadOnlyList<GroupStudent> GroupStudents { get; set; }
        //public IReadOnlyList<Student> Students { get; set; }
    }
}
