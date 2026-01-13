using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.Data.SqlClient;
using Metavystic.Models;

namespace Metavystic.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly string? _connectionString;

        public EmployeeController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // GET: api/employee
        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            var employees = new List<Employee>();

            try
            {
                using var con = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_EmployeesCRUD", con);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@Action", SqlDbType.NVarChar, 10).Value = "SELECT";

                await con.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    employees.Add(new Employee
                    {
                        EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        Email = reader.GetString(reader.GetOrdinal("Email")),
                        Department = reader.GetString(reader.GetOrdinal("Department")),
                        Salary = reader.GetDecimal(reader.GetOrdinal("Salary")),
                        AnnualSalary = reader.GetDecimal(reader.GetOrdinal("AnnualSalary"))
                    });
                }

                return Ok(employees);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST: api/employee
        [HttpPost]
        public async Task<IActionResult> Insert(Employee emp)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                using var con = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_EmployeesCRUD", con);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@Action", SqlDbType.NVarChar, 10).Value = "INSERT";
                cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = emp.Name;
                cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = emp.Email;
                cmd.Parameters.Add("@Salary", SqlDbType.Decimal).Value = emp.Salary;
                cmd.Parameters.Add("@Department", SqlDbType.NVarChar, 50).Value = emp.Department;

                await con.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                return StatusCode(201, "Employee created successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // PUT: api/employee/id
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Employee emp)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid employee data");

            if (id <= 0)
                return BadRequest("Invalid Employee ID");


            emp.EmployeeId = id;

            try
            {
                using var con = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_EmployeesCRUD", con);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@Action", SqlDbType.NVarChar, 10).Value = "UPDATE";
                cmd.Parameters.Add("@EmployeeId", SqlDbType.Int).Value = emp.EmployeeId;
                cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = emp.Name;
                cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = emp.Email;
                cmd.Parameters.Add("@Salary", SqlDbType.Decimal).Value = emp.Salary;
                cmd.Parameters.Add("@Department", SqlDbType.NVarChar, 50).Value = emp.Department;

                await con.OpenAsync();
                var rowsAffected = await cmd.ExecuteNonQueryAsync();

                if (rowsAffected == 0)
                    return NotFound($"Employee with ID {id} not found");

                return Ok("Employee updated successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        // DELETE: api/employee/id
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid ID");

            try
            {
                using var con = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_EmployeesCRUD", con);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@Action", SqlDbType.NVarChar, 10).Value = "DELETE";
                cmd.Parameters.Add("@EmployeeId", SqlDbType.Int).Value = id;

                await con.OpenAsync();
                var rows = await cmd.ExecuteNonQueryAsync();

                if (rows == 0)
                    return NotFound("Employee not found");

                return Ok("Employee deleted successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        
        }


    }
