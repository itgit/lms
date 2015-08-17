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

        public virtual ICollection<File> Files { get; set; }
    }

    public class File
    {
        [Key]
        public string Id { get; set; }
        [Display(Name="File name")]
        public string FileName { get; set; }
        [Display(Name = "File path")]
        public string FilePath { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        [Display(Name = "User")]
        public virtual ApplicationUser User { get; set; }
        public int? GroupId { get; set; }
        [ForeignKey("GroupId")]
        [Display(Name = "Group")]
        public virtual Group Group { get; set; }
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

        [Display(Name = "Start time")]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = @"{0:hh\:mm}")]
        public TimeSpan StartTime { get; set; }

        [Display(Name = "End time")]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = @"{0:hh\:mm}")]
        public TimeSpan EndTime { get; set; }

        [Required]
        public int GroupId { get; set; }
        [ForeignKey("GroupId")]
        [Display(Name = "Group")]
        public virtual Group Group { get; set; }
    }

}