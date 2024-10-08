﻿using MarriageHall.DLL;
using MarriageHall.DTO;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MarriageHall.BLL
{
    public class BLLAccount
    {
        private static BLLAccount instance;
        public static BLLAccount Instance
        {
            get { if (instance == null) instance = new BLLAccount(); return instance; }
            private set { instance = value; }
        }

        public BLLAccount() { }

        public DataTable GetListAccount()
        {
            string query = "SELECT Id, UserName, Name, Gender, Phone, Permission FROM Accounts";

            return DataProvider.Instance.GetDataTable(query);
        }

        public Account GetAccountByBookingId(int bookingId)
        {
            string query = $"SELECT a.* FROM Accounts AS a JOIN Bookings AS b ON a.Id = b.StaffId AND b.Id = {bookingId}";

            DataTable data = DataProvider.Instance.GetDataTable(query);

            foreach (DataRow row in data.Rows)
            {
                return new Account(row);
            }

            return null;
        }

        public Account GetAccountByUserName(string userName)
        {
            string query = $"SELECT Id, UserName, Name, Gender, Phone, Permission FROM Accounts WHERE UserName = '{userName}'";

            DataTable data = DataProvider.Instance.GetDataTable(query);

            foreach (DataRow row in data.Rows)
            {
                return new Account(row);
            }

            return null;
        }

        public DataTable SearchAccountByNameAndPhone(string search)
        {
            string query = $"SELECT Id, UserName, Name, Gender, Phone, Permission FROM Accounts WHERE Name LIKE N'%{search}%' OR Phone LIKE '%{search}%'";

            return DataProvider.Instance.GetDataTable(query);
        }

        public bool Login(string userName, string password)
        {
            string query = $"SELECT PasswordHash, PasswordSalt FROM Accounts WHERE UserName = '{userName}'";
            DataTable dt = DataProvider.Instance.GetDataTable(query);
            if (dt.Rows.Count == 0)
            {
                return false;
            }
            DataRow user = dt.Rows[0];
            byte[] passwordHash = (byte[])user["PasswordHash"];
            byte[] passwordSalt = (byte[])user["PasswordSalt"];

            return VerifyPasswordHash(password, passwordHash, passwordSalt);
        }

        public bool InsertAccount(Account account)
        {
            CreatePasswordHash(account.Password, out byte[] passwordHash, out byte[] passwordSalt);
            string query = $"INSERT INTO Accounts (Name, Phone, Gender, UserName, Permission,  PasswordHash, PasswordSalt) VALUES (N'{account.Name}', '{account.Phone}', {(int)account.Gender}, '{account.UserName}', {(int)account.Permission}, @PasswordHash, @PasswordSalt)";

            return DataProvider.Instance.RunQuery(query, new object[] { passwordHash, passwordSalt});
        }

        public bool UpdateAccount(Account account)
        {
            string query = $"UPDATE Accounts SET Name = N'{account.Name}', Phone = '{account.Phone}', Gender = {(int)account.Gender}, Permission = {(int)account.Permission} WHERE Id = {account.Id}";

            return DataProvider.Instance.RunQuery(query);
        }

        public bool ChangePassword(int id, string newPassword)
        {
            CreatePasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);
            string query = $"UPDATE Accounts SET PasswordHash = @PasswordHash, PasswordSalt = @PasswordSalt WHERE Id = {id}";

            return DataProvider.Instance.RunQuery(query, new object[] { passwordHash, passwordSalt });
        }

        public bool DeleteAccount(int id)
        {
            string query = $"DELETE Accounts WHERE Id = {id}";

            return DataProvider.Instance.RunQuery(query);
        }

        public bool Validate(Account account)
        {
            if (account.UserName.Equals(""))
            {
                MessageBox.Show("Tên đăng nhập không được để trống", "Thông báo");
                return false;
            }
            if (account.Name.Equals(""))
            {
                MessageBox.Show("Tên tài khoản không được để trống", "Thông báo");
                return false;
            }
            if (!Regex.IsMatch(account.Phone, @"^0\d{9,10}$"))
            {
                MessageBox.Show("Số điện thoạt không hợp lệ", "Thông báo");
                return false;
            }
            return true;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac
                    .ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac
                    .ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                return computedHash.SequenceEqual(passwordHash);
            }
        }
    }
}
