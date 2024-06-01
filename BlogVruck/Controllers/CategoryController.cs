using Blog.Data;
using Blog.Models;
using BlogVruck.Extensions;
using BlogVruck.ViewModels;
using BlogVruck.ViewModels.Categories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogVruck.Controllers
{
    [ApiController]
    public class CategoryController : ControllerBase
    {
        //APIREST = PARAMETRIZACAO
        [Authorize(Roles = "user")]
        [HttpGet("v1/categories")]
        public async Task<IActionResult> GetAsync([FromServices] BlogDataContext context)
        {
            try
            {
                var categories = await context.Categories.ToListAsync();
                return Ok(new ResultViewModel<List<Category>>(categories));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<List<Category>>("05XE03 - Falha interna no servidor"));
            }
        }

        [HttpGet("v1/categories/{id:int}")]
        public async Task<IActionResult> GetByIdAsync([FromServices] BlogDataContext context, [FromRoute] int id)
        {
            try
            {
                var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);

                if (category == null)
                {
                    return NotFound(new ResultViewModel<Category>("Conteudo não encontrado"));
                }

                return Ok(new ResultViewModel<Category>(category));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<Category>("05XE04 - Falha interna no servidor"));
            }

        }

        [Authorize(Roles = "user")]
        [HttpPost("v1/categories/")]
        public async Task<IActionResult> PostAsync([FromServices] BlogDataContext context, [FromBody] EditorCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResultViewModel<Category>(ModelState.GetErrors()));
            }

            try
            {
                var category = new Category
                {
                    Id = 0,
                    Name = model.Name,
                    Slug = model.Slug,
                };

                await context.Categories.AddAsync(category);
                await context.SaveChangesAsync();
                return Created($"v1/categories/{category.Id}", new ResultViewModel<Category>(category));
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new ResultViewModel<Category>("05XE9 - Não foi possivel incluir a categoria"));
            }
            catch (Exception)
            {
                return StatusCode(500, new ResultViewModel<Category>("05XE10 - Falha interna no servidor"));
            }
        }

        [Authorize(Roles = "user")]
        [HttpPut("v1/categories/{id}")]
        public async Task<IActionResult> PutAsync([FromServices] BlogDataContext context, [FromRoute] int id, [FromBody] EditorCategoryViewModel model)
        {
            try
            {
                var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
                if (category == null)
                {
                    return NotFound(new ResultViewModel<Category>("Conteudo não encontrado"));
                }

                category.Name = model.Name;
                category.Slug = model.Slug;

                context.Categories.Update(category);
                await context.SaveChangesAsync();
                return Ok(new ResultViewModel<Category>(category));
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new ResultViewModel<Category>("05XE7 - Não foi possivel alterar a categoria"));
            }
            catch (Exception)
            {
                return StatusCode(500, new ResultViewModel<Category>("05XE12 - Falha interna no servidor"));
            }

        }

        [Authorize(Roles = "user")]
        [HttpDelete("v1/categories/{id}")]
        public async Task<IActionResult> DeleteAsync([FromServices] BlogDataContext context, [FromRoute] int id)
        {
            try
            {
                var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
                if (category == null)
                {
                    return NotFound(new ResultViewModel<Category>("Conteudo não encontrado"));
                }

                context.Categories.Remove(category);
                await context.SaveChangesAsync();
                return Ok(new ResultViewModel<Category>(category));
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new ResultViewModel<Category>("05XE8 - Não foi possivel deletar a categoria"));
            }
            catch (Exception)
            {
                return StatusCode(500, new ResultViewModel<Category>("05XE11 - Falha interna no servidor"));
            }
        }

        [HttpGet("v2/categories")]
        public IActionResult Get2([FromServices] BlogDataContext context)
        {
            return Ok(context.Categories.ToList());
        }
    }
}
