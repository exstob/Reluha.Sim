using FakeItEasy;
using Microsoft.Extensions.Logging;
using Sim.Application.UseCases.GetCircuitsUC;
using Sim.Application.UseCases;
using Sim.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sim.Application.UseCases.GetCircuitUC;
using Shouldly;

namespace Sim.Tests.UseCases;

public class GetCircuitUCTests
{

    private readonly ILogger<IUseCase> fakeLogger = A.Fake<ILogger<IUseCase>>();

    [Fact]
    public void GetCircuits_OK()
    {
        var useCase = new GetCircuitUC(new Repository(), fakeLogger);
        var result = useCase.GetCircuit("RelaySerialChain.json");

        result.Name.ShouldBe("RelaySerialChain.json");
    }
}