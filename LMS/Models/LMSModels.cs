using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models
{
    public class Group
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } //gruppnamn
    }


    public class User //LMS användare
    {
        [Key]
        public int Id { get; set; }

        //Properties
        [Display(Name = "First name")]
        public string FName { get; set; } //förnamn
        [Display(Name = "Last name")]
        public string LName { get; set; } //efternamn        
    }

    public class Teacher
    {
        [Key]
        public int Id { get; set; }

        //Connection
        [ForeignKey("Id")]
        public virtual User UserId { get; set; }
    }

    public class Student
    {
        [Key]
        public int Id { get; set; }

        //Connection
        [ForeignKey("Id")]
        public virtual User UserId { get; set; }
        [ForeignKey("Id")]
        public virtual Group GroupId { get; set; }
    }

    public class File
    {
        [Key]
        public int Id { get; set; }

        public string FilePath { get; set; } // sökväg exlusive filnamn
        public string FileName { get; set; } // filnamn extern
        private string FileNameInternal { get; set; } // filnamn intern
        //public int GroupId { get; set; } // en fil kan tillhöra en grupp 


        //Connection
        [ForeignKey("Id")]
        public int UserId { get; set; } // en fil kan tillhöra en användare
        


        // alt 1: associeras via kopplingstabell till filkarakteristik som om den är delad, om den är föreläsnings eller uppgiftsunderlag, om den är inlämningsuppgift
        // alt 2: binär eller integer fält per karakteristik vars värde avgör om den är delad, om den är föreläsnings eller uppgiftsunderlag, om den är inlämningsuppgift
        // alt 3: hexadecimalt fält (ett enda fält) vars värde avgör kombinationen om den är delad, om den är föreläsnings eller uppgiftsunderlag, om den är inlämningsuppgift
        // pga komplexitet och pga förenklad utvidgning, föredras kopplingsalternativet

    }

    public class Schedule
    {
        [Key]
        public int ScheduleId { get; set; }
        
    }

}