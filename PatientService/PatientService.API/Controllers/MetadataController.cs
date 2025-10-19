using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientService.API.Models;
using PatientService.Infrastructure.Data;

namespace PatientService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "RequireAdmin")]
public class MetadataController : ControllerBase
{
    private readonly PatientDbContext _db;
    private readonly IMapper _mapper;

    public MetadataController(PatientDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    // GET api/metadata/gender
    [HttpGet("{table}")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<MetadataDto>>> List(string table)
    {
        var entityType = _db.Model.GetEntityTypes().FirstOrDefault(t => t.ClrType.Name.Equals(table, StringComparison.OrdinalIgnoreCase));
        if (entityType == null)
            return NotFound("Unknown metadata table.");

        var setMethod = typeof(DbContext).GetMethod(nameof(DbContext.Set), Type.EmptyTypes)!.MakeGenericMethod(entityType.ClrType);
        var set = setMethod.Invoke(_db, null) as IQueryable<object>;
        var data = await set!.ToListAsync();
        var result = data.Select(d => _mapper.Map<MetadataDto>(d)).OrderBy(m => m.SortOrder);
        return Ok(result);
    }

    // POST/PUT api/metadata/gender
    [HttpPost("{table}")]
    public async Task<IActionResult> Upsert(string table, MetadataDto dto)
    {
        var entityType = _db.Model.GetEntityTypes().FirstOrDefault(t => t.ClrType.Name.Equals(table, StringComparison.OrdinalIgnoreCase));
        if (entityType == null) return NotFound();
        
        var setMethod = typeof(DbContext).GetMethod(nameof(DbContext.Set), Type.EmptyTypes)!.MakeGenericMethod(entityType.ClrType);
        dynamic set = setMethod.Invoke(_db, null)!;
        
        var findAsyncMethod = set.GetType().GetMethod("FindAsync", new[] { typeof(object[]) });
        var findTask = findAsyncMethod.Invoke(set, new object[] { new object[] { dto.Id } });
        await (dynamic)findTask;
        var entity = ((dynamic)findTask).Result;
        
        if (entity == null)
        {
            entity = Activator.CreateInstance(entityType.ClrType)!;
            _mapper.Map(dto, entity);
            await set.AddAsync(entity);
        }
        else
        {
            _mapper.Map(dto, entity);
        }
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{table}/{id:int}")]
    public async Task<IActionResult> Delete(string table, int id)
    {
        var entityType = _db.Model.GetEntityTypes().FirstOrDefault(t => t.ClrType.Name.Equals(table, StringComparison.OrdinalIgnoreCase));
        if (entityType == null) return NotFound();
        
        var setMethod = typeof(DbContext).GetMethod(nameof(DbContext.Set), Type.EmptyTypes)!.MakeGenericMethod(entityType.ClrType);
        dynamic set = setMethod.Invoke(_db, null)!;
        
        var findAsyncMethod = set.GetType().GetMethod("FindAsync", new[] { typeof(object[]) });
        var findTask = findAsyncMethod.Invoke(set, new object[] { new object[] { id } });
        await (dynamic)findTask;
        var entity = ((dynamic)findTask).Result;
        
        if (entity == null) return NotFound();
        set.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
