using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace RustBot.Permissions
{
    class PermissionManager
    {
        static List<Admin> admins = Utilities.FillList<Admin>("Users/admins.json");
        public static Task<bool> AddAdmin(ulong id, string name, int permLevel)
        {
            return Task.Run( () =>
            {
                if (CheckAdmin(id).Result) { return false; }

                Admin a = new Admin { id = id.ToString(), name = name, permLevel = permLevel };
                Utilities.WriteToJsonFile<Admin>("Users/admins.json", a, true);
                admins.Add(a);

                return true;
            });
        }

        public static Task<bool> RemoveAdmin(string id)
        {
            return Task.Run(() =>
            {
                int count = 0;
                if (id == "") { return false; }
                else
                {
                    string[] admins = File.ReadAllLines("Users/admins.json");

                    foreach (var admin in admins)
                    {
                        count++;
                        Admin a = Utilities.ReadFromJsonFile<Admin>(admin);

                        if (a.id == id)
                        {
                            var file = new List<string>(System.IO.File.ReadAllLines("Users/admins.json"));
                            file.RemoveAt(count - 1);
                            File.WriteAllLines("Users/admins.json", file.ToArray());
                            return true;
                        }
                    }

                    return false;
                }
            });
        }

        public static Task ReloadPermissions()
        {
            admins.Clear();
            admins = Utilities.FillList<Admin>("Users/admins.json");
            return Task.CompletedTask;
        }


        public static Task<bool> CheckAdmin(ulong id)
        {
            return Task.Run(() =>
            {
                foreach (Admin admin in admins)
                {

                    if (admin.id == id.ToString()) { return true; }
                }
                return false;
            });
        }

        public static int GetPerms(ulong id)
        {
            foreach (Admin admin in admins)
            {
                if (admin.id == id.ToString()) 
                {
                    return admin.permLevel;
                }
            }
            return -1;
        }

    }

    public class Admin
    {
        public string id { get; set; }
        public string name { get; set; }
        public int permLevel { get; set; }
    }

    public class PermissionConfig
    {
        public static int User = -1;
        public static int Admin = 100;
    }
}
