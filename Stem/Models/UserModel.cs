using System.Collections.Generic;
using Stem.Databases;

namespace Stem.Models
{
    public class UserModel
    {
        public string Email { get; set; }
        public string DbPassword { get; }

        public void Authenticate(string email, string password)
        {
            
        }

        public void ChangeEmail()
        {
            
        }
        
        public void ChangePassword()
        {
            
        }
        
        public void Create(string password)
        {
            using var sqlCient = new SqlClient(true);
            var data = new Dictionary<string, dynamic> {{"email", Email}};
            sqlCient.Insert("account", data);
        }
    }
}
