using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SE44458Final.Models;
using System.Data.SqlClient;

namespace SE44458Final.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BloodToBankController : ControllerBase
    {
        private readonly string connectionString;
        private readonly IConfiguration _configuration;

        public BloodToBankController(IConfiguration configuration)
        {
            connectionString = configuration["ConnectionStrings:SqlServerDb"]?? "";
            _configuration = configuration;
        }


        [HttpPost]
        public IActionResult AddBloodToBank([FromQuery] string branchName, [FromQuery] string branchPassword, BloodBankDto bloodBankDto)
        {
            if (!ValidateBranchCredentials(branchName, branchPassword))
            {
                return Unauthorized("Invalid branch credentials");
            }

            try
            {
                if (bloodBankDto == null)
                {
                    ModelState.AddModelError("bloodBankDto", "Invalid data");
                    return BadRequest(ModelState);
                }

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO BloodBank " +
             "(DonorName, BloodType, DonationDate, Units) VALUES " +
             "(@DonorName, @BloodType, @DonationDate, @Units)";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@DonorName", bloodBankDto.DonorName);
                        command.Parameters.AddWithValue("@BloodType", bloodBankDto.BloodType);
                        command.Parameters.AddWithValue("@DonationDate", bloodBankDto.DonationDate);
                        command.Parameters.AddWithValue("@Units", bloodBankDto.Units);
                        

                        command.ExecuteNonQuery();
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Blood to bank", $"Error adding blood to bank: {ex.Message}");
                return BadRequest(ModelState);
            }
        }

        private bool ValidateBranchCredentials(string branchName, string branchPassword)
        {
            var validBranchName = _configuration.GetValue<string>($"Branches:{branchName}:Name");
            var validBranchPassword = _configuration.GetValue<string>($"Branches:{branchName}:Password");

            return branchName == validBranchName && branchPassword == validBranchPassword;
        }


        [HttpGet("GetBloodTypeQuantities")]
        public IActionResult GetBloodTypeQuantities()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT BloodType, SUM(CAST(Units AS INT)) AS TotalUnits FROM BloodBank GROUP BY BloodType";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            var result = new List<BloodTypeQuantityDto>();

                            while (reader.Read())
                            {
                                var bloodTypeQuantity = new BloodTypeQuantityDto
                                {
                                    BloodType = reader["BloodType"].ToString(),
                                    TotalQuantity = Convert.ToInt32(reader["TotalUnits"])
                                };

                                result.Add(bloodTypeQuantity);
                            }

                            return Ok(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Blood type quantities", $"Error retrieving blood type quantities: {ex.Message}");
                return BadRequest(ModelState);
            }
        }


    }
}
