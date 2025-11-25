using FarmaDiBusiness.DTOs;
using FarmaDiBusiness.DTOs.RolsDto;
using FarmaDiBusiness.DTOs.UsersDto;
using FarmaDiBusiness.Interfaces;
using FarmaDiCore.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FarmaDiApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUsersService _userService;
        public UserController(IUsersService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterUserRolesDto dto)
        {
            //El framework ya se encarga de validar el formato del dto
            var serviceResponse = await _userService.RegisterUserWithRolesAsync(dto);
            if (serviceResponse.IsSuccess)
            {
                var dataResponse = new UserRolResponseDto();

                dataResponse.UserName = serviceResponse.Data.Users.UserName;
                dataResponse.UserLastName = serviceResponse.Data.Users.UserLastName;
                dataResponse.Mail = serviceResponse.Data.Users.Mail;
                dataResponse.UserPhone = serviceResponse.Data.Users.UserPhone;

                dataResponse.Roles = (IEnumerable<string>)serviceResponse.Data!.Roles.Select(dt => new RolesResponseDto
                {
                    RolId = dt.Id,
                    RolName = dt.RolName
                    

                }).ToList();

                return CreatedAtAction(
                    nameof(GetById),
                    new { Id = dataResponse.Roles },
                    dataResponse

                    );
            }

            var unSuccessFullResponse = new UnsuccessfulResponseDto();

            switch (serviceResponse.MessageCode)
            {
                case MessageCodes.ErrorValidation:
                    unSuccessFullResponse.Code = "400";
                    unSuccessFullResponse.Message = "Ocurrio un error en la validacion de los datos";

                    return BadRequest(unSuccessFullResponse);

                default:
                    unSuccessFullResponse.Code = "500";
                    unSuccessFullResponse.Message = "Ocurrio un error en el servidor";
                    unSuccessFullResponse.Details = new { info = serviceResponse.Message};
                    return StatusCode(500, unSuccessFullResponse);

            }



        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute][Range(1, int.MaxValue)] int id)
        {
            var serviceResponse = await _userService.GetByIdAsync(id);
            if (serviceResponse.IsSuccess)
            {
                return Ok(serviceResponse.Data);
            }
            var unSuccessFullResponse = new UnsuccessfulResponseDto();
            switch (serviceResponse.MessageCode)
            {
                case MessageCodes.NotFound:
                    unSuccessFullResponse.Code = "404";
                    unSuccessFullResponse.Message = "Usuario no encontrado";
                    return NotFound(unSuccessFullResponse);
                default:
                    unSuccessFullResponse.Code = "500";
                    unSuccessFullResponse.Message = "Ocurrio un error en el servidor";
                    return StatusCode(500, unSuccessFullResponse);
            }
        }

        [HttpPost("assing-role")]
        public async Task<IActionResult> AssignRole([FromBody] UserRoleAssignmentDto assignmentDto)
        {
            var result = await _userService.AssignRoleToUserAsync(assignmentDto.UserId, assignmentDto.RoleId);

            if (result.IsSuccess)
            {
                return Ok(result);
            }
            var unSuccessFullResponse = new UnsuccessfulResponseDto();
            switch (result.MessageCode)
            {
                case MessageCodes.ErrorDataBase:
                    unSuccessFullResponse.Code = "200";
                    unSuccessFullResponse.Message = "El usuario ya tiene asignado el rol.";

                    return Ok(unSuccessFullResponse);
                case MessageCodes.NotFound:
                    unSuccessFullResponse.Code = "404";
                    unSuccessFullResponse.Message = "Usuario no encontrado";
                    return NotFound(unSuccessFullResponse);

                default:
                    unSuccessFullResponse.Code = "500";
                    unSuccessFullResponse.Message = "Ocurrio un error en el servidor";
                    return StatusCode(500, unSuccessFullResponse);
            }
        }
    }
}
