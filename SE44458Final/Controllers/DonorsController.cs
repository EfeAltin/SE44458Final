using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SE44458Final.Models;
using System;
using System.Data.SqlClient;

namespace SE44458Final.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonorsController : ControllerBase
    {
        private readonly string connectionString;
        private readonly IConfiguration _configuration;

        public DonorsController(IConfiguration configuration)
        {
            connectionString = configuration["ConnectionStrings:SqlServerDb"] ?? "";
            _configuration = configuration;
        }

        [HttpPost("CreateDonor")]
        public IActionResult CreateDonor([FromQuery] string branchName, [FromQuery] string branchPassword, [FromBody] DonorsDto donorsDto)
        {
            try
            {
                // Check if branchName and branchPassword are valid
                if (!ValidateBranchCredentials(branchName, branchPassword))
                {
                    return Unauthorized("Invalid branch credentials");
                }

                // Insert donor into the database
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO Donors " +
                        "(FullName, BloodType, Town, City, PhoneNo) VALUES " +
                        "(@FullName, @BloodType, @Town, @City, @PhoneNo)";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@FullName", donorsDto.FullName);
                        command.Parameters.AddWithValue("@BloodType", donorsDto.BloodType);
                        command.Parameters.AddWithValue("@Town", donorsDto.Town);
                        command.Parameters.AddWithValue("@City", donorsDto.City);
                        command.Parameters.AddWithValue("@PhoneNo", donorsDto.PhoneNo);

                        command.ExecuteNonQuery();
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Donor", $"Error creating donor: {ex.Message}");
                return BadRequest(ModelState);
            }
        }

        private bool ValidateBranchCredentials(string branchName, string branchPassword)
        {
            var validBranchName = _configuration.GetValue<string>($"Branches:{branchName}:Name");
            var validBranchPassword = _configuration.GetValue<string>($"Branches:{branchName}:Password");

            return branchName == validBranchName && branchPassword == validBranchPassword;
        }
    }
}
