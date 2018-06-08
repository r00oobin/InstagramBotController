using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramBotController
{
    class InstagramBotController
    {
        private static InstagramBotControllerEntities1 dbContext = new InstagramBotControllerEntities1();

        private static Account currentUser;

        private static bool isLogedIn = false;

        public virtual bool CreateAccount(Account account, bool uploadProfilePicture = true, bool uploadImages = true){

            dbContext.People.Add(account.Person1);
            dbContext.Accounts.Add(account);
            dbContext.SaveChanges();

            ForceLogin(account);
            if (uploadProfilePicture)
                this.uploadprofileImage();
            if(uploadImages)
                this.uploadImages();

            LogOut();

            return true;
        }

        public virtual List<Account> GetAccounts()
        {
            return dbContext.Accounts.Select(r => r)
                .ToList();
        }

        public virtual void DeleteAll()
        {
            dbContext.Accounts.RemoveRange(dbContext.Accounts.Select(r => r));
            dbContext.SaveChanges();
            dbContext.People.RemoveRange(dbContext.People.Select(r => r));
            dbContext.SaveChanges();
        }

        public virtual Account GetAccount(string username)
        {
            return  dbContext.Accounts.Select(r => r)
                .Where(r => r.Person1.username == username)
                .First();
        }

        public virtual bool Login(Account account)
        {
            if (isLogedIn)
            {
                this.LogOut();
            }

            currentUser = account;

            isLogedIn = this.run_cmd(Executes.Login, account.Person1.username + " " + account.Person1.password).Equals("True");
            
            if (!isLogedIn)
            {
                currentUser = null;
            }

            if(currentUser != null &&currentUser.userid == null)
            {
                currentUser.userid = this.getUsernameId();
                
                dbContext.SaveChanges();
            }

            return isLogedIn;
        }

        public virtual bool ForceLogin(Account account)
        {
            currentUser = account;

            isLogedIn = true;

            return isLogedIn;
        }

        public virtual void LogOut()
        {
            if (isLogedIn)
            {
                currentUser = null;
            }
            currentUser = null;
            isLogedIn = !isLogedIn;
        }
        public virtual void uploadprofileImage()
        {

            Random random = new Random();
            int randomNumber = random.Next(0, Program.PROFILEPICTURES.Count);

            string imgPath = Program.PROFILEPICTURES[randomNumber];

            if (isLogedIn)
            {
                this.run_cmd(Executes.UploadProfilePicture, imgPath);
            }
            else
            {
                throw new NotLoggedInException();
            }
        }

        public virtual void uploadprofileImage(string imgPath)
        {
            if (isLogedIn)
            {
                this.run_cmd(Executes.UploadProfilePicture, imgPath);
            }
            else
            {
                throw new NotLoggedInException();
            }
        }

        public virtual void uploadImage(string img)
        {
            if (isLogedIn)
            {
                this.run_cmd(Executes.UploadPicture, img);
            }
            else
            {
                throw new NotLoggedInException();
            }
        }

        public virtual void uploadImages()
        {
            if (isLogedIn)
            {
                List<string> imgs = new List<string>();

                Random random = new Random();
                int randomNumber = random.Next(4, 12);
                
                for(int i = 0; i < randomNumber; i++)
                {
                    int randomNumber2 = random.Next(0, Program.PICTURES.Count);

                    uploadImage(Program.PICTURES[randomNumber2]);
                }
            }
            else
            {
                throw new NotLoggedInException();
            }

        }

        public virtual void uploadImages(List<string> imgs)
        {
            if (isLogedIn)
            {
                foreach (string img in imgs)
                {
                    uploadImage(img);
                }
            }
            else
            {
                throw new NotLoggedInException();
            }
            
        }

        public virtual void followUser(string usernameId)
        {
            if (isLogedIn)
            {
                this.run_cmd(Executes.Follow, usernameId);
            }
            else
            {
                throw new NotLoggedInException();
            }
        }

        public virtual void unFollowUser(string usernameId)
        {
            if (isLogedIn)
            {
                this.run_cmd(Executes.UnFollow, usernameId);
            }
            else
            {
                throw new NotLoggedInException();
            }
        }

        public string getUsernameId()
        {
            return run_cmd(Executes.GetUsernameId, null);
        }

        public virtual Account searchAccount(string username)
        {
            throw new NotImplementedException();
        }

        public virtual bool ifUsernameExist(string username)
        {
            return this.run_cmd(Executes.IfUsernameExist, username).Equals("True");
        }

        private string run_cmd(Executes execute, string args)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = @"C:\Users\rfur\AppData\Local\Programs\Python\Python36-32\python.exe";

            if(args == null)
                args = "";

            args = currentUser.Person1.username + " " + currentUser.Person1.password + " " + args;

            switch (execute)
            {
                case Executes.IfUsernameExist:
                    start.Arguments = string.Format("{0} {1}", Program.INSTAGRAMAPIPATH + "ifUsernameExist.py", args);
                    break;
                case Executes.SearchUser:
                    throw new NotImplementedException();
                case Executes.GetUsernameId:
                    start.Arguments = string.Format("{0} {1}", Program.INSTAGRAMAPIPATH + "getUserId.py", args);
                    break;
                case Executes.Follow:
                    start.Arguments = string.Format("{0} {1}", Program.INSTAGRAMAPIPATH + "follow.py", args);
                    break;
                case Executes.Login:
                    start.Arguments = string.Format("{0} {1}", Program.INSTAGRAMAPIPATH + "login.py", args);
                    break;
                case Executes.UnFollow:
                    start.Arguments = string.Format("{0} {1}", Program.INSTAGRAMAPIPATH + "unfollow.py", args);
                    break;
                case Executes.UploadProfilePicture:
                    start.Arguments = string.Format("{0} {1}", Program.INSTAGRAMAPIPATH + "updateProfilePicture.py", args);
                    break;
                case Executes.UploadPicture:
                    start.Arguments = string.Format("{0} {1}", Program.INSTAGRAMAPIPATH + "updatePicture.py", args);
                    break;
                default:
                    throw new NotImplementedException();
            }
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            
            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    return(reader.ReadLine());
                }
            }
        }
    }

    enum Executes
    {
        IfUsernameExist,
        SearchUser,
        GetUsernameId,
        Follow,
        UnFollow,
        UploadProfilePicture,
        UploadPicture,
        Login
    }
}
