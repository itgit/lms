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
        [Display(Name = "Group name")]
        public string Name { get; set; } //gruppnamn

        public virtual ICollection<Activity> Activities { get; set; }

        public virtual ICollection<ApplicationUser> Users { get; set; }

        public virtual ICollection<File> Files { get; set; }
    }

    public class File
    {
        [Key]
        public Guid Id { get; set; }
        [Display(Name="File name")]
        public string FileName { get; set; }
        [Display(Name = "File path")]
        public string FilePath { get; set; }
        [Display(Name = "File size in bytes")]
        public int? FileSize { get; set; }
        [Display(Name = "File type")]
        public string FileType { get; set; }
        [Display(Name = "Date")]
        public DateTime FileDate { get; set; }
        public bool? IsShared { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        [Display(Name = "User")]
        public virtual ApplicationUser User { get; set; }
        public int? GroupId { get; set; }
        [ForeignKey("GroupId")]
        [Display(Name = "Group")]
        public virtual Group Group { get; set; }
        [NotMapped]
        [Display(Name = "File")]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase Upload { get; set; }
        [NotMapped]
        [Display(Name = "File size")]
        public string ReadableFileSize
        {
            get
            {
                string[] prefix = { "byte", "KiB", "MiB", "GiB" };
                var size = (double)FileSize;
                var p = 0;
                while (size >= 1024 && p < prefix.Length)
                {
                    p++;
                    size /= 1024;
                }
                var scale = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(size))) + 1);
                return String.Format("{0} {1}",scale * Math.Round(size / scale, 3), prefix[p]);
            }
        }
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