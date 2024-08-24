using Microsoft.Extensions.Caching.Memory;
using Sim.Application.NanoServices;
using Sim.Domain.Logic;
using Sim.Domain.UiSchematic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Application.UseCases.CreateLogicModel;


public interface ICreateLogicModel : IUseCase
{
    public Task<LogicModel> Generate(UiSchemeModel uiModel);
}

public class CreateLogicModel(IMemoryCache cache) : ICreateLogicModel
{
    private readonly IMemoryCache _cache = cache;
    public async Task<LogicModel> Generate(UiSchemeModel uiModel) 
    {
        var (relays, contacts) = Parser.Parse(uiModel);

        var model = new LogicModel(relays, contacts);
        await model.Evaluate();
        _cache.Set(model.Id.ToString(), model, TimeSpan.FromMinutes(10));

        return model;
    }
}

