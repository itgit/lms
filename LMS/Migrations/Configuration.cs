namespace LMS.Migrations
{
    using LMS.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using ExtensionMethods;

    internal sealed class Configuration : DbMigrationsConfiguration<LMS.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "LMS.Models.ApplicationDbContext";
        }

        protected override void Seed(LMS.Models.ApplicationDbContext context)
        {
            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);
            var roleStore = new RoleStore<IdentityRole>(context);
            var roleManager = new RoleManager<IdentityRole>(roleStore);

            Random rnd = new Random();

            var firstnames = new List<string>()
            {
                // Förnamn, kvinnor
                "Maria",
                "Elisabeth",
                "Anna",
                "Kristina",
                "Margareta",
                "Eva",
                "Birgitta",
                "Karin",
                "Linnéa",
                "Marie",
                "Ingrid",
                "Marianne",
                "Sofia",
                "Kerstin",
                "Lena",
                "Helena",
                "Sara",
                "Inger",
                "Katarina",
                "Emma",
                "Johanna",
                "Viktoria",
                "Cecilia",
                "Monica",
                "Susanne",
                "Elin ",
                "Irene",
                "Anita",
                "Jenny",
                "Therese",
                "Ulla",
                "Ingegerd",
                "Carina",
                "Louise",
                "Hanna",
                "Viola",
                "Gunilla",
                "Ann",
                "Helen",
                "Linda",
                "Ida ",
                "Malin",
                "Annika",
                "Ulrika",
                "Matilda",
                "Josefin",
                "Elsa",
                "Barbro",
                "Anette",
                "Sofie",
                "Anneli",
                "Siv",
                "Astrid",
                "Caroline",
                "Kristin",
                "Åsa",
                "Emelie",
                "Karolina",
                "Julia",
                "Alice",
                "Lisa",
                "Britt",
                "Camilla",
                "Yvonne",
                "Madeleine",
                "Amanda ",
                "Agneta",
                "Rut",
                "Erika",
                "Gun",
                "Lovisa",
                "Charlotte",
                "Berit",
                "Sandra",
                "Rebecka",
                "Frida",
                "Inga",
                "Ann-Marie",
                "Ellinor",
                "Alexandra",
                "Maja",
                "Jessica",
                "Ingeborg",
                "Märta",
                "Charlotta",
                "Isabelle ",
                "Birgit",
                "Emilia",
                "Sonja",
                "Ellen",
                "Ebba",
                "Ann-Christin",
                "Maj",
                "Gunnel",
                "Klara",
                "Britta",
                "Pia",
                "Lisbeth",
                "Mona",
                "Solveig",

                // Förnamn, män
                "Karl",
                "Erik",
                "Lars",
                "Anders",
                "Per",
                "Mikael",
                "Johan",
                "Olof",
                "Nils ",
                "Jan",
                "Lennart",
                "Gustav",
                "Hans",
                "Peter",
                "Gunnar",
                "Sven",
                "Fredrik",
                "Thomas",
                "Bengt",
                "Bo",
                "Daniel",
                "Åke",
                "Göran",
                "Christer",
                "Oskar",
                "Alexander",
                "Andreas",
                "Stefan",
                "Magnus",
                "Martin",
                "Mats",
                "John",
                "Mattias",
                "Leif",
                "Henrik",
                "Ulf",
                "Bertil",
                "Arne",
                "Björn",
                "Jonas",
                "Ingemar",
                "Axel",
                "Robert",
                "Marcus",
                "Christian",
                "Stig",
                "Kjell",
                "Niklas",
                "David",
                "Viktor",
                "Rolf",
                "Håkan",
                "Emil",
                "Patrik",
                "Rickard",
                "Christoffer",
                "Joakim",
                "Tommy",
                "Wilhelm",
                "Roland",
                "Filip",
                "Mohamed",
                "William",
                "Ingvar",
                "Claes",
                "Roger",
                "Simon",
                "Sebastian",
                "Kent",
                "Anton",
                "Ove",
                "Kenneth",
                "Johannes",
                "Tobias",
                "Kurt",
                "Jörgen",
                "Rune",
                "Emanuel",
                "Jonathan",
                "Jakob",
                "Robin",
                "Gösta",
                "Hugo ",
                "Elias",
                "Georg",
                "Adam",
                "Lucas",
                "Johnny",
                "Sten",
                "Torbjörn",
                "Linus",
                "Börje",
                "Alf",
                "Dan",
                "Bernt",
                "Allan",
                "Arvid",
                "Albin",
                "Josef",
                "Ludvig",
            };

            var lastnames = new List<string>()
            {
                // Efternamn
                "Andersson",
                "Johansson",
                "Karlsson",
                "Nilsson",
                "Eriksson",
                "Larsson",
                "Olsson",
                "Persson",
                "Svensson",
                "Gustafsson",
                "Pettersson",
                "Jonsson",
                "Jansson",
                "Hansson",
                "Bengtsson",
                "Jönsson",
                "Lindberg",
                "Jakobsson",
                "Magnusson",
                "Olofsson",
                "Lindström",
                "Lindqvist",
                "Lindgren",
                "Axelsson",
                "Berg",
                "Bergström",
                "Lundberg",
                "Lundgren",
                "Lundqvist",
                "Lind",
                "Mattsson",
                "Berglund",
                "Fredriksson",
                "Sandberg",
                "Henriksson",
                "Forsberg",
                "Sjöberg",
                "Wallin",
                "Engström",
                "Danielsson",
                "Håkansson",
                "Eklund",
                "Lundin",
                "Gunnarsson",
                "Holm",
                "Björk",
                "Bergman",
                "Samuelsson",
                "Fransson",
                "Wikström",
                "Isaksson",
                "Bergqvist",
                "Arvidsson",
                "Nyström",
                "Holmberg",
                "Löfgren",
                "Söderberg",
                "Nyberg",
                "Claesson",
                "Blomqvist",
                "Mårtensson",
                "Nordström",
                "Lundström",
                "Eliasson",
                "Pålsson",
                "Björklund",
                "Viklund",
                "Berggren",
                "Sandström",
                "Lund",
                "Mohamed",
                "Nordin",
                "Ali",
                "Ström",
                "Åberg",
                "Hermansson",
                "Ekström",
                "Holmgren",
                "Sundberg",
                "Hedlund",
                "Dahlberg",
                "Hellström",
                "Sjögren",
                "Falk",
                "Abrahamsson",
                "Martinsson",
                "Öberg",
                "Blom",
                "Andreasson",
                "Ek",
                "Månsson",
                "Strömberg",
                "Åkesson",
                "Jonasson",
                "Hansen",
                "Norberg",
                "Åström",
                "Sundström",
                "Lindholm",
                "Holmqvist",
            };

            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            // role tilldelning

            while (context.Groups.Count() < 20)
            {
                context.Groups.Add(new Group() { Name = "Grupp " + (context.Groups.Count() + 1) });
                context.SaveChanges();
            }

            ApplicationUser user;

            while (context.Users.Count() < 400)
            {
                var firstname = firstnames[rnd.Next(firstnames.Count())];
                var lastname = lastnames[rnd.Next(lastnames.Count())];
                var username = "johndoe1";

                var n = 0;
                do
                {
                    n++;
                    username = string.Format("{0}{1}", firstname.RemoveDiacritics().ToLower(), n);
                } while (context.Users.Any(u => u.UserName == username));

                user = new ApplicationUser { UserName = username, Email = (firstname + "_" + lastname).RemoveDiacritics().ToLower() + n + "@skolan.se", FirstName = firstname, LastName = lastname, GroupId = (context.Groups.Find(rnd.Next(context.Groups.Count()) + 1) ?? context.Groups.FirstOrDefault()).Id };
                userManager.Create(user, firstname + n + "!");
                context.SaveChanges();
            }

            if (!roleManager.RoleExists("admin"))
            {
                var role = new IdentityRole("admin");
                roleManager.Create(role);
            }
            context.SaveChanges();

            if (!context.Users.Any(u => u.UserName == "admin"))
            {
                user = new ApplicationUser { UserName = "admin", Email = "admin@admin.com", FirstName = "Arvid", LastName = "Arvidsson" };
                userManager.Create(user, "Admin123!");
                userManager.AddToRole(user.Id, "admin");
            }
            context.SaveChanges();
        }
    }
}