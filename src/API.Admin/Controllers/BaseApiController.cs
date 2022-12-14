using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using WebApi.Domain.CQRS.CommandHandling;
using WebApi.Domain.CQRS.QueryHandling;

namespace API.Admin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
        private IMediator _mediator;

        public BaseApiController() { }

        protected BaseApiController(IMediator mediator)
        {
            _mediator = mediator;
        }
        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>()!;


        protected async new Task<IActionResult> Response<TResult>(IQuery<TResult> query)
        {
            TResult result = default!;

            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                result = await _mediator.Send(query);
            }
            catch (Exception e)
            {
                return BadRequestActionResult(e.Message);
            }

            return Ok(new
            {
                data = result,
                success = true
            });
        }

        protected async new Task<IActionResult> Response(ICommand command, object data = null)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                await _mediator.Send(command);
            }
            catch (Exception e)
            {
                return BadRequestActionResult(e.Message);
            }

            return Ok(new
            {
                data = data,
                success = true
            });
        }

        private IActionResult BadRequestActionResult(dynamic resultErrors)
        {
            return BadRequest(new
            {
                success = false,
                message = resultErrors
            });
        }
    }
}