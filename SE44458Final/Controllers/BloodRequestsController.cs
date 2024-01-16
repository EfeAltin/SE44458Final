using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SE44458Final.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SE44458Final.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BloodRequestsController : ControllerBase
    {
        private readonly string connectionString;

        public BloodRequestsController(IConfiguration configuration)
        {
            connectionString = configuration["ConnectionStrings:SqlServerDb"] ?? "";
        }

        [HttpPost]
        public IActionResult RequestBlood(BloodRequestDto bloodRequestDto)
        {
            try
            {
                if (bloodRequestDto == null)
                {
                    ModelState.AddModelError("bloodRequestDto", "Invalid data");
                    return BadRequest(ModelState);
                }

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO BloodRequests " +
                                 "(Requester, BloodType, Email, City, Town, NumberOfUnits, DurationOfSearch, Reason) VALUES " +
                                 "(@Requester, @BloodType, @Email, @City, @Town, @NumberOfUnits, @DurationOfSearch, @Reason)";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Requester", bloodRequestDto.Requester);
                        command.Parameters.AddWithValue("@BloodType", bloodRequestDto.BloodType);
                        command.Parameters.AddWithValue("@Email", bloodRequestDto.Email);
                        command.Parameters.AddWithValue("@City", bloodRequestDto.City);
                        command.Parameters.AddWithValue("@Town", bloodRequestDto.Town);
                        command.Parameters.AddWithValue("@NumberOfUnits", bloodRequestDto.NumberOfUnits);
                        command.Parameters.AddWithValue("@DurationOfSearch", bloodRequestDto.DurationOfSearch);
                        command.Parameters.AddWithValue("@Reason", bloodRequestDto.Reason);

                        command.ExecuteNonQuery();
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Blood donation request", $"Error adding blood donation request: {ex.Message}");
                return BadRequest(ModelState);
            }
        }
    }
}
