using System;
using System.Linq;
using System.Threading.Tasks;
using ApiUser.Data;
using ApiUser.Models;
using ApiUser.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiUser.Controllers
{
    [ApiController]
	[Route("v1/user")]
    public class UserController : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get([FromServices]AppDbContext context)
        {
            var users = await context.Users.ToListAsync();
            users.ForEach(u => u.Password = "");
            return !users.Any() ? NotFound() : Ok(users);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Post([FromServices]AppDbContext context,
                                              [FromBody]User userModel)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {   
                userModel.Password = Criptografia.criptografarSenha(userModel.Password);

                await context.Users.AddAsync(userModel);
                await context.SaveChangesAsync();
                return Ok(userModel);
            }
            catch(Exception e)
            {
                return BadRequest();
            }
        }

        [HttpPost]
		[Route("login")]
        [AllowAnonymous]
		public async Task<ActionResult<dynamic>> AuthenticateAsync([FromServices]AppDbContext context,
                                                                   [FromServices] TokenService tokenService,
                                                                   [FromBody] User model)
		{
            model.Password = Criptografia.criptografarSenha(model.Password);

			var user = context.Users.Where(x => x.Username == model.Username &&
												x.Password == model.Password)
												.FirstOrDefault();

			if(user == null)
			{
				return NotFound(new {message = "Usuário ou senha inválidos."});
            }

            var token = tokenService.GenerateToken(user);
			user.Password = "";
			return new
			{
				user = user,
				token = token
			};
		}

        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = "employee,manager")]
        public string Put() => "Atualização de user";
        
        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = "manager")]
        public async Task<IActionResult> Delete([FromServices]AppDbContext context,
                                                 [FromRoute] int id)
        {
            var todo = await context.Users.FirstOrDefaultAsync(x => x.Id == id);
            
            try
            {   
                context.Users.Remove(todo);
                await context.SaveChangesAsync();
                return Ok();
            }
            catch(Exception e)
            {
                return BadRequest();
            }
        }
    }
}