﻿using MarriageHall.DLL;
using MarriageHall.DTO;
using System.Data;
using System.Windows.Forms;

namespace MarriageHall.BLL
{
    public class BLLCategory
    {
        private static BLLCategory instance;
        public static BLLCategory Instance
        {
            get { if (instance == null) instance = new BLLCategory(); return instance; }
            private set { instance = value; }
        }

        public BLLCategory() { }

        public DataTable GetListCategory()
        {
            string query = "SELECT * FROM Categories";

            return DataProvider.Instance.GetDataTable(query);
        }

        public DataTable GetCategoryById(int id)
        {
            string query = $"SELECT * FROM Categories WHERE Id = {id}";

            return DataProvider.Instance.GetDataTable(query);
        }

        public bool InsertCategory(Category category)
        {
            string query = $"INSERT INTO Categories (Name) VALUES (N'{category.Name}')";

            return DataProvider.Instance.RunQuery(query);
        }

        public bool UpdateCategory(Category category)
        {
            string query = $"UPDATE Categories SET Name = N'{category.Name}' WHERE Id = {category.Id}";

            return DataProvider.Instance.RunQuery(query);
        }

        public bool DeleteCategory(int id)
        {
            string query = $"DELETE Categories WHERE Id = {id}";

            return DataProvider.Instance.RunQuery(query);
        }

        public bool Validate(Category category)
        {
            if (category.Name.Equals(""))
            {
                MessageBox.Show("Tên thể loại không được để trống", "Thông báo");
                return false;
            }
            return true;
        }
    }
}
