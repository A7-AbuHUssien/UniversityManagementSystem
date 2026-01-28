using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniversityManagementSystem.Application.DTOs.Parameters;
using UniversityManagementSystem.Application.Interfaces.Services;

namespace UniversityManagementSystem.Api.Areas.Operation
{
    [Area("Operation")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class DisplayEnrollmentsController : ControllerBase
    {
        private readonly IDisplayEnrollmentService _displayEnrollmentService;

        public DisplayEnrollmentsController(IDisplayEnrollmentService displayEnrollmentService)
        {
            _displayEnrollmentService = displayEnrollmentService;
        }
        [HttpGet]
        public async Task<IActionResult>Display([FromQuery] EnrollmentFilterDto? filter)
        {
            var enrollments =await _displayEnrollmentService.GetEnrollmentsAsync(filter);
            return Ok(enrollments);
        }
    }
}
