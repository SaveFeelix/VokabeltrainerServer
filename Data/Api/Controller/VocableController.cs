using Microsoft.AspNetCore.Mvc;
using REST_API.Data.Api.Controller.Base;
using REST_API.Data.Api.Dto.Vocable;
using REST_API.Data.Api.Dto.VocableCollection;
using REST_API.Data.Db;
using REST_API.Data.Db.Models.Vocable;
using REST_API.Utils.Extensions;

namespace REST_API.Data.Api.Controller;

public class VocableController : BaseController<DataContext, VocableController>
{
    public VocableController(DataContext context, ILogger<VocableController> logger) : base(context, logger)
    {
    }

    [HttpPost]
    public async Task<ActionResult<VocableCollectionGetDto>> Create(VocableCreateDto createDto)
    {
        var user = await HttpContext.User.GenerateUserFromJwt(Context);
        if (user is null)
            return Unauthorized();
        var collection = await Context.VocableCollections.FindAsync(createDto.CollectionId);
        if (collection is null || collection.Owner != user)
            return Unauthorized();

        var vocable = new Vocable
        {
            Collection = collection,
            Display = createDto.Display,
            PossibleAnswers = createDto.PossibleAnswers
        };
        await Context.Vocables.AddAsync(vocable);
        await Context.SaveChangesAsync();


        return Ok(new VocableCollectionGetDto(collection.Id, collection.Name,
            collection.Vocables.Select(it => new VocableGetDto(it.Id, it.Display, it.PossibleAnswers))));
    }

    [HttpPut]
    public async Task<ActionResult<VocableCollectionGetDto>> Update(VocableUpdateDto updateDto)
    {
        var user = await HttpContext.User.GenerateUserFromJwt(Context);
        if (user is null)
            return Unauthorized();
        var vocable = await Context.Vocables.FindAsync(updateDto.Id);
        if (vocable is null || vocable.Collection.Owner != user)
            return Unauthorized();

        vocable.Display = updateDto.Display;
        vocable.PossibleAnswers = updateDto.PossibleAnswers;


        Context.Vocables.Update(vocable);

        return await SaveAndGet(vocable);
    }

    [HttpDelete]
    public async Task<ActionResult<VocableCollectionGetDto>> Delete(int id)
    {
        var user = await HttpContext.User.GenerateUserFromJwt(Context);
        if (user is null)
            return Unauthorized();
        var vocable = await Context.Vocables.FindAsync(id);
        if (vocable is null || vocable.Collection.Owner != user)
            return Unauthorized();

        Context.Vocables.Remove(vocable);

        return await SaveAndGet(vocable);
    }

    private async Task<ActionResult<VocableCollectionGetDto>> SaveAndGet(Vocable vocable)
    {
        await Context.SaveChangesAsync();
        return Ok(new VocableCollectionGetDto(vocable.Collection.Id, vocable.Collection.Name,
            vocable.Collection.Vocables.Select(it => new VocableGetDto(it.Id, it.Display, it.PossibleAnswers))));
    }
}