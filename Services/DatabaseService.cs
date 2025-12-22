using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using DoublonManager.Models;

namespace DoublonManager.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString39C;
        private readonly string _connectionString19M;

        public DatabaseService(string conn39C, string conn19M)
        {
            _connectionString39C = conn39C;
            _connectionString19M = conn19M;
        }

        public async Task<List<Employee>> GetEmployeesFromSite(string site)
        {
            var employees = new List<Employee>();
            string connectionString = site == "39C" ? _connectionString39C : _connectionString19M;
            
            // Mock query for now since we don't have the actual DB
            // In a real scenario, we would use strict SQL queries
            string query = "SELECT ID, LastName, FirstName, Code, Num, LocDate FROM Employees"; 

            try 
            {
                // Note: Since we are in a dev environment without the actual DB, 
                // we will return mock data if connection fails or just simple mock data.
                // For the purpose of this implementation, I'll include the SQL code but wrapped 
                // to fallback to mock data if needed for testing UI.
                
                // UNCOMMENT THIS BLOCK FOR REAL DB
                /*
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            employees.Add(new Employee
                            {
                                ID = reader.GetInt32(0),
                                LastName = reader.GetString(1),
                                FirstName = reader.GetString(2),
                                Code = reader.GetString(3),
                                Num = reader.GetInt32(4),
                                LocDate = reader.IsDBNull(5) ? null : reader.GetDateTime(5),
                                Site = site
                            });
                        }
                    }
                }
                */

                // MOCK DATA for demonstration
                await Task.Delay(500); // Simulate network delay
                if (site == "39C")
                {
                    employees.Add(new Employee { ID = 1, LastName = "Dupont", FirstName = "Jean", Code = "A123", Num = 100, Site = "39C" });
                    employees.Add(new Employee { ID = 2, LastName = "Martin", FirstName = "Sophie", Code = "B456", Num = 101, Site = "39C" });
                    employees.Add(new Employee { ID = 3, LastName = "Durand", FirstName = "Pierre", Code = "C789", Num = 102, Site = "39C" });
                }
                else
                {
                    // Perfect duplicate
                    employees.Add(new Employee { ID = 101, LastName = "Dupont", FirstName = "Jean", Code = "A123", Num = 100, Site = "19M" });
                    // Duplicate with different number
                    employees.Add(new Employee { ID = 102, LastName = "Martin", FirstName = "Sophie", Code = "B456", Num = 999, Site = "19M" });
                    // Duplicate with different code
                    employees.Add(new Employee { ID = 103, LastName = "Durand", FirstName = "Pierre", Code = "XXXX", Num = 102, Site = "19M" });
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Error fetching data for {site}: {ex.Message}");
            }

            return employees;
        }

        public async Task<int> GetEmployeeCount(string site)
        {
            var employees = await GetEmployeesFromSite(site);
            return employees.Count;
        }

        public async Task DeleteEmployee(int employeeId, string site)
        {
             // Mock deletion
             await Task.Delay(200);
             Console.WriteLine($"Deleted employee {employeeId} from {site}");
        }
    }
}
