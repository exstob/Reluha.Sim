using Microsoft.Extensions.Caching.Memory;
using Sim.Application.Dtos.Simulate;
using Sim.Domain.Logic;
using Sim.Domain.UiSchematic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sim.Domain.ParsedScheme;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Sim.Application.UseCases.SimulateLogicModel;

public interface ISimulateLogicModel : IUseCase
{
    public Task<SimulateResult> Simulate(SimulateData simReq);
}

public class SimulateLogicModel(IMemoryCache cache, ILogger<SimulateLogicModel> logger) : ISimulateLogicModel
{
    private readonly IMemoryCache _cache = cache;
    private readonly ILogger<SimulateLogicModel> _logger = logger;
    public async Task<SimulateResult> Simulate(SimulateData simReq)
    {
        List<Relay> relays = [];
        if (_cache.TryGetValue<LogicModel>(simReq.SchemeId, out var model) && model is not null)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var switchers = simReq.Steps.Single().Switchers;
            foreach (var switcher in switchers)
            {
                model.UpdateContact(Contact.FullName(switcher), switcher.LogicState);
            }

            relays = await model.EvaluateAll();
            stopwatch.Stop();
            _logger.LogInformation("Simulate elapsed time: " + stopwatch.Elapsed.TotalMilliseconds);
        }

        return new SimulateResult
        {
            SchemeId = simReq.SchemeId,
            Steps = [new SimulateStepResult
                {
                    StepName = simReq.Steps.Single().StepName,
                    Relays = relays.Select(r => new RelayResult { Name = r.Name, NormalContact = r.State.NormalContact, PolarContact = r.State.PolarContact }).ToList(),
                }]
        };
    }
}
