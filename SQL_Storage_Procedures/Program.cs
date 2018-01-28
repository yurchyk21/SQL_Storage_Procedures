using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Bogus;

namespace SQL_Storage_Procedures
{
    class Program
    {


        class Employee
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Gender { get; set; }

        }
        static DataTable GetEmloyeesDataTable(List<Employee> list)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Id");
            dataTable.Columns.Add("Name");
            dataTable.Columns.Add("Gander");

            foreach (var item in list)
            {
                dataTable.Rows.Add(item.Id, item.Name, item.Gender);
            }
            return dataTable;
        }

        static void BogusInsert(int quantity)
        {
            var Faker = new Faker<Employee>()
            .RuleFor(o => o.Name, f => f.Person.FirstName.ToString())
            .RuleFor(o => o.Id, f => f.IndexGlobal)
            .RuleFor(o => o.Gender, f => f.Person.Gender.ToString());

            List<Employee> list = new List<Employee>();

            for (int i = 0; i < quantity; i++)
            {
               list.Add(Faker.Generate());
            };
            Insert(list);
        }

        static void Insert(List<Employee> list)
        {


            using (SqlConnection con = new SqlConnection(ConfigurationManager.
                ConnectionStrings["MyConnection"].ToString()))
            {
                SqlCommand command = new SqlCommand("[dbo].[spInsertEmployees]", con);
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter sqlParameter = new SqlParameter()
                {
                    ParameterName = "@InputEmployees",
                    Value = GetEmloyeesDataTable(list)
                };
                command.Parameters.Add(sqlParameter);
                con.Open();
                command.ExecuteNonQuery();
                con.Close();
            }
        }

        static void Delete(int id)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.
                ConnectionStrings["MyConnection"].ToString()))
            {
                SqlCommand command = new SqlCommand("[dbo].[spDelEmployees]", con);
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter sqlParameter = new SqlParameter()
                {
                    ParameterName = "@DelId",
                    Value = id
                };
                command.Parameters.Add(sqlParameter);
                con.Open();
                command.ExecuteNonQuery();
                con.Close();
            }
        }

        static void Find(string name)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.
                ConnectionStrings["MyConnection"].ToString()))
            {
                SqlCommand command = new SqlCommand("[dbo].[spFindEmployees]", con);
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter sqlParameter = new SqlParameter()
                {
                    ParameterName = "@Name",
                    Value = name
                };
                command.Parameters.Add(sqlParameter);
                con.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Console.WriteLine($"Id {reader["Id"]},\t Name: {reader["Name"]},\t\t Gender {reader["Gender"]}");
                }
                con.Close();
            }
        }

        static void Update()
        {
            int _id;
            string _name, _gender;
            Console.Write("Enter Id: ");
            try
            {
               _id = int.Parse(Console.ReadLine());
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return;
            }
            Console.Write("Enter Name: ");
            _name = Console.ReadLine();
            Console.Write("Enter Gender: ");
            _gender = Console.ReadLine();

            using (SqlConnection con = new SqlConnection(ConfigurationManager.
                ConnectionStrings["MyConnection"].ToString()))
            {
                SqlCommand command = new SqlCommand("[dbo].[spChangeOneEmployee]", con);
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter[] sqlParameter = new SqlParameter[]
                {   new SqlParameter("@Id", _id),
                    new SqlParameter("@Name", _name),
                    new SqlParameter("@Gender", _gender)
                };
                command.Parameters.AddRange(sqlParameter);
                con.Open();
                command.ExecuteNonQuery();
                con.Close();
            }
        }
        static void Main(string[] args)
        {
            //BogusInsert(500);
            Find("Yura");
            Update();
            Find("Yura");
            Console.ReadKey();

        }
    }
}
