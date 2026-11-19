using Microsoft.AspNetCore.Mvc;
using SecurityFootage.Domain;
using SecurityFootage.Domain.Entities;
using SecurityFootage.Domain.Interfaces;
using SecurityFootage.Api.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace SecurityFootage.Api.Controllers
{
    [ApiController]
    [Route("footageaccess")]
    public class FootageAccessController : ControllerBase
    {
        private readonly IFootageAccessService _footageService;

        public FootageAccessController(IFootageAccessService footageAccessService)
        {
            _footageService = footageAccessService;
        }

        [HttpPost("retrieve")]
        public async Task<ActionResult<string>?> Retrieve([FromBody] FootageRequestDto dto,
                                                  [FromHeader(Name = "NickName")] string nickName,
                                                  CancellationToken ct)
        {
            var location = new Location
            {
                Country = dto.Country,
                City = dto.City
            };
           
            var result = await _footageService.RequestFootageAccessAsync(location, nickName, ct);

           if (result.Result == RequestResult.InvalidUser)
            {
                return Unauthorized();
            }
            if (result.Result == RequestResult.InvalidAccess)
            {
                return StatusCode(StatusCodes.Status403Forbidden, result.Result);
            }
            if (result.Result == RequestResult.FootageNotFound)
            {
                return NotFound();
            }
            if (result.Result == RequestResult.SasGenerationFailed)
            {
                return StatusCode(StatusCodes.Status502BadGateway, "Failed to generate SAS link");
            }
            if (result.Result == RequestResult.Success)
            {
                return result.SasUrl!;
            }

            return StatusCode(StatusCodes.Status500InternalServerError, "Unexpected error");
        }

        [HttpPost("create")]
        public async Task<ActionResult<UserCreatedDto>> Create([FromBody] UserCreateDto dto, 
                                                               [FromHeader(Name = "NickName")] string nickName, 
                                                               CancellationToken ct)
        {
            var user = new User
            {
                Nickname = dto.NickName
            };

            var result = await _footageService.CreateUserAsync(user, nickName, ct);

            if (result.Result == RequestResult.InvalidUser)
            {
                return Unauthorized();
            }
            if (result.Result == RequestResult.Conflict)
            {
                return Conflict(new
                {
                    message = "A user with this nickname already exist!"
                });
            }
            if (result.Result != RequestResult.Success || result.User == null)
            {
                return Problem(statusCode: StatusCodes.Status500InternalServerError,
                               title: "Unexpected user creation result");
            }
            
            var response = new UserCreatedDto(result.User.Id, result.User.Nickname, result.User.CreatedAtUtc);

            return StatusCode(StatusCodes.Status201Created, response);
        }
    }
}