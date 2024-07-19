using Sim.Application.UseCases.CreateSchemeLogicModel;
using Sim.Domain;

namespace Sim.Application;

public class SimModelManager: ISimModel
{
    static public async Task Create(SchemeElements elements) 
    {
        var model = CreateSchemeLogicModel.Generate(elements);

        await model.Evaluate();

        var updated = await model.Evaluate();
    }

}

