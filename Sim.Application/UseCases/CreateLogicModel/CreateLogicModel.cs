﻿using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
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

namespace Sim.Application.UseCases.CreateLogicModel;


public interface ICreateLogicModel : IUseCase
{
    public Task<SimulateResult> Generate(UiSchemeModel uiModel);
}

public class CreateLogicModel(IMemoryCache cache, ILogger<CreateLogicModel> logger) : ICreateLogicModel
{
    private readonly IMemoryCache _cache = cache;
    private readonly ILogger<CreateLogicModel> _logger = logger;
    public async Task<SimulateResult> Generate(UiSchemeModel uiModel) 
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        var (relays, contacts) = Parser.Parse(uiModel);
        stopwatch.Stop();
        //Console.WriteLine("Parse elapsed time: " + stopwatch.Elapsed);
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

