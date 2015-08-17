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
        public int? Id { get; set; }
        public string Name { get; set; } //gruppnamn

        public virtual ICollection<Activity> Activities { get; set; }

        public virtual ICollection<ApplicationUser> Users { get; set; }
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

    }

    public enum Day
    {
        Monday = 0,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        //Saturday,
        //Sunday
    };

    public class Activity
    {
        [Key]
        public int Id { get; set; }

        [Display(Name="Name")]
        public string Name { get; set; }
        [Display(Name = "Day")]
        public Day Day { get; set; }
        //public int StartTimeHours { get; set; }
        //public int StartTimeMinutes { get; set; }
        //public int EndTimeHours { get; set; }
        //public int EndTimeMinutes { get; set; }

        [Display(Name = "Starts at")]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = @"{0:hh\:mm}")]
        public TimeSpan StartTime { get; set; }

        [Display(Name = "Ends at")]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = @"{0:hh\:mm}")]
        public TimeSpan EndTime { get; set; }

        //[Display(Name = "Starts at")]
        //public string StartTime { get { return string.Format("{0:00}:{1:00}", StartTimeHours, StartTimeMinutes); } }
        //[Display(Name = "Ends at")]
        //public string EndTime { get { return string.Format("{0:00}:{1:00}", EndTimeHours, EndTimeMinutes); } }

        public int? GroupId { get; set; }
        [ForeignKey("GroupId")]
        public virtual Group Group { get; set; }
    }

}