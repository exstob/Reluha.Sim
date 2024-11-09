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
        var (relays, contacts) = Parser.Parse(uiModel);
        stopwatch.Stop();
        _logger.LogInformation("Parse elapsed time: " + stopwatch.Elapsed.TotalMilliseconds);

        var model = new LogicModel(relays, contacts);

        stopwatch.Reset();
        stopwatch.Start();
        relays = await model.EvaluateAll();
        stopwatch.Stop();
        //Console.WriteLine("Evaluate elapsed time: " + stopwatch.Elapsed);
        _logger.LogInformation("Evaluate elapsed time: " + stopwatch.Elapsed.TotalMilliseconds);

        _cache.Set(model.Id.ToString(), model, TimeSpan.FromMinutes(10));

        return new SimulateResult
        {
            SchemeId = model.Id.ToString(),
            Steps = [new SimulateStepResult
                {
                    StepName = "Init",
                    Relays = relays.Select(r => new RelayResult { Name = r.Name, NormalContact = r.State.NormalContact, PolarContact = r.State.PolarContact }).ToList(),
                }]
        };
    }
}

