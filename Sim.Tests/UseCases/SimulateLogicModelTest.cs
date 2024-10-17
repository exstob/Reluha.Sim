using FakeItEasy;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Shouldly;
using Sim.Application.Dtos.Simulate;
using Sim.Application.UseCases.CreateLogicModel;
using Sim.Application.UseCases.SimulateLogicModel;
using Sim.Domain.UiSchematic;
using Sim.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Tests.UseCases;

public class SimulateLogicModelTest
{
    readonly Repository repo;
    private readonly IMemoryCache cache;
    private readonly ILogger<CreateLogicModel> fakeCreateLogger = A.Fake<ILogger<CreateLogicModel>>();
    private readonly ILogger<SimulateLogicModel> fakeSimLogger = A.Fake<ILogger<SimulateLogicModel>>();
    public SimulateLogicModelTest()
    {
        repo = new Repository();
        cache = A.Fake<IMemoryCache>();
    }

    [Fact]
    public async void SimulateModel_OK()
    {
        var scheme = repo.GetUiScheme();
        var createUseCase = new CreateLogicModel(cache, fakeCreateLogger);

        var model = await createUseCase.Generate(scheme);

        var simUseCase = new SimulateLogicModel(cache, fakeSimLogger);

        var switcher = new UiSwitcher { LogicState = true, Name = "R8801", ExtraProps = new UiSwitcherExtraProps("normal", true) };
        var simData = new SimulateData { SchemeId = model.SchemeId.ToString(), Steps = [new SimulateStep { StepName = "st1", Switchers = [switcher] }] };


        simData.ShouldNotBeNull();
    }
}
