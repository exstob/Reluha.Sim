using FakeItEasy;
using Microsoft.Extensions.Caching.Memory;
using Shouldly;
using Sim.Application.UseCases.CreateLogicModel;
using Sim.Domain.Logic;
using Sim.Domain.ParsedScheme;
using Sim.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Tests.UseCases;

public class CreateLogicModelTest
{
    readonly Repository repo;
    private readonly IMemoryCache cache;
    public CreateLogicModelTest()
    {
        repo = new Repository();
        cache = new MemoryCache(new MemoryCacheOptions());
    }

    [Theory]
    //[InlineData("SerialConnections.json")]
    [InlineData("ParallelConnections.json")]
    public async Task CreateModel_OK(string fileName)
    {
        var scheme = repo.GetUiScheme(fileName);
        var useCase = new CreateLogicModel(cache);

        var result = await useCase.Generate(scheme);

        result.ShouldNotBeNull();
        cache.TryGetValue<LogicModel>(result.Id.ToString(), out var model);

        model.Relays[0].State.ToLogic().ShouldBe("(Plus & (x.R1 | x.R2)) ^ Minus");
    }
}
