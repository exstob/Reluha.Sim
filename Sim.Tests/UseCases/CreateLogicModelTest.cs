using Shouldly;
using Sim.Application.UseCases.CreateLogicModel;
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
    public CreateLogicModelTest()
    {
        repo = new Repository();
    }

    [Fact]
    public async void CreateModel_OK()
    {
        var scheme = repo.GetUiScheme();
        var model = new CreateLogicModel();

        var result = await model.Generate(scheme);

        result.ShouldNotBeNull();
    }
}
