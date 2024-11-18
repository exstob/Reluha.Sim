using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Sim.Application.NanoServices;
using Sim.Domain.Logic;
using Sim.Domain.UiSchematic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sim.Application.Dtos.Simulate;
using Sim.Domain.ParsedScheme;

namespace Sim.Application.UseCases.CreateLogicModel;


public interface IRunLogicModel : IUseCase
{
    public Task<SimulateResult> Generate(UiSchemeModel uiModel);
}

public class RunLogicModel(IMemoryCache cache, ILogger<RunLogicModel> logger) : IRunLogicModel
{
    private readonly IMemoryCache _cache = cache;
    private readonly ILogger<RunLogicModel> _logger = logger;
    public async Task<SimulateResult> Generate(UiSchemeModel uiModel) 
    {            
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        LogicModel model;
        if (uiModel.compiledSchemeId is not null && _cache.TryGetValue<LogicModel>(uiModel.compiledSchemeId, out var cachedModel) && cachedModel is not null)
        {
            model = cachedModel;
            _logger.LogInformation(message: "Use cached model " + cachedModel.Id);
        }
        else
        {
            var (relays, contacts) = Parser.Parse(uiModel);
            model = new LogicModel(relays, contacts);
            _cache.Set(model.Id.ToString(), model, TimeSpan.FromMinutes(10));
        }

        var evalRelays = await model.EvaluateAll();
        stopwatch.Stop();
        _logger.LogInformation("Run model elapsed time: " + stopwatch.Elapsed.TotalMilliseconds);

        return new SimulateResult
        {
            SchemeId = model.Id.ToString(),
            Steps = [new SimulateStepResult
                {
                    StepName = "Init",
                    Relays = evalRelays.Select(r => new RelayResult { Name = r.Name, NormalContact = r.State.NormalContact, PolarContact = r.State.PolarContact }).ToList(),
                }]
        };
    }
}

