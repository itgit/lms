using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Web.Security;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;

namespace LMS.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        private string _firstName;
        private string _lastName;
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        [Display(Name = "First name")]
        public string FirstName { get { return _firstName; } set { _firstName = value.Trim(); } }
        [Display(Name = "Last name")]
        public string LastName { get { return _lastName; } set { _lastName = value.Trim(); } }
        public int? GroupId { get; set; }
        [ForeignKey("GroupId")]
        [Display(Name = "Group")]
        public virtual Group Group { get; set; }
        [NotMapped]
        [Display(Name = "Full name")]
        public string FullName
        {
            get
            {
                var names = new List<string>();

                
                if (!string.IsNullOrEmpty(FirstName))
                {
                    names.Add(FirstName);
                }
                if (!string.IsNullOrEmpty(LastName))
                {
                    names.Add(LastName);
                }
                return string.Join(" ", names);
            }
        }
        [NotMapped]
        public bool IsAdmin
        {
            get
            {
                using (var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext())))
                {
                    return roleManager.FindByName("admin").Users.Any(u => u.UserId == this.Id);
                }
            }
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<LMS.Models.Group> Groups { get; set; }

        public System.Data.Entity.DbSet<LMS.Models.Activity> Activities { get; set; }

        public System.Data.Entity.DbSet<LMS.Models.File> Files { get; set; }

        public System.Data.Entity.DbSet<LMS.Models.ActivityType> ActivityTypes { get; set; }

        public System.Data.Entity.DbSet<LMS.Models.Comment> Comments { get; set; }
    }
}