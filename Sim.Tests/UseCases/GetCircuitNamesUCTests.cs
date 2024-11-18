using FakeItEasy;
using Microsoft.Extensions.Logging;
using Shouldly;
using Sim.Application.Dtos.Simulate;
using Sim.Application.UseCases;
using Sim.Application.UseCases.CreateLogicModel;
using Sim.Application.UseCases.GetCircuitsUC;
using Sim.Application.UseCases.SimulateLogicModel;
using Sim.Domain.UiSchematic;
using Sim.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Tests.UseCases;

public class GetCircuitNamesUCTests
{

    private readonly ILogger<IUseCase> fakeLogger = A.Fake<ILogger<IUseCase>>();

    [Fact]
    public void GetCircuits_OK()
    {
        var useCase = new GetCircuitNamesUC(new Repository(), fakeLogger);
        var result = useCase.GetCircuits();

        result.Count().ShouldBe(12);
        result[0].ShouldBe("BridgeConnection.json");
    }
}
