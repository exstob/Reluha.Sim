using FakeItEasy;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<CreateLogicModel> fakeLogger = A.Fake<ILogger<CreateLogicModel>>();

    public CreateLogicModelTest()
    {
        repo = new Repository();
        cache = new MemoryCache(new MemoryCacheOptions());
    }

    [Theory]
    [InlineData("SerialConnections.json", "(Plus & !x.R1 & x.R2) ^ Minus")]
    [InlineData("ParallelConnections.json", "(Plus & (x.R1 | x.R2)) ^ Minus")]
    [InlineData("SerialAndParallelConnections.json", "(Plus & (x.R1 | x.R2) & ((x.R3 & x.R4) | (x.R5 & x.R6 & x.R7)) & !x.R8) ^ Minus")]
    [InlineData("DualContactGroup.json", "(Minus & (x.R1 | (!x.R1 & x.R2))) ^ Plus")]
    [InlineData("ParallelOfParallelConnection.json", "(Plus & (((x.R6 | x.R5) & ((x.R7 & x.R9) | (x.R8 & x.R10)) & (x.R11 | x.R12)) | ((x.R18 | x.R19) & (x.R20 | x.R21) & (x.R22 | x.R23) & (x.R24 | x.R25) & (x.R26 | x.R27)) | ((x.R2 | x.R1) & (x.R3 | x.R4)))) ^ (Minus & (x.R15 | x.R16 | x.R17) & (x.R13 | x.R14))")]
    [InlineData("ParallelAndChangePolarity.json", "(((Plus & (x.R1 | x.R21) & x.R25) | (Minus & (x.R22 | x.R23 | !x.R24 | !x.R1) & !x.R25)) & x.R26) ^ (((Plus & (x.R32 | !x.R1) & (x.R29 | x.R30) & x.R28) | (Minus & (!x.R33 | x.R1) & (!x.R31 & !x.R28))) & x.R27)")]
    [InlineData("BridgeConnection.json", "(Plus & ((!x.R2 & (!x.R5 | (!x.R3 & !x.R4))) | (!x.R1 & (!x.R4 | (!x.R3 & !x.R5))))) ^ Minus")]
    [InlineData("SerialAndParallelAndDualConnections.json", "(Plus & (((x.R1 & x.R3) & (x.R14 | (((x.R5 & x.R4) | ((x.R6 & x.R2) & !x.R2)) & (!x.R4 & (((x.R13 & x.R12) & (x.R11 | (!x.R10 & (x.R9 | ((x.R8 & x.R7) & x.R10))))) | (((x.R9 | ((x.R8 & x.R7) & x.R10)) | (!x.R10 & x.R11)) & (x.R10 | ((x.R8 & x.R7) & x.R9)))))))) | ((!x.R1 & (!x.R2 | ((x.R6 & x.R2) & (x.R5 & x.R4)))) & ((!x.R4 & (((x.R13 & x.R12) & (x.R11 | (!x.R10 & (x.R9 | ((x.R8 & x.R7) & x.R10))))) | (((x.R9 | ((x.R8 & x.R7) & x.R10)) | (!x.R10 & x.R11)) & (x.R10 | ((x.R8 & x.R7) & x.R9))))) | (((x.R5 & x.R4) | ((x.R6 & x.R2) & !x.R2)) & x.R14))))) ^ ((Minus & x.R18) & (!x.R17 | x.R16 | x.R15))")]
    [InlineData("ShortCircuitState.json", "((Plus & (x.R2 | (x.R1 & x.R3))) | (Minus & (x.R3 | (x.R1 & x.R2)))) ^ ((Plus & (!x.R4 | (!x.R6 & !x.R5))) | (Minus & (!x.R5 | (!x.R6 & !x.R4))))")]
    public async Task CreateModelWithOneRelay_OK(string fileName, string logicResult)
    {
        var scheme = repo.GetUiScheme(fileName);
        var useCase = new CreateLogicModel(cache, fakeLogger);

        var result = await useCase.Generate(scheme);

        result.ShouldNotBeNull();
        cache.TryGetValue<LogicModel>(result.SchemeId.ToString(), out var model);

        model.Relays[0].State.ToLogic().ShouldBe(logicResult);
    }

    [Theory]
    [InlineData("RelaySerialChain.json")]
    public async Task CreateModelWithMultiRelay_OK(string fileName)
    {
        var scheme = repo.GetUiScheme(fileName);
        var useCase = new CreateLogicModel(cache, fakeLogger);

        var result = await useCase.Generate(scheme);

        result.ShouldNotBeNull();
        cache.TryGetValue<LogicModel>(result.SchemeId.ToString(), out var model);
        var r5= model.Relays.Find(r => r.Name == "R5");
        var r10 = model.Relays.Find(r => r.Name == "R10");

        r5.State.ToLogic().ShouldBe("(Plus & x.R4) ^ Minus");
        r10.State.ToLogic().ShouldBe("(Minus & x.R9) ^ Plus");
    }
}
