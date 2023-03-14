using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using REST_API.Data.Api.Controller.Base;
using REST_API.Data.Api.Dto.Vocable;
using REST_API.Data.Api.Dto.VocableCollection;
using REST_API.Data.Db;
using REST_API.Data.Db.Models;
using REST_API.Data.Db.Models.Vocable;
using REST_API.Utils.Extensions;

namespace REST_API.Data.Api.Controller;

public class VocableCollectionController : BaseController<DataContext, VocableCollectionController>
{
    public VocableCollectionController(DataContext context, ILogger<VocableCollectionController> logger) : base(context,
        logger)
    {
    }

    [HttpGet]
    public async Task<ActionResult<IList<VocableCollectionGetDto>>> All()
    {
        var user = await HttpContext.User.GenerateUserFromJwt(Context);
        if (user is null)
            return Unauthorized();
        var vocableCollections =
            await Context.VocableCollections.Where(it => it.Owner == user).ToListAsync();
        var getDtos = new List<VocableCollectionGetDto>();
        foreach (var collection in vocableCollections)
            getDtos.Add(new VocableCollectionGetDto(collection.Id, collection.Name,
                collection.Vocables.Select(it => new VocableGetDto(it.Id, it.Display, it.PossibleAnswers))));
        return Ok(getDtos);
    }

    [HttpPost]
    public async Task<ActionResult<VocableCollectionGetDto>> Create(VocableCollectionCreateDto createDto)
    {
        // Read User
        var user = await HttpContext.User.GenerateUserFromJwt(Context);
        if (user is null)
            return Unauthorized();

        // Check for existing Name
        if (await ValidateName(user, createDto.Name))
            return Conflict();

        // Create & Save Collection
        var collection = new VocableCollection
        {
            Name = createDto.Name,
            Owner = user,
            Vocables = new List<Vocable>()
        };
        await Context.VocableCollections.AddAsync(collection);

        return await GenerateResult(collection);
    }

    [HttpPut]
    public async Task<ActionResult<VocableCollectionGetDto>> Update(VocableCollectionUpdateDto updateDto)
    {
        // Read User
        var user = await HttpContext.User.GenerateUserFromJwt(Context);
        if (user is null)
            return Unauthorized();

        if (await ValidateName(user, updateDto.Name))
            return Conflict();

        var collection = await Context.VocableCollections.FindAsync(updateDto.Id);

        if (collection is null || collection.Owner != user)
            return Unauthorized();
        collection.Name = updateDto.Name;
        Context.VocableCollections.Update(collection);
        return await GenerateResult(collection);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await HttpContext.User.GenerateUserFromJwt(Context);
        if (user is null)
            return Unauthorized();

        var collection = await Context.VocableCollections.FindAsync(id);
        if (collection is null || collection.Owner != user)
            return Unauthorized();

        IList<Vocable> vocables = await Context.Vocables.Where(it => it.Collection == collection).ToListAsync();
        Context.Vocables.RemoveRange(vocables);
        Context.VocableCollections.Remove(collection);
        await Context.SaveChangesAsync();

        return Ok();
    }

    private async Task<bool> ValidateName(User user, string displayName)
    {
        var collection =
            await Context.VocableCollections
                .Where(it => it.Owner == user)
                .FirstOrDefaultAsync(it => it.Name.ToLower() == displayName.ToLower());
        return collection is not null;
    }


    private async Task<ActionResult<VocableCollectionGetDto>> GenerateResult(VocableCollection collection)
    {
        await Context.SaveChangesAsync();


        return Ok(new VocableCollectionGetDto(collection.Id, collection.Name,
            collection.Vocables.Select(it => new VocableGetDto(it.Id, it.Display, it.PossibleAnswers))));
    }
}