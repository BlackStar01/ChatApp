using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Communication
{
    
    public interface Message
    {
        string ToString();
    }

    [Serializable]
    public class Expr : Message
    {
        private double op1, op2;
        private char op;

        public Expr(double op1, double op2, char op)
        {
            this.op1 = op1;
            this.op2 = op2;
            this.op = op;
        }


        public double Op1
        {
            get { return op1; }
        }

        public double Op2
        {
            get { return op2; }
        }

        public char Op
        {
            get { return op; }
        }

        public override string ToString()
        {
            return op1 + " " + op + " " + op2;
        }

    }

    [Serializable]
    public class Result : Message
    {
        private double val;
        private bool error;

        public Result(double val, bool err)
        {
            this.val = val;
            error = err;
        }

        public double Val
        {
            get { return val; }
        }

        public bool Error
        {
            get { return error; }
        }

        public override string ToString()
        {
            return val + " ";
        }

    }

    [Serializable]
    public class UserProfile
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public UserProfile(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public override string ToString()
        {
            return Username;
        }
    }

    public static class UserProfileManager
    {
        private const string ProfileFilePath = "user_profiles.dat";

        public static List<UserProfile> UserProfiles { get; private set; }

        static UserProfileManager()
        {
            UserProfiles = new List<UserProfile>();
            LoadProfiles();
        }

        public static void AddUserProfile(UserProfile profile)
        {
            UserProfiles.Add(profile);
            SaveProfiles();
        }

        public static bool CheckCredentials(string username, string password)
        {
            return UserProfiles.Exists(profile => profile.Username == username && profile.Password == password);
        }

        private static void LoadProfiles()
        {
            if (File.Exists(ProfileFilePath))
            {
                using (FileStream fs = new FileStream(ProfileFilePath, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    UserProfiles = (List<UserProfile>)bf.Deserialize(fs);
                }
            }
        }

        private static void SaveProfiles()
        {
            using (FileStream fs = new FileStream(ProfileFilePath, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, UserProfiles);
            }
        }
    }

}
