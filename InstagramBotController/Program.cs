using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramBotController
{
    class Program
    {
        public static readonly string INSTAGRAMAPIPATH = @"C:\Users\rfur\source\repos\scratch\InstagramBotController\InstagramBotController\Resources\InstagramAPI\Execute\";

        public static readonly string PICTURESPATH = @"D:\picture";

        public static readonly string PROFILEPICTUREPATH = @"D:\profilpicture";

        public static List<string> PROFILEPICTURES = Directory.GetFiles(PROFILEPICTUREPATH).ToList();

        public static List<string> PICTURES = Directory.GetFiles(PICTURESPATH).ToList();

        static void Main(string[] args)
        {
            InstagramBotController controller = new InstagramBotController();
            
            controller.DeleteAll();

            Person person = new Person()
            {
                firstName = "Robin",
                lastName = "Furrer",
                username = "robfur123",
                password = "Hurensohn123"
            };

            Account acc = new Account()
            {
                Person1 = person
            };

            controller.CreateAccount(acc);

            foreach (Account account in controller.GetAccounts())
            {
                Console.WriteLine(controller.ForceLogin(account));
                controller.uploadprofileImage();
                Console.WriteLine(account.id + " " + account.userid + " " + account.Person1.username + " " + account.Person1.password);
            }
        }
    }
}
