using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Sim.Application.Dtos.Build;
using Sim.Application.Dtos.Simulate;
using Sim.Application.NanoServices;
using Sim.Domain.Logic;
using Sim.Domain.UiSchematic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Application.UseCases.BuildLogicModel;

public interface IBuildLogicModel : IUseCase
{
    public Task<BuildResult> Generate(UiSchemeModel uiModel);
}

public class BuildLogicModel(IMemoryCache cache, ILogger<BuildLogicModel> logger) : IBuildLogicModel
{
    private readonly IMemoryCache _cache = cache;
    private readonly ILogger<BuildLogicModel> _logger = logger;
    public async Task<BuildResult> Generate(UiSchemeModel uiModel)
    {

        var (relays, contacts) = Parser.Parse(uiModel);

        var model = new LogicModel(relays, contacts);

        return new BuildResult
        {
            SchemeId = model.Id.ToString(),
            Relays = relays.Select(r => new RelayLogicResult { Name = r.Name, Logic = r.State.ToLogic()}).ToList(),

        };
    }
}
